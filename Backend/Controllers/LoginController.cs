using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PortableManager.Shared;
using PortableManager.Web.Server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginController(IConfiguration configuration, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, true);
            if (result.IsLockedOut)
                return BadRequest(new LoginResult { Successful = false, Error = "You are locked. You can reset your password. Click on \"Забыл пароль\" " });
            if (!result.Succeeded) 
                return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
            
            var user = await _signInManager.UserManager.FindByEmailAsync(login.Email);
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new LoginResult { Successful = false, Error = "Email isn't confirmated" });
            }


            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Email));

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
