using BPHN.BusinessLayer.IServices;
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
        public PermissionService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IHistoryLogService historyLogService) : base(serviceProvider, appSettings)
        {
            _historyLogService = historyLogService;
        }

        public List<Permission> GetDefaultPermissions(Guid accountId, Account context)
        {
            return new List<Permission>()
            {
                new Permission()
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    FunctionType = (int)FunctionTypeEnum.ADD_PITCH,
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
                    FunctionType = (int)FunctionTypeEnum.EDIT_PITCH,
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
                    FunctionType = (int)FunctionTypeEnum.VIEW_LIST_PITCH,
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
                    FunctionType = (int)FunctionTypeEnum.ADD_BOOKING,
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
                    FunctionType = (int)FunctionTypeEnum.EDIT_BOOKING,
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
                    FunctionType = (int)FunctionTypeEnum.VIEW_LIST_BOOKING,
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
                    FunctionType = (int)FunctionTypeEnum.ADD_USER,
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
                    FunctionType = (int)FunctionTypeEnum.EDIT_USER,
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
                    FunctionType = (int)FunctionTypeEnum.VIEW_LIST_USER,
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
                    FunctionType = (int)FunctionTypeEnum.VIEW_LIST_BOOKING_DETAIL,
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
                    Message = "Token đã hết hạn"
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
                    Message = "Token đã hết hạn"
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.EDIT_USER);
            if(!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
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
                var key = _cacheService.GetKeyCache(accountId.ToString(), "Permission");
                await _cacheService.RemoveAsync(key);
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
                        Entity = "Phân quyền",
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
