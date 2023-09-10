using GLSPM.Domain.Dtos;
using Microsoft.AspNetCore.Components.Forms;

namespace GLSPM.Client.Services.Interfaces
{
    public interface IPasswordsService
    {
        event Action PasswordsChnaged;
        Task<SingleObjectResponse<PasswordReadDto>> GetAsync(int id);
        Task<MultiObjectsResponse<IEnumerable<PasswordReadDto>>> GetListAsync(GetListDto input);
        Task<SingleObjectResponse<PasswordReadDto>> CreateAsync(PasswordCreateDto input, IBrowserFile logo);
        Task<SingleObjectResponse<PasswordReadDto>> UpdateAsync(int id,PasswordUpdateDto input);
        Task ChangeLogoAsync(int id, IBrowserFile logo);
        Task DeleteAsync(int id);
        Task MoveToTrashAsync(int id);
        Task<SingleObjectResponse<PasswordReadDto>> RestoreAsync(int id);
        Task<MultiObjectsResponse<IEnumerable<PasswordReadDto>>> GetTrashedListAsync(GetListDto input);
        Task<bool> Exists(string title);
        Task<SingleObjectResponse<string>> GeneratePassword(int length);
    }
}
