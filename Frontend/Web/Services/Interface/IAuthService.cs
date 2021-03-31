using PortableManager.Shared;
using PortableManager.Shared.Models;
using System.Threading.Tasks;

namespace PortableManager.Web.Client.Services
{
    public interface IAuthService
    {
        public Task<RegisterResult> RegisterAsync(RegisterModel registerModel);
        public Task<LoginResult> LoginAsync(LoginModel loginModel);
        public Task LogoutAsync();
        public Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordModel forgotPasswordModel);
        public Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordModel resetPasswordModel);
        
    }
}
