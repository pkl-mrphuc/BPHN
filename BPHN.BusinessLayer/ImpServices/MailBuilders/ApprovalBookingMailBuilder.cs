using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;

namespace BPHN.BusinessLayer.ImpServices.MailBuilders
{
    public class ApprovalBookingMailBuilder : IMailBuilder
    {
        private readonly AppSettings _appSettings;

        public ApprovalBookingMailBuilder(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;   
        }

        public List<Attachment> BuildAttachments(object? data)
        {
            return new List<Attachment>();
        }

        public async Task<string> BuildBody(object? data)
        {
            if (data is not null && _appSettings is not null && !string.IsNullOrWhiteSpace(_appSettings.MailTemplateAPI))
            {
                using (var client = new HttpClient())
                {
                    var approvalBookingParam = (ApprovalBookingParameter)data;
                    if (approvalBookingParam is not null)
                    {
                        var vm = new MailVm<MailApprovalBookingVm>()
                        {
                            Model = new MailApprovalBookingVm
                            {
                                BookingDate = approvalBookingParam.BookingDate,
                                MatchDate = approvalBookingParam.MatchDate,
                                NameDetail = approvalBookingParam.NameDetail,
                                PhoneNumber = approvalBookingParam.PhoneNumber,
                                Price = approvalBookingParam.Price,
                                StadiumName = approvalBookingParam.StadiumName,
                                TimeFrameInfo = approvalBookingParam.TimeFrameInfo
                            }
                        };

                        var stringContent = new StringContent(JsonConvert.SerializeObject(vm), UnicodeEncoding.UTF8, "application/json");
                        var response = await client.PostAsync($"{_appSettings.MailTemplateAPI}/approval-booking", stringContent);
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
            return "[BPHN] PHÊ DUYỆT THÔNG TIN ĐẶT SÂN";
        }
    }
}
