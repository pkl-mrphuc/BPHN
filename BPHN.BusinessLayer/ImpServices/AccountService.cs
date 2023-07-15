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
        
        public AccountService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IAccountRepository accountRepository,
            IEmailService mailService,
            IKeyGenerator keyGenerator,
            IHistoryLogService historyLogService,
            IConfigRepository configRepository,
            IPermissionService permissionService) : base(serviceProvider, appSettings)
        {
            _accountRepository = accountRepository;
            _mailService = mailService;
            _keyGenerator = keyGenerator;
            _historyLogService = historyLogService;
            _configRepository = configRepository;
            _permissionService = permissionService;
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
                    Message = "Token đã hết hạn"
                };
            }

            var isValid = ValidateModelByAttribute(account, new List<string>() { "UserName", "PhoneNumber", "FullName", "Email" });
            if (!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            if (account.Id != context.Id)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản không tồn tại"
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
                        ActionType = ActionEnum.SUBMIT_RESET_PASSWORD,
                        ActionName = string.Empty,
                        Description = string.Empty,
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
            var account = await _accountRepository.GetAccountById(id);
            if(account != null)
            {
                account.Permissions = await _permissionRepository.GetPermissions(id);
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
                    Message = "Token đã hết hạn"
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEW_LIST_USER);
            if( !hasPermission ||
                (context.Role != RoleEnum.ADMIN && !await AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
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
                case RoleEnum.TENANT:
                    where.Add(new WhereCondition()
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString() }
                    });
                    where.Add(new WhereCondition()
                    {
                        Column = "ParentId",
                        Operator = "=",
                        Value = context.Id.ToString()
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
            var data = new Account();
            if (string.IsNullOrEmpty(id))
            {
                data.Id = Guid.NewGuid();
                data.Gender = GenderEnum.MALE.ToString();
                data.Status = ActiveStatusEnum.ACTIVE.ToString();
            }
            else
            {
                Guid accountId;
                var success = Guid.TryParse(id, out accountId);
                if(success)
                {
                    data = await _accountRepository.GetAccountById(accountId);
                }
                else
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.EMPTY_INPUT,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                if (data == null)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = "Không lấy được thông tin tài khoản. Vui lòng kiểm tra lại"
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
            if(context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEW_LIST_USER);
            if(!hasPermission ||
                (context.Role != RoleEnum.ADMIN && !await AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
                };
            }

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var where = new List<WhereCondition>();
            switch(context.Role)
            {
                case RoleEnum.ADMIN:
                    where.Add(new WhereCondition()
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString(), RoleEnum.TENANT.ToString() }
                    });
                    break;
                case RoleEnum.TENANT:
                    where.Add(new WhereCondition()
                    {
                        Column = "Role",
                        Operator = "in",
                        Value = new[] { RoleEnum.USER.ToString() }
                    });
                    where.Add(new WhereCondition()
                    {
                        Column = "ParentId",
                        Operator = "=",
                        Value = context.Id.ToString()
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
            if (string.IsNullOrEmpty(token))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
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
                    Message = "Token đã hết hạn"
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
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var realAccount = await _accountRepository.GetAccountByUserName(account.UserName);

            if (realAccount == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }

            if (!realAccount.Status.Equals(ActiveStatusEnum.ACTIVE.ToString()))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INACTIVE_DATA,
                    Message = "Tài khoản chưa được kích hoạt"
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
                        Message = "Tài khoản hoặc mật khẩu không đúng"
                    };
                }
            }
            catch (Exception)
            {

                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }
            

            string token = _accountRepository.GetToken(realAccount.Id.ToString());

            var fakeContext = new Account()
            {
                FullName = realAccount.FullName,
                IPAddress = _contextService.GetIPAddress()
            };

            var thread = new Thread(delegate()
            {
                _historyLogService.Write(new HistoryLog()
                {
                    IPAddress = fakeContext.IPAddress,
                    Actor = realAccount.UserName,
                    ActorId = realAccount.Id,
                    ActionType = ActionEnum.LOGIN,
                    ActionName = string.Empty
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
                    Token = token
                }
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
                    Message = "Token đã hết hạn"
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.ADD_USER);
            if (!hasPermission ||
                (context.Role != RoleEnum.ADMIN && !await AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
                };
            }

            var isValid = ValidateModelByAttribute(account, new List<string>() { "Id", "Password" });
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var existUserName = await _accountRepository.CheckExistUserName(account.UserName);
            if(existUserName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EXISTED,
                    Message = "Tên tài khoản đã được đăng ký"
                };
            }

            account.Id = Guid.NewGuid();
            account.CreatedBy = context.FullName;
            account.CreatedDate = DateTime.Now;
            account.ModifiedBy = context.FullName;
            account.ModifiedDate = DateTime.Now;
            account.Role = RoleEnum.TENANT;
            if(context.Role != RoleEnum.ADMIN)
            {
                account.ParentId = context.Id;
                account.Role = RoleEnum.USER;
            }

            var resultRegister = await _accountRepository.RegisterForTenant(account);
            var permissions = _permissionService.GetDefaultPermissions(account.Id, context);
            var resultPermission = await _permissionRepository.Save(permissions);
            if(resultRegister)
            {
                if(account.Status.Equals(ActiveStatusEnum.ACTIVE.ToString()))
                {
                    var thread = new Thread(() =>
                    {
                        _mailService.SendMail(new ObjectQueue()
                        {
                            QueueJobType = QueueJobTypeEnum.SEND_MAIL,
                            DataJson = JsonConvert.SerializeObject(new ResetPasswordParameter()
                            {
                                ReceiverAddress = account.Email,
                                AccountId = account.Id,
                                FullName = account.FullName,
                                UserName = account.UserName,
                                MailType = MailTypeEnum.SET_PASSWORD,
                                ParameterType = typeof(ResetPasswordParameter)
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
                        ActionType = ActionEnum.REGISTER_ACCOUNT,
                        ActionName = string.Empty,
                        Description = account.UserName,
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
            if(string.IsNullOrEmpty(userName))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var realAccount = await _accountRepository.GetAccountByUserName(userName);
            if(realAccount == null) 
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản không tồn tại"
                };
            }

            if (!realAccount.Status.Equals(ActiveStatusEnum.ACTIVE.ToString()))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INACTIVE_DATA,
                    Message = "Tài khoản chưa được kích hoạt"
                };
            }

            var resultSendMail = _mailService.SendMail(new ObjectQueue()
            {
                QueueJobType = QueueJobTypeEnum.SEND_MAIL,
                DataJson = JsonConvert.SerializeObject(new ResetPasswordParameter()
                {
                    ReceiverAddress = realAccount.Email,
                    AccountId = realAccount.Id,
                    FullName = realAccount.FullName,
                    UserName = realAccount.UserName,
                    MailType = MailTypeEnum.SET_PASSWORD,
                    ParameterType = typeof(ResetPasswordParameter)
                })
            });

            if(resultSendMail)
            {
                var fakeContext = new Account()
                {
                    FullName = realAccount.FullName,
                    IPAddress = _contextService.GetIPAddress()
                };

                var thread = new Thread(delegate()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        IPAddress = fakeContext.IPAddress,
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id,
                        ActionType = ActionEnum.SEND_RESET_PASSWORD,
                        ActionName = string.Empty,
                        Description = string.Empty,
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

        public async Task<ServiceResultModel> SubmitResetPassword(string code, string password, string userName)
        {
            var param = _keyGenerator.Decryption(code);
            var expireResetPasswordModel = JsonConvert.DeserializeObject<ExpireResetPasswordModel>(param);
            if (expireResetPasswordModel.ExpireTime < DateTime.Now)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Quá hạn thiết lập mật khẩu"
                };
            }

            var account = new Account()
            {
                Id = Guid.Parse(expireResetPasswordModel.AccountId),
                Password = password,
            };

            var isValid = ValidateModelByAttribute(account, new List<string>() { "UserName", "PhoneNumber", "FullName", "Email" });
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var realAccount = await _accountRepository.GetAccountById(account.Id);
            if (realAccount == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản không tồn tại"
                };
            }

            if(realAccount.UserName != userName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NO_INTEGRITY,
                    Message = "Yêu cầu không toàn vẹn"
                };
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var resultResetPassword = await _accountRepository.SavePassword(account.Id, passwordHash);

            if(resultResetPassword)
            {
                var fakeContext = new Account()
                {
                    FullName = realAccount.FullName,
                    IPAddress = _contextService.GetIPAddress()
                };

                var thread = new Thread(delegate()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        IPAddress = fakeContext.IPAddress,
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id,
                        ActionType = ActionEnum.SUBMIT_RESET_PASSWORD,
                        ActionName = string.Empty,
                        Description = string.Empty,
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
