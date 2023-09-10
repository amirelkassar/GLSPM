using GLSPM.Domain;
using GLSPM.Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Dtos.Passwords
{
    public class PasswordReadDto : CriticalEntityBase<int, string>
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Source { get; set; }
    }
}
