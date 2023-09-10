using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Passwords;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Passwords;
using GLSPM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface IPasswordsAppService : IAppService<Password,int,PasswordReadDto,PasswordCreateDto,PasswordUpdateDto>, ITrasherAppService<PasswordReadDto, int>, ILogosAppService<int>
    {
        Task<SingleObjectResponse<string>> GeneratePassword(int length);
    }
}
