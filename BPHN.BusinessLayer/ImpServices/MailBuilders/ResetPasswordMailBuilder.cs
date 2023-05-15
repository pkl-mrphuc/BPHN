﻿using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            if (data != null)
            {
                using (var client = new HttpClient())
                {
                    var resetPasswordParam = (ResetPasswordParameter)data;
                    var vm = new MailVm<MailResetPasswordVm>()
                    {
                        Model = new MailResetPasswordVm()
                        {
                            AccountId = resetPasswordParam.AccountId,
                            FullName = resetPasswordParam.FullName,
                            UserName = resetPasswordParam.UserName
                        }
                    };
                    var stringContent = new StringContent(JsonConvert.SerializeObject(vm), UnicodeEncoding.UTF8, "application/json");
                    var response = await client.PostAsync(string.Format("{0}{1}", _appSettings.MailTemplateAPI, "reset-password"), stringContent);
                    if(response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;
                    }
                }
            }
            return "";
        }

        public string BuildSubject(object? data)
        {
            return "[BPHN] Reset Your Password";
        }
    }
}
