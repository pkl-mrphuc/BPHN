using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class PermissionService : BaseService, IPermissionService
    {
        private readonly IHistoryLogService _historyLogService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionRepository _permissionRepository;
        public PermissionService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            INotificationService notificationService,
            IHistoryLogService historyLogService,
            IPermissionRepository permissionRepository) : base(serviceProvider, appSettings)
        {
            _historyLogService = historyLogService;
            _notificationService = notificationService;
            _permissionRepository = permissionRepository;
        }

        public async Task<ServiceResultModel> GetPermissions(Guid accountId)
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

            var permissions = await _permissionRepository.GetPermissions(accountId);
            if (permissions.IsNullOrEmpty())
            {
                permissions = GetDefaultPermissions(accountId, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<PermissionRespond>>(permissions)
            };
        }

        public async Task<ServiceResultModel> SavePermissions(Guid accountId, IEnumerable<Permission> permissions)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.EDITUSER);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            permissions = permissions.Select(item =>
            {
                item.ModifiedDate = DateTime.Now;
                item.ModifiedBy = context.FullName;
                item.CreatedBy = context.FullName;
                item.CreatedDate = DateTime.Now;
                item.AccountId = accountId;
                item.Id = item.Id.Equals(Guid.Empty) ? Guid.NewGuid() : item.Id;
                return item;
            });

            var oldPermissions = await _permissionRepository.GetPermissions(accountId);
            var saveResult = await _permissionRepository.Save(permissions);
            if (saveResult)
            {
                var account = new Account() { UserName = string.Empty };
                await _notificationService.Insert<Account>(context, NotificationTypeEnum.CHANGEPERMISSION, account);

                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.SAVE,
                        Entity = EntityEnum.PERMISSION.ToString(),
                        Data = new HistoryLogDescription
                        {
                            ModelId = context.Id,
                            OldData = JsonConvert.SerializeObject(oldPermissions),
                            NewData = JsonConvert.SerializeObject(permissions)
                        }
                    }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = saveResult
            };
        }

        public async Task<IEnumerable<Permission>> GetAll(Guid accountId)
        {
            return await _permissionRepository.GetPermissions(accountId);
        }

        public IEnumerable<Permission> GetDefaultPermissions(Guid accountId, Account context)
        {
            return new List<Permission>
            {
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.ADDPITCH,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.EDITPITCH,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.VIEWLISTPITCH,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.ADDBOOKING,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.EDITBOOKING,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.VIEWLISTBOOKING,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.VIEWLISTBOOKINGDETAIL,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.VIEWLISTBOOKINGDETAIL,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.ADDUSER,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.EDITUSER,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.ADDINVOICE,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.EDITINVOICE,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.VIEWLISTINVOICE,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.ADDSERVICE,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.EDITSERVICE,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.VIEWLISTSERVICE,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
            };
        }

        public async Task<bool> IsValidPermission(Guid accountId, FunctionTypeEnum functionType)
        {
            var permissions = await GetAll(accountId);
            if (permissions is null)
            {
                return false;
            }

            var result = permissions.FirstOrDefault(item => item.FunctionType == (int)functionType && item.Allow) is not null;
            if (!result)
            {
                var context = _contextService.GetContext();
                if (context?.Id == accountId && context?.Role == RoleEnum.ADMIN && (functionType == FunctionTypeEnum.VIEWLISTUSER || functionType == FunctionTypeEnum.ADDUSER || functionType == FunctionTypeEnum.EDITUSER))
                {
                    return true;
                }
            }

            return result;
        }
    }
}
