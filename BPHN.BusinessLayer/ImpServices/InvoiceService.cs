using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class InvoiceService : BaseService, IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IHistoryLogService _historyLogService;
        private readonly IItemService _itemService;
        private readonly ILicenseService _licenseService;
        public InvoiceService(
            IServiceProvider provider,
            IOptions<AppSettings> appSettings,
            IPermissionService permissionService,
            IInvoiceRepository invoiceRepository,
            INotificationService notificationService,
            IHistoryLogService historyLogService,
            IItemService itemService,
            ILicenseService licenseService) : base(provider, appSettings)
        {
            _permissionService = permissionService;
            _invoiceRepository = invoiceRepository;
            _notificationService = notificationService;
            _historyLogService = historyLogService;
            _itemService = itemService;
            _licenseService = licenseService;
        }

        public async Task<ServiceResultModel> GetInstance(string id)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            if (!string.IsNullOrWhiteSpace(id) && !Guid.TryParse(id, out var invoiceId))
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            var data = new Invoice();
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out invoiceId))
            {
                data = await _invoiceRepository.GetById(invoiceId);
                if (data is null)
                {
                    return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig));
                }

                if (!string.IsNullOrWhiteSpace(data.Detail))
                {
                    data.Items = JsonConvert.DeserializeObject<List<InvoiceItem>>(data.Detail);
                }
            }
            else
            {
                data.Id = Guid.NewGuid();
            }


            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<GetSingleInvoiceRespond>(data)
            };
        }

        public async Task<ServiceResultModel> GetByBooking(string id)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var bookingDetailId))
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            var data = await _invoiceRepository.GetByBooking(bookingDetailId);
            if (data is not null && !string.IsNullOrWhiteSpace(data.Detail))
            {
                data.Items = JsonConvert.DeserializeObject<List<InvoiceItem>>(data.Detail);
            }
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<GetSingleInvoiceRespond>(data)
            };
        }

        public async Task<ServiceResultModel> GetInvoices(string txtSearch, string status, int? customerType, DateTime? date, int? paymentType)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.VIEWLISTINVOICE);
            if (!hasPermission)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            var lstInvoice = await _invoiceRepository.GetInvoices(context.Id, txtSearch, status, customerType, date, paymentType);
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<InvoiceRespond>>(lstInvoice)
            };
        }

        public async Task<ServiceResultModel> Insert(Invoice data, Guid? bookingDetailId)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.ADDINVOICE);
            if (!hasPermission)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            var isValid = ValidateModelByAttribute(data, "Id") || data.Items == null || data.Items.Count() == 0;
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            var isValidLicense = await _licenseService.CheckIsValid(context.Id);
            if (!isValidLicense)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_LICENSE, _resourceService.Get(SharedResourceKey.INVALIDLICENSE, context.LanguageConfig));
            }

            var isValidQuantity = await _itemService.CheckQuantityInStock(data.Items ?? Enumerable.Empty<InvoiceItem>());
            if (!isValidQuantity)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_DATA, _resourceService.Get(SharedResourceKey.INVALIDDATA, context.LanguageConfig));
            }

            data.Id = Guid.NewGuid();
            data.Detail = JsonConvert.SerializeObject(data.Items);
            data.Date = DateTime.Now;
            data.AccountId = context.ParentId ?? context.Id;
            data.CreatedBy = context.FullName;
            data.CreatedDate = DateTime.Now;
            data.ModifiedBy = context.FullName;
            data.ModifiedDate = DateTime.Now;

            var _ = bookingDetailId.HasValue ? new InvoiceBookingDetail
            {
                Id = Guid.NewGuid(),
                InvoiceId = data.Id,
                BookingDetailId = bookingDetailId.Value,
                CreatedBy = context.FullName,
                CreatedDate = DateTime.Now,
                ModifiedBy = context.FullName,
                ModifiedDate = DateTime.Now,
            } : null;


            var insertResult = await _invoiceRepository.Insert(data, _);
            if (insertResult)
            {
                if (data.Items is not null && InvoiceStatusEnum.PAID.ToString().Equals(data.Status))
                {
                    await _itemService.UpdateQuantity(data.Items.Where(x => x.ItemId != Guid.Empty));
                }
                await _notificationService.Insert(context, NotificationTypeEnum.INSERTINVOICE, new Invoice
                {
                    CustomerType = data.CustomerType,
                    CustomerName = data.CustomerName,
                    CustomerPhone = data.CustomerPhone,
                    Total = data.Total
                });

                _historyLogService.Write(Guid.NewGuid(), new HistoryLog
                {
                    ActionType = ActionEnum.INSERT,
                    Entity = EntityEnum.INVOICE.ToString(),
                    Data = new HistoryLogDescription
                    {
                        ModelId = data.Id,
                        NewData = JsonConvert.SerializeObject(data)
                    }
                }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = insertResult
            };
        }

        public async Task<ServiceResultModel> Update(Invoice data)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.EDITINVOICE);
            if (!hasPermission)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            var isValid = ValidateModelByAttribute(data) || data.Items == null || data.Items.Count() == 0;
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            var isValidQuantity = await _itemService.CheckQuantityInStock(data.Items ?? Enumerable.Empty<InvoiceItem>());
            if (!isValidQuantity)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_DATA, _resourceService.Get(SharedResourceKey.INVALIDDATA, context.LanguageConfig));
            }

            data.ModifiedBy = context.FullName;
            data.ModifiedDate = DateTime.Now;
            data.Date = DateTime.Now;
            data.Detail = JsonConvert.SerializeObject(data.Items);

            var oldInvoice = await _invoiceRepository.GetById(data.Id);
            var updateResult = await _invoiceRepository.Update(data);
            if (updateResult)
            {
                if (data.Items is not null && InvoiceStatusEnum.PAID.ToString().Equals(data.Status))
                {
                    await _itemService.UpdateQuantity(data.Items.Where(x => x.ItemId != Guid.Empty));
                }
                await _notificationService.Insert(context, NotificationTypeEnum.UPDATEINVOICE, new Invoice
                {
                    CustomerType = data.CustomerType,
                    CustomerName = data.CustomerName,
                    CustomerPhone = data.CustomerPhone,
                    Total = data.Total
                });

                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.UPDATE,
                        Entity = EntityEnum.INVOICE.ToString(),
                        Data = new HistoryLogDescription
                        {
                            ModelId = data.Id,
                            OldData = JsonConvert.SerializeObject(oldInvoice),
                            NewData = JsonConvert.SerializeObject(data)
                        }
                    }, context);
            }


            return new ServiceResultModel
            {
                Success = true,
                Data = updateResult
            };
        }
    }
}
