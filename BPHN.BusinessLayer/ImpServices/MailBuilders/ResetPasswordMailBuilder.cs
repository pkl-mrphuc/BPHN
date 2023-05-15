using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices.MailBuilders
{
    public class ResetPasswordMailBuilder : IMailBuilder
    {
        private readonly AppSettings _appSettings;
        public ResetPasswordMailBuilder(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public List<Attachment> BuildAttachments(object? data)
        {
            return new List<Attachment>();
        }

        public async Task<string> BuildBody(object? data)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    
                };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(string.Format("{0}{1}", _appSettings.MailTemplateAPI, "reset-password"), content);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }

        public string BuildSubject(object? data)
        {
            return "[BPHN] Reset Your Password";
        }
    }
}
