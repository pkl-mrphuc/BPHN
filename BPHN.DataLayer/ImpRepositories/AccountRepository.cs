using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.ImpRepositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly AppSettings _appSettings;
        public AccountRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public bool CheckExistUserName(string userName)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = "select count(*) from accounts where UserName = @userName";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@userName", userName);
                int affect = connection.QuerySingle<int>(query, dic);
                return affect > 0 ? true : false;
            }
        }

        public Account? GetAccountByUserName(string userName)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = "select * from accounts where UserName = @userName";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@userName", userName);
                var account = connection.QueryFirstOrDefault<Account>(query, dic);
                return account;
            }
        }

        public Account? GetAccountById(Guid id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = "select * from accounts where Id = @id";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@id", id);
                var account = connection.QueryFirstOrDefault<Account>(query, dic);
                return account;
            }
        }

        public string GetToken(string id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool RegisterForTenant(Account account)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = @"insert into accounts(Id, UserName, Password, Gender, PhoneNumber, FullName, Email, Role, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
                                value (@id, @userName, @password, @gender, @phoneNumber, @fullName, @email, @role, @createdBy, @createdDate, @modifiedBy, @modifiedDate)";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@id", account.Id);
                dic.Add("@fullName", account.FullName);
                dic.Add("@userName", account.UserName);
                dic.Add("@password", account.Password);
                dic.Add("@gender", account.Gender.ToString());
                dic.Add("@email", account.Email);
                dic.Add("@phoneNumber", account.PhoneNumber);
                dic.Add("@role", RoleEnum.TENANT.ToString());
                dic.Add("@createdDate", account.CreatedDate);
                dic.Add("@createdBy", account.CreatedBy);
                dic.Add("@modifiedDate", account.ModifiedDate);
                dic.Add("@modifiedBy", account.ModifiedBy);
                int affect = connection.Execute(query, dic);
                return affect > 0 ? true : false;
            }
        }

        public List<Account> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            string whereQuery = BuildWhereQuery(where);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            for (int i = 0; i < where.Count; i++)
            {
                var item = string.Format("@where{0}", i);
                dic.Add(item, where[i].Value);
            }
            string query = $"select * from accounts where {whereQuery} order by ModifiedDate desc limit @offSet, @pageSize";


            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                int totalRecord = connection.QuerySingle<int>($"select count(*) from accounts where {whereQuery}", dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                int offSet = (pageIndex - 1) * pageSize;

                dic.Add("@offSet", offSet);
                dic.Add("@pageSize", pageSize);
                var lstAccount = connection.Query<Account>(query, dic);
                return lstAccount.ToList();
            }
        }

        public object GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            string whereQuery = BuildWhereQuery(where);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            for (int i = 0; i < where.Count; i++)
            {
                var item = string.Format("@where{0}", i);
                dic.Add(item, where[i].Value);
            }

            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                int totalRecord = connection.QuerySingle<int>($"select count(*) from accounts where {whereQuery}", dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                int totalRecordCurrentPage = 0;
                if (totalRecord > 0)
                {
                    if(pageIndex == totalPage)
                    {
                        totalRecordCurrentPage = totalRecord - ((pageIndex - 1) * pageSize);
                    }
                    else
                    {
                        totalRecordCurrentPage = pageSize;
                    }
                }
                return new { TotalPage = totalPage, TotalRecordCurrentPage = totalRecordCurrentPage, TotalAllRecords = totalRecord };
            }
        }

        public bool SavePassword(Guid id, string password)
        {
            return true;
        }
    }
}
