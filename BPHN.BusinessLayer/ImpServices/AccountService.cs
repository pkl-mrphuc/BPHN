using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.Others;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IContextService _contextService;
        private readonly IEmailService _mailService;
        private readonly IKeyGenerator _keyGenerator;
        
        public AccountService(IAccountRepository accountRepository,
            IContextService contextService, 
            IEmailService mailService,
            IKeyGenerator keyGenerator)
        {
            _accountRepository = accountRepository;
            _contextService = contextService;
            _mailService = mailService;
            _keyGenerator = keyGenerator;
        }

        public ServiceResultModel GetById(Guid id)
        {
            return new ServiceResultModel()
            {
                Success = true,
                Data = _accountRepository.GetAccountById(id)
            };
        }

        public ServiceResultModel GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            var context = _contextService.GetContext();
            if (context == null || (context != null && context.Role != RoleEnum.ADMIN))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "No Role"
                };
            }

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var resultCountPaging = _accountRepository.GetCountPaging(pageIndex, pageSize, txtSearch);

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultCountPaging
            };
        }

        public ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            var context = _contextService.GetContext();
            if(context == null || (context != null && context.Role != RoleEnum.ADMIN))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "No Role"
                };
            }

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var lstTenants = _accountRepository.GetPaging(pageIndex, pageSize, txtSearch);

            return new ServiceResultModel()
            {
                Success = true,
                Data = lstTenants
            };
        }

        public ServiceResultModel Login(Account account)
        {
            bool isValid = ValidateModelByAttribute(account, new List<string>() { "Id", "PhoneNumber", "FullName", "Email" });
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "Input Empty"
                };
            }

            bool existUserName = _accountRepository.CheckExistUserName(account.UserName);
            if(!existUserName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "Account Not Found"
                };
            }

            var realAccount = _accountRepository.GetAccountByUserName(account.UserName);
            if (realAccount != null)
            {
                
                if (!BCrypt.Net.BCrypt.Verify(account.Password, realAccount.Password))
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        Message = "Password Incorrect"
                    };
                }

                string token = _accountRepository.GetToken(realAccount.Id.ToString());
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
                        Token = token
                    }
                };
            }

            return new ServiceResultModel()
            {
                Success = false,
                Message = "Login Fail"
            };
        }

        public ServiceResultModel RegisterForTenant(Account account)
        {
            var context = _contextService.GetContext();
            if (context == null ||
                (context != null && context.Role != RoleEnum.ADMIN))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "No Role"
                };
            }

            bool isValid = ValidateModelByAttribute(account, new List<string>() { "Id", "Password" });
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "Input Invalid"
                };
            }

            bool existUserName = _accountRepository.CheckExistUserName(account.UserName);
            if(existUserName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "UserName Existed"
                };
            }

            bool resultRegister = _accountRepository.RegisterForTenant(account);
            if(resultRegister)
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

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultRegister
            };
        }

        public ServiceResultModel ResetPassword(string userName)
        {
            if(string.IsNullOrEmpty(userName))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "Input Empty"
                };
            }

            var realAccount = _accountRepository.GetAccountByUserName(userName);
            if(realAccount == null) 
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "User NotExist"
                };
            }

            bool resultSendMail = _mailService.SendMail(new ObjectQueue()
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
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultSendMail
            };
        }

        public ServiceResultModel SubmitResetPassword(string code, string password)
        {
            string param = _keyGenerator.Decryption(code);
            var expireResetPasswordModel = JsonConvert.DeserializeObject<ExpireResetPasswordModel>(param);
            if (expireResetPasswordModel.ExpireTime < DateTime.Now)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "ExpireTime"
                };
            }

            var account = new Account()
            {
                Id = Guid.Parse(expireResetPasswordModel.AccountId),
                Password = password,
            };

            bool isValid = ValidateModelByAttribute(account, new List<string>() { "UserName", "PhoneNumber", "FullName", "Email" });
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "Input Empty"
                };
            }

            var realAccount = _accountRepository.GetAccountById(account.Id);
            if (realAccount == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    Message = "User NotExist"
                };
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);
            bool resultResetPassword = _accountRepository.SavePassword(account.Id, passwordHash);
            return new ServiceResultModel() 
            {
                Success = true,
                Data = resultResetPassword
            };
        }
    }
}
