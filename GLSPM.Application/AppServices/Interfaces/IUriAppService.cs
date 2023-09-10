using GLSPM.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface IUriAppService
    {
        public Uri GetPageUri(PaginationParametersBase filter, string route);
    }
}
