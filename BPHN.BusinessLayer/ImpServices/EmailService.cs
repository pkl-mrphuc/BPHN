using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BPHN.BusinessLayer.ImpServices
{
    public class EmailService : BaseService, IEmailService
    {
        private readonly IRabbitMQProducerService _producer;
        private readonly IKeyGenerator _keyGenerator;
        private IOptions<AppSettings> _;

        public EmailService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IRabbitMQProducerService producer,
            IKeyGenerator keyGenerator) : base(serviceProvider, appSettings)
        {
            _producer = producer;
            _keyGenerator = keyGenerator;
            _ = appSettings;
        }

        public bool SendMail<T>(string type, T data)
        {
            //_producer.Publish(new ObjectQueue
            //{
            //    QueueJobType = QueueJobTypeEnum.SENDMAIL,
            //    DataJson = JsonConvert.SerializeObject(data),
            //    DataType = type
            //});
            Handle(JsonConvert.SerializeObject(data));
            return true;
        }

        private async Task Handle(string dataJson)
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
            IMailBuilder builder;
            switch (sendMail.MailType)
            {
                case MailTypeEnum.FORTGOTPASSWORD:
                    builder = new ForgotPasswordMailBuilder(_, _keyGenerator);
                    break;
                case MailTypeEnum.SETPASSWORD:
                    builder = new SetPasswordMailBuilder(_, _keyGenerator);
                    break;
                case MailTypeEnum.APPROVALBOOKING:
                    builder = new ApprovalBookingMailBuilder(_);
                    break;
                case MailTypeEnum.DECLINEBOOKING:
                    builder = new DeclineBookingMailBuilder(_);
                    break;
                default:
                    throw new Exception("GetInstance Fail");
            }

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
