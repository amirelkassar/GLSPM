using GLSPM.Domain;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Dtos.Passwords
{
    public class PasswordCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        public IFormFile? Logo { get; set; }
        public string? AdditionalInfo { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Source { get; set; }
        [Required]
        public string UserID { get; set; }
    }
}
