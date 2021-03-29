using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortableManager.Shared;
using PortableManager.Web.Server.Models;
using PortableManager.Web.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        //private static UserModel LoggedOutUser = new UserModel { };
        private readonly UserManager<User> _userManager;

        public AccountsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegisterModel model)
        {
            var newUser = new User { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);
          
            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new RegisterResult { Successful = false, Errors = errors });
            }

            await _userManager.AddToRoleAsync(newUser, "User");
            if (model.Email.StartsWith("admin"))
                await _userManager.AddToRoleAsync(newUser, "Admin");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Accounts",
                new { userId = newUser.Id, code = code },
                protocol: HttpContext.Request.Scheme);

            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(model.Email, "Confirm your account",
                $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>подтвердить email</a>");

            return Ok(new RegisterResult { Successful = true, Messages = new string[] { "Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме" } });
        }

        [HttpGet("send/email")]
        public async Task<IActionResult> SendEmailTest()
        {
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync("vlad.senko2012@gmail.com", "Confirm your account",
                $"Подтвердите регистрацию, перейдя по ссылке");

            return Ok(new CommonRelust { Successful = true, Messages = new string[] { "SENT" } });
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if(userId == null || code == null)
                return BadRequest(new CommonRelust { Successful = false, Messages = new string[] { $"userId == {userId} || code == {code}" } });

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                return BadRequest(new CommonRelust { Successful = false, Messages = new string[] { $"userId == {userId} doesn't exists" } });

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
                return Ok(new CommonRelust { Successful = true, Messages = new string[] { $"userId == {userId} is confirmated" } });
            else
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new CommonRelust { Successful = false, Messages = errors });
            }
        }
    }
}
