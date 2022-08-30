using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class TokenServiceCustom : ITokenService
    {
        //private readonly UserManager<Account> _userManager;
        //private readonly IConfiguration _config;
        //public TokenServiceCustom(UserManager<Account> userManager, IConfiguration config)
        //{
        //    _config = config;
        //    _userManager = userManager;
        //}

        //public async Task<string> GenerateToken(Account account)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Email, account.Email),
        //        new Claim(ClaimTypes.Name, account.UserName)
        //    };

        //    var roles = await _userManager.GetRolesAsync(account);

        //    foreach (var role in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:TokenKey"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        //    var tokenOptions = new JwtSecurityToken
        //    (
        //        issuer: null,
        //        audience: null,
        //        claims: claims,
        //        expires: DateTime.Now.AddDays(7),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        //}

        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<Account> _userManager;
        public TokenServiceCustom(IConfiguration config, UserManager<Account> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTSettings:TokenKey"]));
        }

        public async Task<string> GenerateToken(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
