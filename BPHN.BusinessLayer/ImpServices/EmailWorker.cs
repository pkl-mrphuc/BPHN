using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BPHN.BusinessLayer.ImpServices
{
    public class EmailWorker : IEmailWorker
    {
        private readonly IMailBuilderFactory _mailFactory;
        private readonly AppSettings _appSettings;
        public EmailWorker(IMailBuilderFactory mailFactory, IOptions<AppSettings> appSettings)
        {
            _mailFactory = mailFactory;
            _appSettings = appSettings.Value;
        }

        public async Task Handle(string dataJson)
        {
            if (string.IsNullOrWhiteSpace(dataJson))
            {
                throw new Exception("Input Empty");
            }

            var sendMail = JsonConvert.DeserializeObject<SendMailParameter>(dataJson);

            if (sendMail is null)
            {
                throw new Exception("Input Empty");
            }

            var parameterType = sendMail.ParameterType;
            var data = JsonConvert.DeserializeObject(dataJson, parameterType);
            var builder = _mailFactory.GetInstance(sendMail.MailType);

            var body = await builder.BuildBody(data);

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new Exception("Build Body Fail");
            }

            var message = new MailMessage(
                from: _appSettings.MailConfiguration.Mail,
                to: sendMail.ReceiverAddress,
                subject: builder.BuildSubject(data),
                body: body
            );
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Sender = new MailAddress(_appSettings.MailConfiguration.Mail, _appSettings.MailConfiguration.DisplayName);
            if (sendMail.HasAttachmentFile)
            {
                var files = builder.BuildAttachments(data);
                for (int i = 0; i < files.Count; i++)
                {
                    message.Attachments.Add(files[i]);
                }
            }

            using (var client = new SmtpClient(_appSettings.MailConfiguration.Host))
            {
                client.Port = _appSettings.MailConfiguration.Port;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_appSettings.MailConfiguration.Mail, _appSettings.MailConfiguration.Password);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.Send(message);
            }
        }
    }
}
