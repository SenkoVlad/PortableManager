﻿using Microsoft.AspNetCore.Components.Authorization;
using PortableManager.Shared;
using PortableManager.Shared.Models;
using PortableManager.Web.Client.Infrastructure;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PortableManager.Web.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                   AuthenticationStateProvider authenticationStateProvider,
                   ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode)
            {
                await _localStorage.SetAsync<string>("authToken", loginResult.Token);
                ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

                return loginResult;
            }
            
            return loginResult;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<RegisterResult> RegisterAsync(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync<RegisterModel>("accounts/register", registerModel);
            var registerResult = JsonSerializer.Deserialize<RegisterResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
            return registerResult;
        }

        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordModel forgotPasswordModel)
        {
            if (string.IsNullOrWhiteSpace(forgotPasswordModel.Email))
            {
                var response = await _httpClient.GetAsync("accounts/forgotpassword");
                return JsonSerializer.Deserialize<ForgotPasswordResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                var response = await _httpClient.PostAsJsonAsync<ForgotPasswordModel>("accounts/forgotpassword", forgotPasswordModel);
                return JsonSerializer.Deserialize<ForgotPasswordResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        public async Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            var response = await _httpClient.PostAsJsonAsync<ResetPasswordModel>("accounts/resetpassword", resetPasswordModel);
            return JsonSerializer.Deserialize<ResetPasswordResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ConfirmEmailResult> ConfirmateEmailAsync(ConfirmEmailModel confirmEmailModel)
        {
            var response = await _httpClient.PostAsJsonAsync<ConfirmEmailModel>("accounts/confirmate/email", confirmEmailModel);
            return JsonSerializer.Deserialize<ConfirmEmailResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
