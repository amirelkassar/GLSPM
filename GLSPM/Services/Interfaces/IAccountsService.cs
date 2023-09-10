using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;
using Microsoft.AspNetCore.Components.Forms;

namespace GLSPM.Client.Services.Interfaces
{
    public interface IAccountsService
    {
        const int maxAllowedSize = 2000000;//2mb
        event Action UserLoginChange;
        Task<SingleObjectResponse<LoginResponseDto>> Login(LoginUserDto input);
        Task<SingleObjectResponse<object>> Register(RegisterUserDto input,IBrowserFile avatar);
        Task Logout();
        bool IsLogged { get;}
        LoginResponseDto User { get;}

    }
}
