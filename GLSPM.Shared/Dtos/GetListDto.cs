using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Dtos
{
    public class GetListDto : PaginationParametersBase
    {
        public string Filter { get; set; }
        public string Sorting { get; set; }
        public bool IncludeDeleted { get; set; }
    }
}
