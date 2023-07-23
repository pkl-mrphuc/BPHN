using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BPHN.BusinessLayer.ImpServices
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _mailService;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IHistoryLogService _historyLogService;
        private readonly IConfigRepository _configRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        public AccountService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IAccountRepository accountRepository,
            IEmailService mailService,
            IKeyGenerator keyGenerator,
            IHistoryLogService historyLogService,
            IConfigRepository configRepository,
            IPermissionService permissionService,
            INotificationService notificationService) : base(serviceProvider, appSettings)
        {
            _accountRepository = accountRepository;
            _mailService = mailService;
            _keyGenerator = keyGenerator;
            _historyLogService = historyLogService;
            _configRepository = configRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }

        public async Task<ServiceResultModel> ChangePassword(Account account)
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

            var isValid = ValidateModelByAttribute(account, new List<string>() { "UserName", "PhoneNumber", "FullName", "Email" });
            if (!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
                };
            }

            if (account.Id != context.Id)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig)
                };
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var resultResetPassword = await _accountRepository.SavePassword(account.Id, passwordHash);

            if (resultResetPassword)
            {
                var thread = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        IPAddress = context.IPAddress,
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.SUBMITRESETPASSWORD,
                        ActionName = string.Empty,
                        Description = string.Empty,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    }, context);
                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultResetPassword
            };
        }

        public async Task<ServiceResultModel> GetById(Guid id)
        {
            Account? account = null;
            var cacheResult = await _cacheService.GetAsync(_cacheService.GetKeyCache(id, EntityEnum.ACCOUNT));
            if(!string.IsNullOrWhiteSpace(cacheResult))
            {
                account = JsonConvert.DeserializeObject<Account>(cacheResult);
            }
            if(account == null)
            {
                account = await _accountRepository.GetAccountById(id);
                if(account != null)
                {
                    await _cacheService.SetAsync(_cacheService.GetKeyCache(id, EntityEnum.ACCOUNT), JsonConvert.SerializeObject(account));
                }
            }
            
            if (account != null)
            {
                List<Guid>? lstRelationId = null;
                cacheResult = await _cacheService.GetAsync(_cacheService.GetKeyCache(id, EntityEnum.ACCOUNT, "RelationId"));
                if(!string.IsNullOrWhiteSpace(cacheResult))
                {
                    lstRelationId = JsonConvert.DeserializeObject<List<Guid>>(cacheResult);
                }

                if(lstRelationId == null)
                {
                    lstRelationId = await _accountRepository.GetRelationIds(id);
                    if(lstRelationId != null)
                    {
                        await _cacheService.SetAsync(_cacheService.GetKeyCache(id, EntityEnum.ACCOUNT, "RelationId"), JsonConvert.SerializeObject(lstRelationId));
                    }
                }
                account.RelationIds = lstRelationId == null ||  lstRelationId.Count == 0 ? new List<Guid>() { id } :  lstRelationId;

                List<Config>? lstConfig = null;
                cacheResult = await _cacheService.GetAsync(_cacheService.GetKeyCache(id, EntityEnum.CONFIG));
                if(!string.IsNullOrWhiteSpace(cacheResult))
                {
                    lstConfig = JsonConvert.DeserializeObject<List<Config>>(cacheResult);
                }

                if(lstConfig == null)
                {
                    lstConfig = await _configRepository.GetConfigs(id, "Language");
                }
                account.LanguageConfig = lstConfig?.Where(item => item.Key == "Language").FirstOrDefault()?.Value ?? string.Empty;
            }
            return new ServiceResultModel()
            {
                Success = true,
                Data = account
            };
        }

        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTUSER);
            if (!hasPermission ||
                (context.Role != RoleEnum.ADMIN && !await AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var where = new List<WhereCondition>();
            switch (context.Role)
            {
                case RoleEnum.ADMIN:
                    where.Add(new WhereCondition()
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString(), RoleEnum.TENANT.ToString() }
                    });
                    break;
                default:
                    where.Add(new WhereCondition()
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString() }
                    });
                    where.Add(new WhereCondition()
                    {
                        Column = "Id",
                        Operator = "!=",
                        Value = context.Id
                    });
                    where.Add(new WhereCondition()
                    {
                        Column = "ParentId",
                        Operator = "in",
                        Value = context.RelationIds.ToArray()
                    });
                    break;
            }

            var resultCountPaging = await _accountRepository.GetCountPaging(pageIndex, pageSize, txtSearch, where);

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultCountPaging
            };
        }

        public async Task<ServiceResultModel> GetInstance(string id)
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

            Account? data = null;
            if (string.IsNullOrWhiteSpace(id))
            {
                data = new Account();
                data.Id = Guid.NewGuid();
                data.Gender = GenderEnum.MALE.ToString();
                data.Status = ActiveStatusEnum.ACTIVE.ToString();
            }
            else
            {
                Guid accountId;
                var success = Guid.TryParse(id, out accountId);
                if (success)
                {
                    var cacheResult = await _cacheService.GetAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.ACCOUNT, accountId.ToString()));
                    if (!string.IsNullOrWhiteSpace(cacheResult))
                    {
                        data = JsonConvert.DeserializeObject<Account>(cacheResult);
                    }

                    if (data == null)
                    {
                        data = await _accountRepository.GetAccountById(accountId);
                        if (data != null)
                        {
                            await _cacheService.SetAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.ACCOUNT, accountId.ToString()), JsonConvert.SerializeObject(data));
                        }
                    }
                }
                else
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.EMPTY_INPUT,
                        Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
                    };
                }

                if (data == null)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig)
                    };
                }
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = data
            };
        }

        public async Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTUSER);
            if (!hasPermission ||
                (context.Role != RoleEnum.ADMIN && !await AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var where = new List<WhereCondition>();
            switch (context.Role)
            {
                case RoleEnum.ADMIN:
                    where.Add(new WhereCondition()
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString(), RoleEnum.TENANT.ToString() }
                    });
                    break;
                default:
                    where.Add(new WhereCondition()
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString() }
                    });
                    where.Add(new WhereCondition()
                    {
                        Column = "Id",
                        Operator = "!=",
                        Value = context.Id
                    });
                    where.Add(new WhereCondition()
                    {
                        Column = "ParentId",
                        Operator = "in",
                        Value = context.RelationIds.ToArray()
                    });
                    break;
            }

            var lstTenants = await _accountRepository.GetPaging(pageIndex, pageSize, txtSearch, where);

            return new ServiceResultModel()
            {
                Success = true,
                Data = lstTenants
            };
        }

        public ServiceResultModel GetTokenInfo(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken.ValidTo < DateTime.Now)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = jwtToken
            };
        }

        public async Task<ServiceResultModel> Login(Account account)
        {
            var isValid = ValidateModelByAttribute(account, new List<string>() { "Id", "PhoneNumber", "FullName", "Email" });
            if (!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var realAccount = await _accountRepository.GetAccountByUserName(account.UserName);

            if (realAccount == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.LOGINFAIL)
                };
            }

            if (!realAccount.Status.Equals(ActiveStatusEnum.ACTIVE.ToString()))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INACTIVE_DATA,
                    Message = _resourceService.Get(SharedResourceKey.INACTIVESTATUS)
                };
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(account.Password, realAccount.Password))
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = _resourceService.Get(SharedResourceKey.LOGINFAIL)
                    };
                }
            }
            catch (Exception)
            {

                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.LOGINFAIL)
                };
            }


            string token = _accountRepository.GetToken(realAccount.Id.ToString());

            var fakeContext = new Account()
            {
                FullName = realAccount.FullName,
                IPAddress = _contextService.GetIPAddress()
            };

            var thread = new Thread(delegate ()
            {
                _historyLogService.Write(new HistoryLog()
                {
                    IPAddress = fakeContext.IPAddress,
                    Actor = realAccount.UserName,
                    ActorId = realAccount.Id,
                    ActionType = ActionEnum.LOGIN,
                    ActionName = string.Empty,
                    Entity = EntityEnum.ACCOUNT.ToString()
                }, fakeContext);
            });
            thread.Start();

            return new ServiceResultModel()
            {
                Success = true,
                Data = new Account()
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
                    RelationIds = await _accountRepository.GetRelationIds(realAccount.Id)
                }
            };
        }

        public async Task<ServiceResultModel> Refresh()
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

            await _cacheService.RemoveAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.ACCOUNT));
            await _cacheService.RemoveAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.ACCOUNT, "RelationId"));
            await _cacheService.RemoveAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.CONFIG));
            await _cacheService.RemoveAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.PERMISSION));
            for (int i = 0; i < context.RelationIds.Count; i++)
            {
                await _cacheService.RemoveAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.ACCOUNT, context.RelationIds[i].ToString()));
            }

            return new ServiceResultModel()
            {
                Success = true
            };
        }

        public async Task<ServiceResultModel> RegisterForTenant(Account account)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.ADDUSER);
            if (!hasPermission ||
                (context.Role != RoleEnum.ADMIN && !await AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var isValid = ValidateModelByAttribute(account, new List<string>() { "Id", "Password" });
            if (!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
                };
            }

            var existUserName = await _accountRepository.CheckExistUserName(account.UserName);
            if (existUserName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EXISTED,
                    Message = _resourceService.Get(SharedResourceKey.EXISTED)
                };
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

            var resultRegister = await _accountRepository.RegisterForTenant(account);
            var permissions = _permissionService.GetDefaultPermissions(account.Id, context);
            var resultPermission = await _permissionRepository.Save(permissions);
            if (resultRegister)
            {
                await _notificationService.Insert<Account>(context, NotificationTypeEnum.INSERTACCOUNT, new Account()
                {
                    UserName = account.UserName
                });
                for (int i = 0; i < context.RelationIds.Count; i++)
                {
                     await _cacheService.RemoveAsync(_cacheService.GetKeyCache(context.RelationIds[i], EntityEnum.ACCOUNT, "RelationId"));
                }
                if (account.Status.Equals(ActiveStatusEnum.ACTIVE.ToString()))
                {
                    var thread = new Thread(() =>
                    {
                        _mailService.SendMail(new ObjectQueue()
                        {
                            QueueJobType = QueueJobTypeEnum.SENDMAIL,
                            DataJson = JsonConvert.SerializeObject(new SetPasswordParameter()
                            {
                                ReceiverAddress = account.Email,
                                AccountId = account.Id,
                                FullName = account.FullName,
                                UserName = account.UserName,
                                MailType = MailTypeEnum.SETPASSWORD,
                                ParameterType = typeof(SetPasswordParameter)
                            })
                        });
                    });
                    thread.Start();
                }

                var threadLog = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        IPAddress = context.IPAddress,
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.REGISTERACCOUNT,
                        ActionName = string.Empty,
                        Description = account.UserName,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    }, context);
                });
                threadLog.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultRegister
            };
        }

        public async Task<ServiceResultModel> ResetPassword(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var realAccount = await _accountRepository.GetAccountByUserName(userName);
            if (realAccount == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                };
            }

            if (!realAccount.Status.Equals(ActiveStatusEnum.ACTIVE.ToString()))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INACTIVE_DATA,
                    Message = _resourceService.Get(SharedResourceKey.INACTIVESTATUS)
                };
            }

            var resultSendMail = _mailService.SendMail(new ObjectQueue()
            {
                QueueJobType = QueueJobTypeEnum.SENDMAIL,
                DataJson = JsonConvert.SerializeObject(new SetPasswordParameter()
                {
                    ReceiverAddress = realAccount.Email,
                    AccountId = realAccount.Id,
                    FullName = realAccount.FullName,
                    UserName = realAccount.UserName,
                    MailType = MailTypeEnum.FORTGOTPASSWORD,
                    ParameterType = typeof(SetPasswordParameter)
                })
            });

            if (resultSendMail)
            {
                var fakeContext = new Account()
                {
                    FullName = realAccount.FullName,
                    IPAddress = _contextService.GetIPAddress()
                };

                var thread = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        IPAddress = fakeContext.IPAddress,
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id,
                        ActionType = ActionEnum.SENDRESETPASSWORD,
                        ActionName = string.Empty,
                        Description = string.Empty,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    }, fakeContext);
                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultSendMail
            };
        }

        public async Task<ServiceResultModel> SubmitSetPassword(string code, string password, string userName)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(userName))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var param = _keyGenerator.Decryption(code);
            var expireResetPasswordModel = JsonConvert.DeserializeObject<ExpireSetPasswordModel>(param);

            if (expireResetPasswordModel == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            if (expireResetPasswordModel.ExpireTime < DateTime.Now)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var account = new Account()
            {
                Id = Guid.Parse(expireResetPasswordModel.AccountId),
                Password = password,
            };

            var isValid = ValidateModelByAttribute(account, new List<string>() { "UserName", "PhoneNumber", "FullName", "Email" });
            if (!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var realAccount = await _accountRepository.GetAccountById(account.Id);
            if (realAccount == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                };
            }

            if (realAccount.UserName != userName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NO_INTEGRITY,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                };
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var resultResetPassword = await _accountRepository.SavePassword(account.Id, passwordHash);

            if (resultResetPassword)
            {
                var fakeContext = new Account()
                {
                    FullName = realAccount.FullName,
                    IPAddress = _contextService.GetIPAddress()
                };

                var thread = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        IPAddress = fakeContext.IPAddress,
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id,
                        ActionType = ActionEnum.SUBMITRESETPASSWORD,
                        ActionName = string.Empty,
                        Description = string.Empty,
                        Entity = EntityEnum.ACCOUNT.ToString()
                    }, fakeContext);
                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultResetPassword
            };
        }

        public ServiceResultModel ValidateToken(string token)
        {
            var result = GetTokenInfo(token);
            return new ServiceResultModel()
            {
                Success = result.Success
            };
        }

        private async Task<bool> AllowMultiUser(Guid accountId)
        {
            var configs = await _configRepository.GetConfigs(accountId, "MultiUser");
            return configs != null && configs.Any(item => item.Value == "true") ? true : false;
        }

    }
}
