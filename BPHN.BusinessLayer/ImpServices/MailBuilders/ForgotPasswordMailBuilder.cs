﻿using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;

namespace BPHN.BusinessLayer.ImpServices.MailBuilders
{
    public class ForgotPasswordMailBuilder : IMailBuilder
    {
        private readonly AppSettings _appSettings;
        private readonly IKeyGenerator _keyGenerator;
        public ForgotPasswordMailBuilder(IOptions<AppSettings> appSettings, IKeyGenerator keyGenerator)
        {
            _appSettings = appSettings.Value;
            _keyGenerator = keyGenerator;
        }

        public List<Attachment> BuildAttachments(object? data)
        {
            return new List<Attachment>();
        }

        public async Task<string> BuildBody(object? data)
        {
            if (data != null && _appSettings != null && !string.IsNullOrWhiteSpace(_appSettings.MailTemplateAPI))
            {
                using (var client = new HttpClient())
                {
                    var setPasswordParam = (SetPasswordParameter)data;
                    if(setPasswordParam != null)
                    {
                        string key = _keyGenerator.Encryption(JsonConvert.SerializeObject(
                                                        new ExpireSetPasswordModel()
                                                        {
                                                            ExpireTime = DateTime.Now.AddMinutes(30),
                                                            AccountId = setPasswordParam.AccountId.ToString()
                                                        }
                                                    ));

                        string link = string.Format("{0}/set-password?code={1}&userName={2}", _appSettings.ClientHost, key, setPasswordParam.UserName);
                        var vm = new MailVm<MailForgotPasswordVm>()
                        {
                            Model = new MailForgotPasswordVm()
                            {
                                AccountId = setPasswordParam.AccountId,
                                FullName = setPasswordParam.FullName,
                                UserName = setPasswordParam.UserName,
                                Key = key,
                                Link = link
                            }
                        };
                        var stringContent = new StringContent(JsonConvert.SerializeObject(vm), UnicodeEncoding.UTF8, "application/json");
                        var response = await client.PostAsync($"{_appSettings.MailTemplateAPI}/forgot-password", stringContent);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            return responseString;
                        }
                    }
                }
            }
            return string.Empty;
        }

        public string BuildSubject(object? data)
        {
            return "[BPHN] QUÊN MẬT KHẨU";
        }
    }
}
