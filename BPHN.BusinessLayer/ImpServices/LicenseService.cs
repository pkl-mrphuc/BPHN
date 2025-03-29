using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class LicenseService : BaseService, ILicenseService
    {
        private readonly ILicenseRepository _licenseRepository;
        private readonly IOverviewService _overviewService;
        public LicenseService(
            IServiceProvider provider,
            IOptions<AppSettings> appSettings,
            ILicenseRepository licenseRepository,
            IOverviewService overviewService) : base(provider, appSettings)
        {
            _licenseRepository = licenseRepository;
            _overviewService = overviewService;
        }

        public async Task<bool> CheckIsValid(Guid accountId)
        {
            var result = false;

            var data = await _overviewService.GetTotalInvoices(accountId);
            if (data is not null)
            {
                result = (data.ExpireTime ?? DateTime.MinValue) > DateTime.Now && data.MaxDraft > data.Draft + 1 && data.MaxPublished > data.Published + 1;
            }
            return result;
        }

        public async Task<bool> Insert(License data)
        {
            switch (data.Type)
            {
                case LicenseTypeEnum.TRIAL:
                    data.MaxInvoices = 1000;
                    data.MaxDraftInvoices = 0;
                    break;
                case LicenseTypeEnum.BASIC:
                    data.MaxInvoices = 1200;
                    data.MaxDraftInvoices = 10;
                    break;
                case LicenseTypeEnum.PREMIUM:
                    data.MaxInvoices = 1500;
                    data.MaxDraftInvoices = 20;
                    break;
                case LicenseTypeEnum.ENTERPRISE:
                    data.MaxInvoices = 2000;
                    data.MaxDraftInvoices = 100;
                    break;
                default:
                    throw new Exception();
            }

            return await _licenseRepository.Insert(data);
        }

        public async Task<bool> Update(License data)
        {
            var license = await _licenseRepository.Get(data.AccountId);
            if (license is not null)
            {
                data.Id = license.Id;
                switch (data.Type)
                {
                    case LicenseTypeEnum.BASIC:
                        data.MaxInvoices = license.MaxInvoices + 200;
                        data.MaxDraftInvoices = 10;
                        break;
                    case LicenseTypeEnum.PREMIUM:
                        data.MaxInvoices = license.MaxInvoices + 500;
                        data.MaxDraftInvoices = 20;
                        break;
                    case LicenseTypeEnum.ENTERPRISE:
                        data.MaxInvoices = license.MaxInvoices + 1000;
                        data.MaxDraftInvoices = 100;
                        break;
                    default:
                        break;
                }

                return await _licenseRepository.Update(data);
            }
            return false;
        }
    }
}
