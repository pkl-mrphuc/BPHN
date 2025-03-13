using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.ImpRepositories;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Responses;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class InvoiceService : BaseService, IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPermissionService _permissionService;
        public InvoiceService(
            IServiceProvider provider, 
            IOptions<AppSettings> appSettings,
            IPermissionService permissionService,
            IInvoiceRepository invoiceRepository) : base(provider, appSettings)
        {
            _permissionService = permissionService;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<ServiceResultModel> GetInstance(string id)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            if (!string.IsNullOrWhiteSpace(id) && !Guid.TryParse(id, out var invoiceId))
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
                };
            }

            var data = new Invoice();
            data.Id = Guid.NewGuid();
            if (Guid.TryParse(id, out invoiceId))
            {
                data = await _invoiceRepository.GetById(invoiceId);
                if (data is null)
                {
                    return new ServiceResultModel
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig)
                    };
                }
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<InvoiceRespond>(data)
            };
        }

        public async Task<ServiceResultModel> GetInvoices()
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTINVOICE);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var lstInvoice = await _invoiceRepository.GetInvoices(context.Id);
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<InvoiceRespond>>(lstInvoice)
            };
        }
    }
}
