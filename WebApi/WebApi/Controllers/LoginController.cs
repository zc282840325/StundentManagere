using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApi.EF;
using WebApi.JWT;
using WebApi.Model;
using WebApi.Tool;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;
        readonly PermissionRequirement _requirement;

        public LoginController(UserDbContext context, IConfiguration configuration, PermissionRequirement permission)
        {
            _context = context;
            _configuration = configuration;
            _requirement = permission;
        }
        /// <summary>
        /// 验证登录获取Token
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Login([FromBody] Login login)
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.UserPwd))
            {
                return new JsonResult(new
                {
                    Status = false,
                    message = "用户名或密码不能为空"
                });
            }
            login.UserPwd = MD5Helper.MD5Encrypt32(login.UserPwd);
            Console.WriteLine(login.UserPwd);
            var user = _context.Users.Where(x => x.UserName == login.UserName && x.UserPwd == login.UserPwd && x.IsDelete == false).FirstOrDefault();

            string roleName = "";
            var roleList = await _context.Roles.Where(x => x.IsDelete == false).ToListAsync();
            if (user != null)
            {
                var userRoles = await _context.UserRoles.Where(a => a.IsDelete == false && a.Uid == user.Id).ToListAsync();
                if (userRoles.Count > 0)
                {
                    var arr = userRoles.Select(ur => ur.Rid.ObjToString()).ToList();
                    var roles = roleList.Where(d => arr.Contains(d.Id.ObjToString()));
                    roleName = string.Join(',', roles.Select(r => r.RoleName).ToArray());
                }
                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim("GID", "-9999"),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };


                claims.AddRange(roleName.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                //用户标识
                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(claims);

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                return new JsonResult(new
                {
                    accessToken = token,
                    username = user.UserName
                });
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "认证失败"

                });
            }
        }

        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken")]  
        public async Task<object> RefreshToken(string token = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                return new JsonResult(new
                {
                    Status = false,
                    message = "token无效，请重新登录！"
                });
            }
            var tokenModel = JwtToken.SerializeJwt(token);
            if (tokenModel != null && tokenModel.Uid > 0)
            {
                string roleName = "";
                var roleList = await _context.Roles.Where(x => x.IsDelete == false).ToListAsync();
                var user = await _context.Users.FindAsync(tokenModel.Uid);
                if (user != null)
                {
                    var userRoles = await _context.UserRoles.Where(a => a.IsDelete == false && a.Uid == user.Id).ToListAsync();
                    if (userRoles.Count > 0)
                    {
                        var arr = userRoles.Select(ur => ur.Rid.ObjToString()).ToList();
                        var roles = roleList.Where(d => arr.Contains(d.Id.ObjToString()));
                        roleName = string.Join(',', roles.Select(r => r.RoleName).ToArray());
                    }
                    //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ObjToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                    claims.AddRange(roleName.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                    //用户标识
                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    var refreshToken = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                    return new JsonResult(refreshToken);
                }
            }

            return new JsonResult(new
            {
                success = false,
                message = "认证失败"
            });
        }
       /// <summary>
       /// 验证token过期
       /// </summary>
       /// <param name="token"></param>
       /// <returns></returns>
        [HttpGet]
        [Route("Verification")]
        public object Verification(string token="")
        {
            bool flag = false;
            string msg ="Token过期";
            if (!string.IsNullOrEmpty(token))
            {
                var tokenModel = JwtToken.SerializeJwt(token);
                if (tokenModel != null && tokenModel.Uid > 0)
                {
                    if (DateTime.Compare(tokenModel.Expiration,DateTime.Now)>=0)
                    {
                        flag=true;
                        msg = "Token有效";
                    }
                }
            }
            return new JsonResult(new
            {
                success = flag,
                message = msg
            });
        }
    }
}
