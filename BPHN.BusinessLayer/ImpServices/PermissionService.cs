using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Principal;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BPHN.BusinessLayer.ImpServices
{
    public class PermissionService : BaseService, IPermissionService
    {
        private readonly IHistoryLogService _historyLogService;
        private readonly INotificationService _notificationService;
        private readonly IAccountRepository _accountRepository;
        public PermissionService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            INotificationService notificationService,
            IHistoryLogService historyLogService,
            IAccountRepository accountRepository) : base(serviceProvider, appSettings)
        {
            _historyLogService = historyLogService;
            _notificationService = notificationService;
            _accountRepository = accountRepository;
        }

        public List<Permission> GetDefaultPermissions(Guid accountId, Account context)
        {
            return new List<Permission>()
            {
                new Permission()
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
                new Permission()
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
                new Permission()
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
                new Permission()
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
                new Permission()
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
                new Permission()
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
                new Permission()
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
                new Permission()
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
                new Permission()
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.VIEWLISTUSER,
                    Allow = false,
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                },
                new Permission()
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
            };
        }

        public async Task<ServiceResultModel> GetPermissions(Guid accountId)
        {
            var context = _contextService.GetContext();
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var permissions = await _permissionRepository.GetPermissions(accountId);
            if(permissions == null || permissions.Count == 0)
            {
                permissions = GetDefaultPermissions(accountId, context);
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = permissions
            };
        }

        public async Task<ServiceResultModel> SavePermissions(Guid accountId, List<Permission> permissions)
        {
            var context = _contextService.GetContext();
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.EDITUSER);
            if(!hasPermission)
            {
                return new ServiceResultModel()
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
            }).ToList();

            var oldPermissions = await _permissionRepository.GetPermissions(accountId);
            var saveResult = await _permissionRepository.Save(permissions);
            if(saveResult)
            {
                await _notificationService.Insert<Account>(context, NotificationTypeEnum.CHANGEPERMISSION, new Account()
                {
                    UserName = (await _accountRepository.GetAccountById(accountId))?.UserName ?? string.Empty
                });
                await _cacheService.RemoveAsync(_cacheService.GetKeyCache(accountId, EntityEnum.PERMISSION));
                var thread = new Thread(delegate ()
                {
                    var historyLogId = Guid.NewGuid();
                    _historyLogService.Write(new HistoryLog()
                    {
                        Id = historyLogId,
                        IPAddress = context.IPAddress,
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.SAVE,
                        Entity = EntityEnum.PERMISSION.ToString(),
                        ActionName = string.Empty,
                        Description = BuildLinkDescription(historyLogId),
                        Data = new HistoryLogDescription()
                        {
                            ModelId = context.Id,
                            OldData = JsonConvert.SerializeObject(oldPermissions),
                            NewData = JsonConvert.SerializeObject(permissions)
                        }
                    }, context);

                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = saveResult
            };
        }
    }
}
