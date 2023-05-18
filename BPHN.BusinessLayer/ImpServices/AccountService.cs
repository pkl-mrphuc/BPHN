﻿using BPHN.BusinessLayer.IServices;
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
        private readonly IHistoryLogService _historyLogService;
        
        public AccountService(IAccountRepository accountRepository,
            IContextService contextService, 
            IEmailService mailService,
            IKeyGenerator keyGenerator,
            IHistoryLogService historyLogService)
        {
            _accountRepository = accountRepository;
            _contextService = contextService;
            _mailService = mailService;
            _keyGenerator = keyGenerator;
            _historyLogService = historyLogService;
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
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
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
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
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
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            bool existUserName = _accountRepository.CheckExistUserName(account.UserName);
            if(!existUserName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }

            var realAccount = _accountRepository.GetAccountByUserName(account.UserName);
            if (!BCrypt.Net.BCrypt.Verify(account.Password, realAccount.Password))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }

            string token = _accountRepository.GetToken(realAccount.Id.ToString());

            var thread = new Thread(() =>
            {
                _historyLogService.Write(new HistoryLog()
                {
                    Actor = realAccount.UserName,
                    ActorId = realAccount.Id.ToString(),
                    ActionType = ActionEnum.LOGIN,
                    EntityName = string.Empty
                });
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
                    Token = token
                }
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
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
                };
            }

            bool isValid = ValidateModelByAttribute(account, new List<string>() { "Id", "Password" });
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            bool existUserName = _accountRepository.CheckExistUserName(account.UserName);
            if(existUserName)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EXISTED,
                    Message = "Tên tài khoản đã được đăng ký"
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

                    _historyLogService.Write(new HistoryLog()
                    {
                        Actor = context.UserName,
                        ActorId = context.Id.ToString(),
                        ActionType = ActionEnum.REGISTER_ACCOUNT,
                        EntityName = string.Empty,
                        Description = account.UserName,
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
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var realAccount = _accountRepository.GetAccountByUserName(userName);
            if(realAccount == null) 
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản không tồn tại"
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

            if(resultSendMail)
            {
                Thread thread = new Thread(() =>
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id.ToString(),
                        ActionType = ActionEnum.SEND_RESET_PASSWORD,
                        EntityName = string.Empty,
                        Description = string.Empty,
                    });
                });
                thread.Start();
            }
            
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
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Quá hạn thiết lập mật khẩu"
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
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var realAccount = _accountRepository.GetAccountById(account.Id);
            if (realAccount == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "Tài khoản không tồn tại"
                };
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);
            bool resultResetPassword = _accountRepository.SavePassword(account.Id, passwordHash);

            if(resultResetPassword)
            {
                Thread thread = new Thread(() =>
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        Actor = realAccount.UserName,
                        ActorId = realAccount.Id.ToString(),
                        ActionType = ActionEnum.SUBMIT_RESET_PASSWORD,
                        EntityName = string.Empty,
                        Description = string.Empty,
                    });
                });
                thread.Start();
            }

            return new ServiceResultModel() 
            {
                Success = true,
                Data = resultResetPassword
            };
        }
    }
}
