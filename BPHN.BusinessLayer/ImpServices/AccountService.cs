using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.AspNetCore.Authentication;
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
        private readonly IConfigService _configService;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;
		private readonly IPermissionService _permissionService;
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
            IFileService fileService) : base(serviceProvider, appSettings)
        {
            _accountRepository = accountRepository;
            _mailService = mailService;
            _keyGenerator = keyGenerator;
            _historyLogService = historyLogService;
            _configService = configService;
            _notificationService = notificationService;
            _fileService = fileService;
			_permissionService = permissionService;
        }

		public async Task<ServiceResultModel> ChangePassword(Account account)
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

			var isValid = ValidateModelByAttribute(account, "UserName", "PhoneNumber", "FullName", "Email");
			if (!isValid)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.EMPTY_INPUT,
					Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
				};
			}

			if (account.Id != context.Id)
			{
				return new ServiceResultModel
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
                account.RelationIds = await _accountRepository.GetRelationIds(id);
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
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.OUT_TIME,
					Message = _resourceService.Get(SharedResourceKey.OUTTIME)
				};
			}

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTUSER);
            if (!hasPermission || (context.Role != RoleEnum.ADMIN && !await _configService.AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel
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
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.OUT_TIME,
					Message = _resourceService.Get(SharedResourceKey.OUTTIME)
				};
			}

            if (!string.IsNullOrWhiteSpace(id) && !Guid.TryParse(id, out var accountId))
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
                };
            }

            var data = new Account();
            data.Id = Guid.NewGuid();
            data.Gender = GenderEnum.MALE.ToString();
            data.Status = ActiveStatusEnum.ACTIVE.ToString();
            if (Guid.TryParse(id, out accountId))
            {
                data = await _accountRepository.GetAccountById(accountId);
                if (data is null)
                {
                    return new ServiceResultModel
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig)
                    };
                }

                data.AvatarUrl = _fileService.GetFileUrl(accountId.ToString());
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
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.OUT_TIME,
					Message = _resourceService.Get(SharedResourceKey.OUTTIME)
				};
			}

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTUSER);
            if (!hasPermission || (context.Role != RoleEnum.ADMIN && !await _configService.AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel
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

		public ServiceResultModel GetTokenInfo(string token)
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				return new ServiceResultModel
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
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.OUT_TIME,
					Message = _resourceService.Get(SharedResourceKey.OUTTIME)
				};
			}

			return new ServiceResultModel
			{
				Success = true,
				Data = jwtToken
			};
		}

		public async Task<ServiceResultModel> Login(Account account)
		{
			var isValid = ValidateModelByAttribute(account, "Id", "PhoneNumber", "FullName", "Email");
			if (!isValid)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.EMPTY_INPUT,
					Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
				};
			}

            var realAccount = await _accountRepository.GetAccountByUserName(account.UserName);
            if (realAccount is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.LOGINFAIL)
                };
            }

            if (!ActiveStatusEnum.ACTIVE.ToString().Equals(realAccount.Status))
            {
                return new ServiceResultModel
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
					return new ServiceResultModel
					{
						Success = false,
						ErrorCode = ErrorCodes.NOT_EXISTS,
						Message = _resourceService.Get(SharedResourceKey.LOGINFAIL)
					};
				}
			}
			catch (Exception)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.NOT_EXISTS,
					Message = _resourceService.Get(SharedResourceKey.LOGINFAIL)
				};
			}

			string token = _accountRepository.GetToken(realAccount.Id.ToString());
			string refreshToken = _accountRepository.GetRefreshToken(realAccount.Id.ToString());

			_accountRepository.SaveToken(realAccount.Id, token, refreshToken);

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
                    RelationIds = await _accountRepository.GetRelationIds(realAccount.Id),
                    AvatarUrl = _fileService.GetFileUrl(realAccount.Id.ToString())
                }
            };
        }

		public async Task<ServiceResultModel> LoginGoogle(AuthenticateResult authenticateResult)
		{
			throw new Exception();
		}

		public async Task<ServiceResultModel> Refresh()
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

            return new ServiceResultModel
            {
                Success = true
            };
        }

		public ServiceResultModel RefreshToken(string refreshToken)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_appSettings.Secret1);
			tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			var jwtToken = (JwtSecurityToken)validatedToken;

			if (jwtToken is null)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.INACTIVE_DATA
				};
			}

			var userId = Guid.Parse(jwtToken.Claims.First(x => "id".Equals(x.Type)).Value);
			var expiredTimeTick = long.Parse(jwtToken.Claims.First(x => "expiredTime".Equals(x.Type)).Value);
			var expiredTime = new DateTime(expiredTimeTick);
			var token = _accountRepository.GetToken(userId.ToString());
			if (expiredTime < DateTime.UtcNow)
			{
				refreshToken = _accountRepository.GetRefreshToken(userId.ToString());
			}

			_accountRepository.SaveToken(userId, token, refreshToken);

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
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.OUT_TIME,
					Message = _resourceService.Get(SharedResourceKey.OUTTIME)
				};
			}

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.ADDUSER);
            if (!hasPermission || (context.Role != RoleEnum.ADMIN && !await _configService.AllowMultiUser(context.Id)))
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

			var isValid = ValidateModelByAttribute(account, "Id", "Password");
			if (!isValid)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.EMPTY_INPUT,
					Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
				};
			}

			var existUserName = await _accountRepository.CheckExistUserName(account.UserName);
			if (existUserName)
			{
				return new ServiceResultModel
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
            account.Permissions = _permissionService.GetDefaultPermissions(account.Id, context);
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
            var _ = await _permissionService.SavePermissions(account.Id, account.Permissions);
            if (resultRegister)
            {
                await _notificationService.Insert<Account>(context, NotificationTypeEnum.INSERTACCOUNT, new Account
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

		public async Task<ServiceResultModel> ResetPassword(string userName)
		{
			if (string.IsNullOrWhiteSpace(userName))
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.EMPTY_INPUT,
					Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
				};
			}

			var realAccount = await _accountRepository.GetAccountByUserName(userName);
			if (realAccount is null)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.NOT_EXISTS,
					Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
				};
			}

            if (!ActiveStatusEnum.ACTIVE.ToString().Equals(realAccount.Status))
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INACTIVE_DATA,
                    Message = _resourceService.Get(SharedResourceKey.INACTIVESTATUS)
                };
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
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.EMPTY_INPUT,
					Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
				};
			}

            var parameter = _keyGenerator.Decryption(code);
            var expireResetPasswordModel = JsonConvert.DeserializeObject<ExpireSetPasswordModel>(parameter);
            if (expireResetPasswordModel is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

			if (expireResetPasswordModel.ExpireTime < DateTime.Now)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.OUT_TIME,
					Message = _resourceService.Get(SharedResourceKey.OUTTIME)
				};
			}

			var account = new Account
			{
				Id = Guid.Parse(expireResetPasswordModel.AccountId),
				Password = password,
			};

			var isValid = ValidateModelByAttribute(account, "UserName", "PhoneNumber", "FullName", "Email");
			if (!isValid)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.EMPTY_INPUT,
					Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
				};
			}

			var realAccount = await _accountRepository.GetAccountById(account.Id);
			if (realAccount is null)
			{
				return new ServiceResultModel
				{
					Success = false,
					ErrorCode = ErrorCodes.NOT_EXISTS,
					Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
				};
			}

			if (realAccount.UserName != userName)
			{
				return new ServiceResultModel
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
			var result = GetTokenInfo(token);
			return new ServiceResultModel
			{
				Success = result.Success
			};
		}

        public async Task<IEnumerable<Guid>> GetRelationIds(Guid id)
        {
            return await _accountRepository.GetRelationIds(id);
        }
    }
}
