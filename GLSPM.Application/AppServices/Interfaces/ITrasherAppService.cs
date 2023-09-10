using GLSPM.Application.Dtos;
using GLSPM.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface ITrasherAppService<TEntity,TKey>
    {
        Task MarkAsDeletedAsync(TKey key);
        Task<SingleObjectResponse<TEntity>> UnMarkAsDeletedAsync(TKey key);
        Task<MultiObjectsResponse<IEnumerable<TEntity>>> GetDeletedAsync(PaginationParametersBase pagination);
        Task<bool> IsDeleted(TKey key);
    }
}
