﻿using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class ItemService : BaseService, IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IHistoryLogService _historyLogService;
        public ItemService(
            IServiceProvider provider,
            IOptions<AppSettings> appSettings,
            IItemRepository itemRepository,
            IPermissionService permissionService,
            INotificationService notificationService,
            IHistoryLogService historyLogService) : base(provider, appSettings)
        {
            _itemRepository = itemRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _historyLogService = historyLogService;
        }

        public async Task<ServiceResultModel> GetInstance(string id)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            if (!string.IsNullOrWhiteSpace(id) && !Guid.TryParse(id, out var itemId))
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            var data = new Item();
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out itemId))
            {
                data = await _itemRepository.GetById(itemId);
                if (data is null)
                {
                    return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig));
                }
            }
            else
            {
                data.Id = Guid.NewGuid();
                data.Status = ActiveStatusEnum.ACTIVE.ToString();
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<ItemRespond>(data)
            };
        }

        public async Task<ServiceResultModel> GetItems(string txtSearch, string status, string code, string unit, string quantity)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.VIEWLISTSERVICE);
            if (!hasPermission)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            IEnumerable<Item> lstItem;
            if (!string.IsNullOrWhiteSpace(txtSearch) ||
                !string.IsNullOrWhiteSpace(status) ||
                !string.IsNullOrWhiteSpace(code) ||
                !string.IsNullOrWhiteSpace(unit) ||
                !string.IsNullOrWhiteSpace(quantity))
            {
                lstItem = await _itemRepository.GetItems(context.Id, txtSearch, status, code, unit, quantity);
            }
            else
            {
                lstItem = await _itemRepository.GetAll(context.Id);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<ItemRespond>>(lstItem)
            };
        }

        public async Task<ServiceResultModel> Insert(Item data)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.ADDSERVICE);
            if (!hasPermission)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            var isValid = ValidateModelByAttribute(data, "Id");
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            data.Id = Guid.NewGuid();
            data.AccountId = context.ParentId ?? context.Id;
            data.CreatedBy = context.FullName;
            data.CreatedDate = DateTime.Now;
            data.ModifiedBy = context.FullName;
            data.ModifiedDate = DateTime.Now;

            var insertResult = await _itemRepository.Insert(data);
            if (insertResult)
            {
                await _notificationService.Insert(context, NotificationTypeEnum.INSERTSERVICE, new Item
                {
                    Name = data.Name
                });

                _historyLogService.Write(Guid.NewGuid(), new HistoryLog
                {
                    ActionType = ActionEnum.INSERT,
                    Entity = EntityEnum.SERVICE.ToString(),
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

        public async Task<ServiceResultModel> Update(Item data)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.EDITSERVICE);
            if (!hasPermission)
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            var isValid = ValidateModelByAttribute(data);
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            data.ModifiedBy = context.FullName;
            data.ModifiedDate = DateTime.Now;

            var oldItem = await _itemRepository.GetById(data.Id);
            var updateResult = await _itemRepository.Update(data);
            if (updateResult)
            {
                await _notificationService.Insert(context, NotificationTypeEnum.UPDATESERVICE, new Item
                {
                    Name = data.Name
                });

                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.UPDATE,
                        Entity = EntityEnum.SERVICE.ToString(),
                        Data = new HistoryLogDescription
                        {
                            ModelId = data.Id,
                            OldData = JsonConvert.SerializeObject(oldItem),
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

        public async Task UpdateQuantity(IEnumerable<InvoiceItem> items)
        {
            await _itemRepository.UpdateQuantity(items);
        }

        public async Task<bool> CheckQuantityInStock(IEnumerable<InvoiceItem> items)
        {
            return await _itemRepository.CheckQuantityInStock(items);
        }
    }
}
