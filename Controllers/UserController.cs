using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerAPI.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CustomerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Learn_DBContext context;
        private readonly JWTSetting setting;
        private readonly IRefreshTokenGenerator tokenGenerator;
        public UserController(Learn_DBContext learn_DB, IOptions<JWTSetting> options, IRefreshTokenGenerator _refreshToken)
        {
            context = learn_DB;
            setting = options.Value;
            tokenGenerator = _refreshToken;
        }

        [NonAction]
        public TokenResponse Authenticate(string username,Claim[] claims)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var tokenkey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenhandler = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                 signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)

                );
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(username);

            return tokenResponse;
        }

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] usercred user)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var _user = context.TblUser.FirstOrDefault(o => o.Userid == user.username && o.Password == user.password);
            if (_user == null)
                return Unauthorized();

            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _user.Userid),
                        new Claim(ClaimTypes.Role, _user.Role)

                    }
                ),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenhandler.WriteToken(token);

            tokenResponse.JWTToken = finaltoken;
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(user.username);

            return Ok(tokenResponse);
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {
           
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token.JWTToken);
            var username = securityToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
           

            //var username = principal.Identity.Name;
            var _reftable = context.TblRefreshtoken.FirstOrDefault(o => o.UserId == username && o.RefreshToken == token.RefreshToken);
            if (_reftable == null)
            {
                return Unauthorized();
            }
            TokenResponse _result = Authenticate(username, securityToken.Claims.ToArray());
            return Ok(_result);
        }

        [Route("GetMenubyRole/{role}")]
        [HttpGet]
        public IActionResult GetMenubyRole(string role)
        {
            var _result = (from q1 in context.TblPermission.Where(item=>item.RoleId==role)
                          join q2 in context.TblMenu
                          on q1.MenuId equals q2.Id
                          select new { q1.MenuId, q2.Name, q2.LinkName }).ToList();
           // var _result = context.TblPermission.Where(o => o.RoleId == role).ToList();
           
            return Ok(_result);
        }

        [Route("HaveAccess")]
        [HttpGet]
        public IActionResult HaveAccess(string role,string menu)
        {
            APIResponse result = new APIResponse();
            //var username = principal.Identity.Name;
            var _result = context.TblPermission.Where(o => o.RoleId == role && o.MenuId == menu).FirstOrDefault();
            if (_result != null)
            {
                result.result = "pass";
            }
            return Ok(result);
        }

    }
}
