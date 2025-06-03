using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
                var result = await connection.ExecuteScalarAsync<bool>(Query.ACCOUNT__CHECK_EXIST_USERNAME, new Dictionary<string, object>
                {
                    { "@userName", userName }
                });
                return result;
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

        public (string token, string refreshToken) GetToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var refreshTokenKey = Encoding.ASCII.GetBytes(_appSettings.Secret1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", account.Id.ToString()),
                    new Claim("fullName", account.FullName),
                    new Claim("userName", account.UserName),
                    new Claim("role", account.Role.ToString("D")),
                    new Claim("ipAddress", account.IPAddress ?? ""),
                    new Claim("languageConfig", account.LanguageConfig ?? ""),
                    new Claim("parentId", account.ParentId?.ToString() ?? ""),
                }),
                Expires = DateTime.Now.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
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
                return affect > 0;
            }
        }

        public async Task<IEnumerable<Account>> GetPaging(Guid accountId, RoleEnum role, int pageIndex, int pageSize, string txtSearch)
        {
            var conditions = new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "Id",
                    Operator = "!=",
                    Value = accountId
                }
            };

            if (role == RoleEnum.TENANT)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "ParentId",
                    Operator = "=",
                    Value = accountId
                });
            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Email",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }

            var where = BuildWhere(Query.ACCOUNT__GET_ALL, conditions, "limit @offSet, @pageSize");
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var totalRecord = await GetTotalRecord(accountId, role, txtSearch);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                var offSet = (pageIndex - 1) * pageSize;

                where.param.Add("@offSet", offSet);
                where.param.Add("@pageSize", pageSize);
                var lstAccount = await connection.QueryAsync<Account>(where.query, where.param);
                return lstAccount ?? Enumerable.Empty<Account>();
            }
        }

        public async Task<object> GetCountPaging(Guid accountId, RoleEnum role, int pageIndex, int pageSize, string txtSearch)
        {
            var totalRecord = await GetTotalRecord(accountId, role, txtSearch);
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
                return affect > 0;
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
                return affect > 0;
            }
        }

        public Account? ValidateToken(string token, bool isRefreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(isRefreshToken ? _appSettings.Secret1 : _appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                if (jwtToken is not null && jwtToken.ValidTo >= DateTime.Now && !jwtToken.Claims.IsNullOrEmpty())
                {
                    var claims = jwtToken.Claims.ToDictionary(x => x.Type, x => x.Value);

                    var keys = new string[] { "id", "fullName", "languageConfig", "role", "userName", "ipAddress", "parentId" };
                    var properties = keys.ToDictionary(x => x, x => claims.ContainsKey(x) ? claims[x] : "");
                    return new Account
                    {
                        Id = Guid.Parse(properties["id"]),
                        FullName = properties["fullName"],
                        LanguageConfig = properties["languageConfig"],
                        Role = Enum.Parse<RoleEnum>(properties["role"]),
                        UserName = properties["userName"],
                        IPAddress = properties["ipAddress"],
                        ParentId = Guid.TryParse(properties["parentId"], out var parentId) ? parentId : null
                    };
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        public async Task<int> GetTotalRecord(Guid accountId, RoleEnum role, string txtSearch)
        {
            var conditions = new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "Id",
                    Operator = "!=",
                    Value = accountId
                }
            };

            if (role == RoleEnum.TENANT)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "ParentId",
                    Operator = "=",
                    Value = accountId
                });
            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Email",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }

            var where = BuildWhere(Query.ACCOUNT__COUNT, conditions);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                return await connection.QuerySingleAsync<int>(where.query, where.param);
            }
        }
    }
}
