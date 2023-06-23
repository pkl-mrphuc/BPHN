using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BPHN.DataLayer.ImpRepositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly AppSettings _appSettings;
        public AccountRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<bool> CheckExistUserName(string userName)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = "select count(*) from accounts where UserName = @userName";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@userName", userName);
                int affect = await connection.QuerySingleAsync<int>(query, dic);
                return affect > 0 ? true : false;
            }
        }

        public async Task<Account?> GetAccountByUserName(string userName)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = "select * from accounts where UserName = @userName";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@userName", userName);
                var account = await connection.QueryFirstOrDefaultAsync<Account>(query, dic);
                return account;
            }
        }

        public async Task<Account?> GetAccountById(Guid id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = "select UserName, FullName, Gender, PhoneNumber, Email, Id, Role, Status from accounts where Id = @id";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@id", id);
                var account = await connection.QueryFirstOrDefaultAsync<Account>(query, dic);
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

        public async Task<bool> RegisterForTenant(Account account)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = @"insert into accounts(Id, UserName, Password, Gender, PhoneNumber, FullName, Email, Role, Status, ParentId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
                                value (@id, @userName, @password, @gender, @phoneNumber, @fullName, @email, @role, @status, @parentId, @createdBy, @createdDate, @modifiedBy, @modifiedDate)";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@id", account.Id);
                dic.Add("@fullName", account.FullName);
                dic.Add("@userName", account.UserName);
                dic.Add("@password", account.Password);
                dic.Add("@gender", account.Gender);
                dic.Add("@email", account.Email);
                dic.Add("@phoneNumber", account.PhoneNumber);
                dic.Add("@role", account.Role.ToString());
                dic.Add("@status", account.Status);
                dic.Add("@parentId", account.ParentId);
                dic.Add("@createdDate", account.CreatedDate);
                dic.Add("@createdBy", account.CreatedBy);
                dic.Add("@modifiedDate", account.ModifiedDate);
                dic.Add("@modifiedBy", account.ModifiedBy);
                int affect = await connection.ExecuteAsync(query, dic);
                return affect > 0 ? true : false;
            }
        }

        public async Task<List<Account>> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {

            string query = string.Empty;
            string countQuery = string.Empty;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                query = @"  select distinct ac.UserName, ac.FullName, ac.Gender, ac.PhoneNumber, ac.Email, ac.Id, ac.Role, ac.Status, ac.ModifiedDate from  (
                                                    select * from accounts where Role = @where0 and UserName like @where1
                                                    union 
                                                    select * from accounts where Role = @where0 and Email like @where2
                                                    union 
                                                    select * from accounts where Role = @where0 and FulLName like @where3) as ac
                            order by ac.ModifiedDate desc
                            limit @offSet, @pageSize";
                countQuery = @" select distinct count(*) from   (
                                                                select * from accounts where Role = @where0 and UserName like @where1
                                                                union 
                                                                select * from accounts where Role = @where0 and Email like @where2
                                                                union 
                                                                select * from accounts where Role = @where0 and FulLName like @where3) as ac";
                dic.Add("@where0", "TENANT");
                dic.Add("@where1", $"%{txtSearch}%");
                dic.Add("@where2", $"%{txtSearch}%");
                dic.Add("@where3", $"%{txtSearch}%");
            }
            else 
            {
                query = "select UserName, FullName, Gender, PhoneNumber, Email, Id, Role, Status, ModifiedDate from accounts where Role = @where0 order by ModifiedDate desc limit @offSet, @pageSize";
                countQuery = "select count(*) from accounts where Role = @where0";
                dic.Add("@where0", "TENANT");
            }


            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                int totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                int offSet = (pageIndex - 1) * pageSize;

                dic.Add("@offSet", offSet);
                dic.Add("@pageSize", pageSize);
                var lstAccount = await connection.QueryAsync<Account>(query, dic);
                return lstAccount.ToList();
            }
        }

        public async Task<object> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            string countQuery = string.Empty;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                countQuery = @" select distinct count(*) from   (
                                                                select * from accounts where Role = @where0 and UserName like @where1
                                                                union 
                                                                select * from accounts where Role = @where0 and Email like @where2
                                                                union 
                                                                select * from accounts where Role = @where0 and FulLName like @where3) as ac";
                dic.Add("@where0", "TENANT");
                dic.Add("@where1", $"%{txtSearch}%");
                dic.Add("@where2", $"%{txtSearch}%");
                dic.Add("@where3", $"%{txtSearch}%");
            }
            else
            {
                countQuery = "select count(*) from accounts where Role = @where0";
                dic.Add("@where0", "TENANT");
            }

            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                int totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
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

        public async Task<bool> SavePassword(Guid id, string password)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                string query = @"update accounts set Password = @password where Id = @id";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@id", id);
                dic.Add("@password", password);
                int affect = await connection.ExecuteAsync(query, dic);
                return affect > 0 ? true : false;
            }
        }
    }
}
