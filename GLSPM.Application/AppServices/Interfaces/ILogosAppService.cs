using GLSPM.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface ILogosAppService<TKey>
    {
        Task ChangeLogo(ChangeLogoDto<TKey> input);
        Task<string> GetLogoPathAsync(TKey id);
    }
}
