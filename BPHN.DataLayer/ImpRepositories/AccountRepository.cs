using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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
                var affect = await connection.QuerySingleAsync<int>(Query.ACCOUNT__CHECK_EXIST_USERNAME, new Dictionary<string, object>
                {
                    { "@userName", userName }
                });
                return affect > 0 ? true : false;
            }
        }

        public async Task<Account?> GetAccountByUserName(string userName)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var account = await connection.QueryFirstOrDefaultAsync<Account>(Query.ACCOUNT__GET_BY_USERNAME, new Dictionary<string, object>
                {
                    { "@userName", userName }
                });
                return account;
            }
        }

        public async Task<Account?> GetAccountById(Guid id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var account = await connection.QueryFirstOrDefaultAsync<Account>(Query.ACCOUNT__GET_BY_ID, new Dictionary<string, object>
                {
                    { "@id", id }
                });
                return account;
            }
        }

        public (string token, string refreshToken) GetToken(Guid id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var refreshTokenKey = Encoding.ASCII.GetBytes(_appSettings.Secret1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id.ToString()) }),
                Expires = DateTime.Now.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id.ToString()) }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(refreshTokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);
            return (tokenHandler.WriteToken(token), tokenHandler.WriteToken(refreshToken));
        }

        public async Task<bool> RegisterForTenant(Account account)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.ACCOUNT__INSERT, new Dictionary<string, object?>
                {
                    { "@id", account.Id },
                    { "@fullName", account.FullName },
                    { "@userName", account.UserName },
                    { "@password", account.Password },
                    { "@gender", account.Gender },
                    { "@email", account.Email },
                    { "@phoneNumber", account.PhoneNumber },
                    { "@role", account.Role.ToString() },
                    { "@status", account.Status },
                    { "@parentId", account.ParentId },
                    { "@createdDate", account.CreatedDate },
                    { "@createdBy", account.CreatedBy },
                    { "@modifiedDate", account.ModifiedDate },
                    { "@modifiedBy", account.ModifiedBy },
                });
                return affect > 0 ? true : false;
            }
        }

        public async Task<List<Account>> GetPaging(int pageIndex, int pageSize, string txtSearch, List<WhereCondition> where)
        {

            var query = string.Empty;
            var countQuery = string.Empty;
            var dic = new Dictionary<string, object>();
            var whereQuery = BuildWhereQuery(where);
            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                query = $@"  select distinct ac.UserName, ac.FullName, ac.Gender, ac.PhoneNumber, ac.Email, ac.Id, ac.Role, ac.Status, ac.ModifiedDate from  (
                                                    select * from accounts where {whereQuery} and UserName like @where
                                                    union 
                                                    select * from accounts where {whereQuery} and Email like @where
                                                    union 
                                                    select * from accounts where {whereQuery} and PhoneNumber like @where
                                                    union 
                                                    select * from accounts where {whereQuery} and FulLName like @where) as ac
                            order by ac.ModifiedDate desc
                            limit @offSet, @pageSize";
                countQuery = $@" select distinct count(*) from   (
                                                                select * from accounts where Role = {whereQuery} and UserName like @where
                                                                union 
                                                                select * from accounts where Role = {whereQuery} and Email like @where
                                                                union 
                                                                select * from accounts where {whereQuery} and PhoneNumber like @where
                                                                union 
                                                                select * from accounts where Role = {whereQuery} and FulLName like @where) as ac";

                dic.Add("@where", $"%{txtSearch}%");
            }
            else
            {
                query = $"select UserName, FullName, Gender, PhoneNumber, Email, Id, Role, Status, ModifiedDate from accounts where {whereQuery} order by ModifiedDate desc limit @offSet, @pageSize";
                countQuery = $"select count(*) from accounts where {whereQuery}";
            }

            for (int i = 0; i < where.Count; i++)
            {
                dic.Add($"@where{i}", where[i].Value);
            }


            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                var offSet = (pageIndex - 1) * pageSize;

                dic.Add("@offSet", offSet);
                dic.Add("@pageSize", pageSize);
                var lstAccount = await connection.QueryAsync<Account>(query, dic);
                return lstAccount.ToList();
            }
        }

        public async Task<object> GetCountPaging(int pageIndex, int pageSize, string txtSearch, List<WhereCondition> where)
        {
            var countQuery = string.Empty;
            var dic = new Dictionary<string, object>();
            var whereQuery = BuildWhereQuery(where);
            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                countQuery = $@" select distinct count(*) from   (
                                                                select * from accounts where {whereQuery} and UserName like @where
                                                                union 
                                                                select * from accounts where {whereQuery} and Email like @where
                                                                union 
                                                                select * from accounts where {whereQuery} and PhoneNumber like @where
                                                                union 
                                                                select * from accounts where {whereQuery} and FulLName like @where) as ac";
                dic.Add("@where", $"%{txtSearch}%");
            }
            else
            {
                countQuery = $"select count(*) from accounts where {whereQuery}";
            }

            for (int i = 0; i < where.Count; i++)
            {
                dic.Add($"@where{i}", where[i].Value);
            }

            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                var totalRecordCurrentPage = 0;
                if (totalRecord > 0)
                {
                    if (pageIndex == totalPage)
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
                var affect = await connection.ExecuteAsync(Query.ACCOUNT__UPDATE_PASSWORD, new Dictionary<string, object>
                {
                    { "@id", id },
                    { "@password", password }
                });
                return affect > 0 ? true : false;
            }
        }

        public async Task<IEnumerable<Guid>> GetRelationIds(Guid id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var result = await connection.QueryAsync<Guid>(Query.ACCOUNT__GET_RELATION_IDS, new Dictionary<string, object>
                {
                    { "@id", id }
                });
                return result ?? Enumerable.Empty<Guid>();
            }
        }

        public async Task<List<Account>> GetAll()
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var lstAccount = await connection.QueryAsync<Account>(Query.ACCOUNT__GET_ALL);
                return lstAccount.ToList();
            }
        }

        public async Task SaveToken(Guid id, string token, string refreshToken)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                await connection.ExecuteAsync(Query.ACCOUNT__UPDATE_TOKEN, new Dictionary<string, object>()
                {
                    { "@id", id},
                    { "@token", token},
                    { "@refreshToken", refreshToken }
                });
            }
        }

        public async Task<bool> UpdateTenant(Account account)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.ACCOUNT__UPDATE, new Dictionary<string, object>()
                {
                    { "@id", account.Id},
                    { "@gender", account.Gender},
                    { "@phoneNumber", account.PhoneNumber},
                    { "@fullName", account.FullName},
                    { "@status",  account.Status },
                });
                return affect > 0 ? true : false;
            }
        }

        public Guid ValidateToken(string token, bool isRefreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(isRefreshToken ? _appSettings.Secret1 : _appSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            if (jwtToken is not null && Guid.TryParse(jwtToken.Claims.First(x => "id".Equals(x.Type)).Value, out var accountId) && jwtToken.ValidTo >= DateTime.Now)
            {
                return accountId;
            }
            return Guid.Empty;
        }
    }
}
