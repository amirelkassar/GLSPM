using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Dtos
{
    public class ChangeLogoDto<TKey>
    {
        [Required]
        public TKey Key { get; set; }
        [Required]
        public IFormFile Logo { get; set; }
    }
}
