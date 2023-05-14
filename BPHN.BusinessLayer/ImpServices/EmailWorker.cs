using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.ObjectQueues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;

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
            if (string.IsNullOrEmpty(dataJson))
            {
                throw new Exception("Input Empty");
            }

            SendMailParameter sendMail = JsonConvert.DeserializeObject<SendMailParameter>(dataJson);
            Type parameterType = sendMail.ParameterType;
            var data = JsonConvert.DeserializeObject(dataJson, parameterType);
            IMailBuilder builder = _mailFactory.GetInstance(sendMail.MailType);

            MailMessage message = new MailMessage(
                from: _appSettings.MailConfiguration.Mail,
                to: sendMail.ReceiverAddress,
                subject: builder.BuildSubject(data),
                body: builder.BuildBody(data)
            );
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Sender = new MailAddress(_appSettings.MailConfiguration.Mail, _appSettings.MailConfiguration.DisplayName);
            if(sendMail.HasAttachmentFile)
            {
                var files = builder.BuildAttachments(data);
                for (int i = 0; i < files.Count; i++)
                {
                    message.Attachments.Add(files[i]);
                }
            }

            using (SmtpClient client = new SmtpClient(_appSettings.MailConfiguration.Host))
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
