using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Dtos.Passwords
{
    public class PasswordUpdateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        public string? AdditionalInfo { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Source { get; set; }
    }
}
