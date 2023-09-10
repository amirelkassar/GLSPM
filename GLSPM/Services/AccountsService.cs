using Blazored.LocalStorage;
using GLSPM.Client.Services.Interfaces;
using GLSPM.Domain;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GLSPM.Client.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly HttpClient _httpClient;
        private readonly ISyncLocalStorageService _localStorageService;
        private readonly GLSPMAuthenticationStateProvider _authenticationStateProvider;

        public event Action UserLoginChange;

        public AccountsService(HttpClient httpClient,
            ISyncLocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authenticationStateProvider = (GLSPMAuthenticationStateProvider)authenticationStateProvider;
        }
        public bool IsLogged => _localStorageService.ContainKey(LocalStorageUserDataKey);

        public LoginResponseDto User => _localStorageService.GetItem<LoginResponseDto>(LocalStorageUserDataKey);

        public async Task<SingleObjectResponse<LoginResponseDto>> Login(LoginUserDto input)
        {
            var response = await _httpClient.PostAsJsonAsync(Accounts.Login, input);
            var loginData = await response.Content.ReadFromJsonAsync<SingleObjectResponse<LoginResponseDto>>();
            if (loginData != null && loginData.Success)
            {
                _localStorageService.SetItem(LocalStorageUserDataKey, loginData.Data);
                await _authenticationStateProvider.SingIn();
                UserLoginChange?.Invoke();
            }
            return loginData;
        }

        public async Task Logout()
        {
            _localStorageService.RemoveItem(LocalStorageUserDataKey);
            await _authenticationStateProvider.SignOut();
            UserLoginChange?.Invoke();

        }

        public async Task<SingleObjectResponse<object>> Register(RegisterUserDto input, IBrowserFile avatar)
        {
            var content = new MultipartFormDataContent();
            if (avatar != null)
            {
                var fileContent = new StreamContent(avatar.OpenReadStream(IAccountsService.maxAllowedSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(avatar.ContentType);
                content.Add(
                    content: fileContent,
                    name: "\"avatar\"",
                    fileName: avatar.Name);
            }

            content.Add(
                content: new StringContent(input.Username),
                name: "\"username\"");

            content.Add(
                content: new StringContent(input.Email),
                name: "\"email\"");

            content.Add(
                content: new StringContent(input.Password),
                name: "\"password\"");

            var response = await _httpClient.PostAsync(Accounts.Register, content);
            return await response.Content.ReadFromJsonAsync<SingleObjectResponse<object>>();
        }

    }
}
