using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI
{
  public  interface IRefreshTokenGenerator
    {
        string GenerateToken(string username);
    }
}
