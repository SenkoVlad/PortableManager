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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var newUser = new User { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new RegisterResult { Successful = false, Errors = errors });
            }

            await _userManager.AddToRoleAsync(newUser, "User");
            if (model.Email.StartsWith("admin"))
                await _userManager.AddToRoleAsync(newUser, "Admin");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var callbackUrl = $"https://localhost:8080/login?userId={newUser.Id}&token={token}&email={newUser.Email}";

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


        [HttpPost("confirmate/email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel confirmEmailModel)
        {
            if (confirmEmailModel.UserId == null || confirmEmailModel.Token == null)
                return BadRequest(new ConfirmEmailResult { Status = false, Messages = new string[] { "Неверный token или пользователь" } });

            var user = await _userManager.FindByIdAsync(confirmEmailModel.UserId); 
            if (user == null)
                return BadRequest(new ConfirmEmailResult { Status = false, Messages = new string[] { "Пользователь не найден" } });

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailModel.Token);
            if (result.Succeeded)
                return Ok(new ConfirmEmailResult { Status = true, Messages = new string[] { "Email подтверждён" } });
            else
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new ConfirmEmailResult { Status = false, Messages = errors });
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
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return BadRequest(new ForgotPasswordResult { ShowForm = true, ShowMessages = true, Messages = new string[] { "Пользователь с таким email не существует" } });

            if (!(await _userManager.IsEmailConfirmedAsync(user)))
                return BadRequest(new ForgotPasswordResult { ShowForm = true, ShowMessages = true, Messages = new string[] { "Email не подтвержден" } });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"https://localhost:5001/resetpassword?userId={user.Id}&token={token}&email={user.Email}";

            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(model.Email, "Reset Password",
                $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>Reset password</a>");
            return Ok(new ForgotPasswordResult { ShowForm = true, ShowMessages = true, Messages = new string[] { "Ссылка для сброса пароля отправлена на email" } });
        }


        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || user.Id != model.UserId)
                return Ok(new ResetPasswordResult { Status = false,  Messages = new string[] { "Неверный email" } });
           
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            
            if (result.Succeeded)
            {
                if (await _userManager.IsLockedOutAsync(user))
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

                return Ok(new ResetPasswordResult { Status = true,  Messages = new string[] { "Пароль успешно сброшен" } });
            }
            else
            {
                List<string> Errors = new List<string>();
                foreach (var error in result.Errors)
                    Errors.Add(error.Description);

                return Ok(new ResetPasswordResult { Status = false,  Messages = Errors });
            }
        }
    }
}
