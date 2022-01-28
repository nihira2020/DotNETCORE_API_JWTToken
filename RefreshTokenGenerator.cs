using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CustomerAPI.Models;

namespace CustomerAPI
{

    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly Learn_DBContext context;

        public RefreshTokenGenerator(Learn_DBContext learn_DB)
        {
            context = learn_DB;
        }
        public string GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string RefreshToken = Convert.ToBase64String(randomnumber);

                var _user = context.TblRefreshtoken.FirstOrDefault(o => o.UserId == username);
                if (_user != null)
                {
                    _user.RefreshToken = RefreshToken;
                    context.SaveChanges();
                }
                else
                {
                    TblRefreshtoken tblRefreshtoken = new TblRefreshtoken()
                    {
                        UserId=username,
                        TokenId=new Random().Next().ToString(),
                        RefreshToken= RefreshToken,
                        IsActive=true
                    };
                }

                return RefreshToken;
            }
        }
    }
}
