using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BPHN.BusinessLayer.ImpServices
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _mailService;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IHistoryLogService _historyLogService;
        private readonly IConfigService _configService;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;
        private readonly IPermissionService _permissionService;
        private readonly ILicenseService _licenseService;
        public AccountService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IAccountRepository accountRepository,
            IEmailService mailService,
            IKeyGenerator keyGenerator,
            IHistoryLogService historyLogService,
            IConfigService configService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IFileService fileService,
            ILicenseService licenseService) : base(serviceProvider, appSettings)
        {
            _accountRepository = accountRepository;
            _mailService = mailService;
            _keyGenerator = keyGenerator;
            _historyLogService = historyLogService;
            _configService = configService;
            _notificationService = notificationService;
            _fileService = fileService;
            _permissionService = permissionService;
            _licenseService = licenseService;
        }

        public async Task<ServiceResultModel> ChangePassword(Account account)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var isValid = ValidateModelByAttribute(account, "UserName", "PhoneNumber", "FullName", "Email");
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            if (account.Id != context.Id)
            {
                return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig));
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var resultResetPassword = await _accountRepository.SavePassword(account.Id, passwordHash);
            if (resultResetPassword)
            {
                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.SUBMITRESETPASSWORD,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = resultResetPassword
            };
        }

        public async Task<ServiceResultModel> GetById(Guid id)
        {
            var account = await _accountRepository.GetAccountById(id);
            if (account is not null)
            {
                account.RelationIds = new Guid[] { id };
                account.LanguageConfig = await _configService.Language(id);
            }
            return new ServiceResultModel
            {
                Success = true,
                Data = account
            };
        }

        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.VIEWLISTUSER);
            if (!hasPermission || (context.Role != RoleEnum.ADMIN && !await _configService.AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var where = new List<WhereCondition>();
            switch (context.Role)
            {
                case RoleEnum.ADMIN:
                    where.Add(new WhereCondition
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString(), RoleEnum.TENANT.ToString() }
                    });
                    break;
                default:
                    where.Add(new WhereCondition
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString() }
                    });
                    where.Add(new WhereCondition
                    {
                        Column = "Id",
                        Operator = "!=",
                        Value = context.Id
                    });
                    where.Add(new WhereCondition
                    {
                        Column = "ParentId",
                        Operator = "in",
                        Value = context.RelationIds.ToArray()
                    });
                    break;
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = await _accountRepository.GetCountPaging(pageIndex, pageSize, txtSearch, where)
            };
        }

        public async Task<ServiceResultModel> GetInstance(string id)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            if (!string.IsNullOrWhiteSpace(id) && !Guid.TryParse(id, out var accountId))
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            var data = new Account();
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out accountId))
            {
                data = await _accountRepository.GetAccountById(accountId);
                if (data is null)
                {
                    return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig));
                }
                else
                {
                    data.AvatarUrl = _fileService.GetFileUrl(accountId.ToString());
                }
            }
            else
            {
                data.Id = Guid.NewGuid();
                data.Gender = GenderEnum.MALE.ToString();
                data.Status = ActiveStatusEnum.ACTIVE.ToString();
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<AccountRespond>(data)
            };
        }

        public async Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.VIEWLISTUSER);
            if (!hasPermission || (context.Role != RoleEnum.ADMIN && !await _configService.AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var where = new List<WhereCondition>();
            switch (context.Role)
            {
                case RoleEnum.ADMIN:
                    where.Add(new WhereCondition
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString(), RoleEnum.TENANT.ToString() }
                    });
                    break;
                default:
                    where.Add(new WhereCondition
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString() }
                    });
                    where.Add(new WhereCondition
                    {
                        Column = "Id",
                        Operator = "!=",
                        Value = context.Id
                    });
                    where.Add(new WhereCondition
                    {
                        Column = "ParentId",
                        Operator = "in",
                        Value = context.RelationIds.ToArray()
                    });
                    break;
            }

            var lstTenants = await _accountRepository.GetPaging(pageIndex, pageSize, txtSearch, where);

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<AccountRespond>>(lstTenants)
            };
        }

        public async Task<ServiceResultModel> Login(Account account)
        {
            var isValid = ValidateModelByAttribute(account, "Id", "PhoneNumber", "FullName", "Email");
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT));
            }

            var realAccount = await _accountRepository.GetAccountByUserName(account.UserName);
            if (realAccount is null)
            {
                return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.LOGINFAIL));
            }

            if (!ActiveStatusEnum.ACTIVE.ToString().Equals(realAccount.Status))
            {
                return new ServiceResultModel(ErrorCodes.INACTIVE_DATA, _resourceService.Get(SharedResourceKey.INACTIVESTATUS));
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(account.Password, realAccount.Password))
                {
                    return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.LOGINFAIL));
                }
            }
            catch (Exception)
            {
                return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.LOGINFAIL));
            }

            var (token, refreshToken) = _accountRepository.GetToken(realAccount.Id);
            await _accountRepository.SaveToken(realAccount.Id, token, refreshToken);

            _historyLogService.Write(Guid.NewGuid(),
                new HistoryLog
                {
                    Actor = realAccount.UserName,
                    ActorId = realAccount.Id,
                    ActionType = ActionEnum.LOGIN,
                    Entity = EntityEnum.ACCOUNT.ToString()
                },
                new Account
                {
                    FullName = realAccount.FullName,
                    IPAddress = _contextService.GetIPAddress()
                });

            return new ServiceResultModel
            {
                Success = true,
                Data = new LoginRespond
                {
                    Id = realAccount.Id,
                    FullName = realAccount.FullName,
                    UserName = realAccount.UserName,
                    PhoneNumber = realAccount.PhoneNumber,
                    Email = realAccount.Email,
                    Gender = realAccount.Gender,
                    Role = realAccount.Role,
                    ParentId = realAccount.ParentId,
                    Token = token,
                    RefreshToken = refreshToken,
                    RelationIds = new Guid[] { realAccount.Id },
                    AvatarUrl = _fileService.GetFileUrl(realAccount.Id.ToString())
                }
            };
        }

        public async Task<ServiceResultModel> LoginGoogle(AuthenticateResult authenticateResult)
        {
            var email = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.LOGINFAIL));
            }

            var user = await _accountRepository.GetAccountByUserName(email);
            if (user is null)
            {
                user = new Account
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FullName = email,
                    UserName = email,
                    Gender = GenderEnum.MALE.ToString(),
                    Status = ActiveStatusEnum.ACTIVE.ToString(),
                    CreatedBy = Constansts.SYSTEM,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = Constansts.SYSTEM,
                    ModifiedDate = DateTime.Now,
                    Role = RoleEnum.USER,
                };

                var result = await _accountRepository.RegisterForTenant(user);
                if (!result)
                {
                    return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.LOGINFAIL));
                }
            }

            if (!ActiveStatusEnum.ACTIVE.ToString().Equals(user.Status))
            {
                return new ServiceResultModel(ErrorCodes.INACTIVE_DATA, _resourceService.Get(SharedResourceKey.INACTIVESTATUS));
            }

            var (token, refreshToken) = _accountRepository.GetToken(user.Id);
            await _accountRepository.SaveToken(user.Id, token, refreshToken);

            return new ServiceResultModel
            {
                Success = true,
                Data = new LoginRespond
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Gender = user.Gender,
                    Role = user.Role,
                    Token = token,
                    RefreshToken = refreshToken,
                }
            };
        }

        public async Task<ServiceResultModel> Refresh()
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            return new ServiceResultModel
            {
                Success = true
            };
        }

        public async Task<ServiceResultModel> RefreshToken(string refreshToken)
        {
            var accountId = _accountRepository.ValidateToken(refreshToken, true);
            if (accountId == Guid.Empty)
            {
                return new ServiceResultModel(ErrorCodes.INACTIVE_DATA, _resourceService.Get(SharedResourceKey.INVALIDDATA));
            }

            var (token, newRefreshToken) = _accountRepository.GetToken(accountId);
            await _accountRepository.SaveToken(accountId, token, newRefreshToken);

            return new ServiceResultModel
            {
                Success = true,
                Data = token
            };
        }

        public async Task<ServiceResultModel> RegisterForTenant(Account account)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.ADDUSER);
            if (!hasPermission || (context.Role != RoleEnum.ADMIN && !await _configService.AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            var isValid = ValidateModelByAttribute(account, "Id", "Password");
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            var existUserName = await _accountRepository.CheckExistUserName(account.UserName);
            if (existUserName)
            {
                return new ServiceResultModel(ErrorCodes.EXISTED, _resourceService.Get(SharedResourceKey.EXISTED));
            }

            account.Id = Guid.NewGuid();
            account.CreatedBy = context.FullName;
            account.CreatedDate = DateTime.Now;
            account.ModifiedBy = context.FullName;
            account.ModifiedDate = DateTime.Now;
            account.Role = RoleEnum.TENANT;
            if (context.Role == RoleEnum.TENANT)
            {
                account.ParentId = context.Id;
                account.Role = RoleEnum.USER;
            }
            if (context.Role == RoleEnum.USER)
            {
                account.ParentId = context.ParentId;
                account.Role = RoleEnum.USER;
            }
            account.Permissions = _permissionService.GetDefaultPermissions(account.Id, context);

            var resultRegister = await _accountRepository.RegisterForTenant(account);
            if (resultRegister)
            {
                await _licenseService.Insert(new License
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    Type = account.LicenseType,
                    ExpireTime = DateTime.Now.AddYears(1),
                    CreatedBy = context.FullName,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = context.FullName,
                    ModifiedDate = DateTime.Now
                });

                await _permissionService.SavePermissions(account.Id, account.Permissions);

                await _notificationService.Insert(context, NotificationTypeEnum.INSERTACCOUNT, new Account
                {
                    UserName = account.UserName
                });

                if (ActiveStatusEnum.ACTIVE.ToString().Equals(account.Status))
                {
                    _mailService.SendMail("bphn.email.set-password",
                        new SetPasswordParameter
                        {
                            ReceiverAddress = account.Email,
                            AccountId = account.Id,
                            FullName = account.FullName,
                            UserName = account.UserName,
                            MailType = MailTypeEnum.SETPASSWORD,
                            ParameterType = typeof(SetPasswordParameter)
                        });
                }

                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.REGISTERACCOUNT,
                        Description = account.UserName,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = resultRegister
            };
        }

        public async Task<ServiceResultModel> Update(Account account)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var hasPermission = await _permissionService.IsValidPermissions(context.Id, FunctionTypeEnum.EDITUSER);
            if (!hasPermission || (context.Role != RoleEnum.ADMIN && !await _configService.AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel(ErrorCodes.INVALID_ROLE, _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig));
            }

            var isValid = ValidateModelByAttribute(account, "Email", "UserName", "Password");
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig));
            }

            account.ModifiedBy = context.FullName;
            account.ModifiedDate = DateTime.Now;

            var result = await _accountRepository.UpdateTenant(account);
            if (result)
            {
                await _licenseService.Update(new License
                {
                    AccountId = account.Id,
                    Type = account.LicenseType,
                    ExpireTime = DateTime.Now.AddYears(1),
                    ModifiedBy = context.ModifiedBy,
                    ModifiedDate = DateTime.Now,
                });

                await _notificationService.Insert(context, NotificationTypeEnum.UPDATEACCOUNT, new Account
                {
                    UserName = account.UserName
                });

                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.UPDATE,
                        Description = account.UserName,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = result
            };
        }

        public async Task<ServiceResultModel> ResetPassword(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT));
            }

            var realAccount = await _accountRepository.GetAccountByUserName(userName);
            if (realAccount is null)
            {
                return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.NOTEXIST));
            }

            if (!ActiveStatusEnum.ACTIVE.ToString().Equals(realAccount.Status))
            {
                return new ServiceResultModel(ErrorCodes.INACTIVE_DATA, _resourceService.Get(SharedResourceKey.INACTIVESTATUS));
            }

            var resultSendMail = _mailService.SendMail("bphn.email.forgot-password",
                new SetPasswordParameter
                {
                    ReceiverAddress = realAccount.Email,
                    AccountId = realAccount.Id,
                    FullName = realAccount.FullName,
                    UserName = realAccount.UserName,
                    MailType = MailTypeEnum.FORTGOTPASSWORD,
                    ParameterType = typeof(SetPasswordParameter)
                });

            if (resultSendMail)
            {
                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id,
                        ActionType = ActionEnum.SENDRESETPASSWORD,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    },
                    new Account()
                    {
                        FullName = realAccount.FullName,
                        IPAddress = _contextService.GetIPAddress()
                    });
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = resultSendMail
            };
        }

        public async Task<ServiceResultModel> SubmitSetPassword(string code, string password, string userName)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(userName))
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT));
            }

            var parameter = _keyGenerator.Decryption(code);
            var expireResetPasswordModel = JsonConvert.DeserializeObject<ExpireSetPasswordModel>(parameter);
            if (expireResetPasswordModel is null)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT));
            }

            if (expireResetPasswordModel.ExpireTime < DateTime.Now)
            {
                return new ServiceResultModel(ErrorCodes.OUT_TIME, _resourceService.Get(SharedResourceKey.OUTTIME));
            }

            var account = new Account
            {
                Id = Guid.Parse(expireResetPasswordModel.AccountId),
                Password = password,
            };

            var isValid = ValidateModelByAttribute(account, "UserName", "PhoneNumber", "FullName", "Email");
            if (!isValid)
            {
                return new ServiceResultModel(ErrorCodes.EMPTY_INPUT, _resourceService.Get(SharedResourceKey.EMPTYINPUT));
            }

            var realAccount = await _accountRepository.GetAccountById(account.Id);
            if (realAccount is null)
            {
                return new ServiceResultModel(ErrorCodes.NOT_EXISTS, _resourceService.Get(SharedResourceKey.NOTEXIST));
            }

            if (realAccount.UserName != userName)
            {
                return new ServiceResultModel(ErrorCodes.NO_INTEGRITY, _resourceService.Get(SharedResourceKey.NOTEXIST));
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var resultResetPassword = await _accountRepository.SavePassword(account.Id, passwordHash);
            if (resultResetPassword)
            {
                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id,
                        ActionType = ActionEnum.SUBMITRESETPASSWORD,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    },
                    new Account
                    {
                        FullName = realAccount.FullName,
                        IPAddress = _contextService.GetIPAddress()
                    });
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = resultResetPassword
            };
        }

        public ServiceResultModel ValidateToken(string token)
        {
            var accountId = _accountRepository.ValidateToken(token, false);
            return new ServiceResultModel
            {
                Success = accountId != Guid.Empty,
                Data = accountId
            };
        }

        public async Task<IEnumerable<Guid>> GetRelationIds(Guid id)
        {
            return await _accountRepository.GetRelationIds(id);
        }
    }
}
