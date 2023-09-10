using GLSPM.Client.Services.Interfaces;
using GLSPM.Domain.Dtos;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GLSPM.Client.Services
{
    public class PasswordsService : IPasswordsService
    {
        private readonly HttpClient _httpClient;

        public PasswordsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public event Action PasswordsChnaged;

        public async Task ChangeLogoAsync(int id, [NotNull] IBrowserFile logo)
        {
            var content = new MultipartFormDataContent();
            //adding the logo
            var fileContent = new StreamContent(logo.OpenReadStream(IAccountsService.maxAllowedSize));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(logo.ContentType);
            content.Add(
                content: fileContent,
                name: "\"logo\"",
                fileName: logo.Name);
            //adding the key
            content.Add(
               content: new StringContent(id.ToString()),
               name: "\"key\"");

            var response = await _httpClient.PutAsync(Passwords.ChnageLogo, content);
            PasswordsChnaged?.Invoke();
        }

        public async Task<SingleObjectResponse<PasswordReadDto>> CreateAsync(PasswordCreateDto input, IBrowserFile logo)
        {
            var content = new MultipartFormDataContent();
            if (logo != null)
            {
                //adding the logo
                var fileContent = new StreamContent(logo.OpenReadStream(IAccountsService.maxAllowedSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(logo.ContentType);
                content.Add(
                    content: fileContent,
                    name: "\"logo\"",
                    fileName: logo.Name);
            }

            //adding the title
            content.Add(
               content: new StringContent(input.Title),
               name: "\"title\"");

            //adding the Username
            content.Add(
               content: new StringContent(input.Username),
               name: "\"Username\"");

            //adding the Password
            content.Add(
               content: new StringContent(input.Password),
               name: "\"Password\"");

            //adding the UserID
            content.Add(
               content: new StringContent(input.UserID),
               name: "\"UserID\"");

            if (!string.IsNullOrWhiteSpace(input.AdditionalInfo))
            {
                //adding the AdditionalInfo
                content.Add(
                   content: new StringContent(input.AdditionalInfo),
                   name: "\"AdditionalInfo\"");
            }
            if (!string.IsNullOrWhiteSpace(input.Source))
            {
                //adding the Source
                content.Add(
                   content: new StringContent(input?.Source),
                   name: "\"Source\"");
            }

            var response = await _httpClient.PostAsync(Passwords.Create, content);
            PasswordsChnaged?.Invoke();
            return await response.Content.ReadFromJsonAsync<SingleObjectResponse<PasswordReadDto>>();
        }

        public async Task DeleteAsync(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync(Passwords.Delete(id));
            PasswordsChnaged?.Invoke();

        }

        public async Task<bool> Exists(string title)
        {
            var response = await GetListAsync(new GetListDto { Filter = title });
            return response.Data.Count() == 0 || response.Data == null;
        }

        public async Task<SingleObjectResponse<string>> GeneratePassword(int length)
        {
            var response = await _httpClient.PostAsJsonAsync(Passwords.GeneratePassword, length);
            return await response.Content.ReadFromJsonAsync<SingleObjectResponse<string>>();
        }

        public async Task<SingleObjectResponse<PasswordReadDto>> GetAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<SingleObjectResponse<PasswordReadDto>>(Passwords.GetOne(id));
            PasswordsChnaged?.Invoke();
            return response;
        }

        public async Task<MultiObjectsResponse<IEnumerable<PasswordReadDto>>> GetListAsync(GetListDto input)
        {
            var response = await _httpClient.GetFromJsonAsync<MultiObjectsResponse<IEnumerable<PasswordReadDto>>>(Passwords.GetList(input));
            PasswordsChnaged?.Invoke();
            return response;
        }

        public async Task<MultiObjectsResponse<IEnumerable<PasswordReadDto>>> GetTrashedListAsync(GetListDto input)
        {
            var response = await _httpClient.GetFromJsonAsync<MultiObjectsResponse<IEnumerable<PasswordReadDto>>>(Passwords.GetTrashed(input));
            PasswordsChnaged?.Invoke();
            return response;
        }

        public async Task MoveToTrashAsync(int id)
        {
            var response = await _httpClient.PostAsync(Passwords.MoveToTrash(id), null);
            PasswordsChnaged?.Invoke();
        }

        public async Task<SingleObjectResponse<PasswordReadDto>> RestoreAsync(int id)
        {
            var response = await _httpClient.PutAsync(Passwords.Restore(id), null);
            PasswordsChnaged?.Invoke();
            return await response.Content.ReadFromJsonAsync<SingleObjectResponse<PasswordReadDto>>();
        }

        public async Task<SingleObjectResponse<PasswordReadDto>> UpdateAsync(int id, PasswordUpdateDto input)
        {
            var response = await _httpClient.PutAsJsonAsync(Passwords.Update(id), input);
            PasswordsChnaged?.Invoke();
            return await response.Content.ReadFromJsonAsync<SingleObjectResponse<PasswordReadDto>>();
        }
    }
}
