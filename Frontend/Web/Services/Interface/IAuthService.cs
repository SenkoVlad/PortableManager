using PortableManager.Shared;
using System.Threading.Tasks;

namespace PortableManager.Web.Client.Services
{
    interface IAuthService
    {
        public Task<RegisterResult> RegisterAsync(RegisterModel registerModel);
        public Task<LoginResult> LoginAsync(LoginModel loginModel);
        public Task LogoutAsync();
    }
}
