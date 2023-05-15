using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
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
    public class AccountRepository : IAccountRepository
    {
        private readonly AppSettings _appSettings;
        public AccountRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public bool CheckExistUserName(string userName)
        {
            return userName == "admin" ? true : false;
        }

        public Account? GetAccountByUserName(string userName)
        {
            return new Account()
            {
                Id = Guid.Parse("6EDE10DA-CBD6-4F61-92A3-A196AB34C2FD"),
                UserName = userName,
                Password = "$2a$11$J0drMq7IOZWCNgHPDIB5beJMRzCTvkuF.1nhkTWYOrfCGChruhn/G",
                Gender = GenderEnum.MALE,
                FullName = "LKP",
                PhoneNumber = "0962521680",
                Email = "lephuc0125@gmail.com",
                Role = RoleEnum.ADMIN,
                Token = "abc"
            };
        }

        public Account? GetAccountById(Guid id)
        {
            return new Account()
            {
                Id = Guid.Parse("6EDE10DA-CBD6-4F61-92A3-A196AB34C2FD"),
                UserName = "admin",
                Password = "$2a$11$J0drMq7IOZWCNgHPDIB5beJMRzCTvkuF.1nhkTWYOrfCGChruhn/G",
                Gender = GenderEnum.MALE,
                FullName = "LKP",
                PhoneNumber = "0962521680",
                Email = "lephuc0125@gmail.com",
                Role = RoleEnum.ADMIN,
                Token = "abc"
            };
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
            return true;
        }

        public List<Account> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new List<Account>();
        }

        public object GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new { TotalPage = 1, TotalRecordCurrentPage = 10, TotalAllRecords = 100 };
        }

        public bool SavePassword(Guid id, string password)
        {
            return true;
        }
    }
}
