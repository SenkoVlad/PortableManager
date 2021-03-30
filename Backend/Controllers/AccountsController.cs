using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortableManager.Shared;
using PortableManager.Shared.Models;
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

        [HttpGet("forgotpassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return Ok(new ForgotPasswordResult { ShowForm = true });
        }

        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok(new ForgotPasswordResult { ShowForm = true, ShowMessages = true, Messages = new string[] { "Пользователь с таким email не существует" } });

            if (!(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok(new ForgotPasswordResult { ShowForm = true, ShowMessages = true, Messages = new string[] { "Email не подтвержден" } });

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ActionLink("ResetPassword", "Accounts", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(model.Email, "Reset Password",
                $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>Reset password</a>");
            return Ok(new ForgotPasswordResult { ShowForm = true, ShowMessages = true, Messages = new string[] { "Ссылка для сброса пароля отправлена на email" } });
        }

        [HttpGet("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string code)
        {
            return Ok();
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return View("ResetPasswordConfirmation");
        //    }
        //    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return View("ResetPasswordConfirmation");
        //    }
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //    return View(model);
        //}
    }
}
