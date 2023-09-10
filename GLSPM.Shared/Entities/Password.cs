using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GLSPM.Domain.Entities
{
    public class Password : CriticalEntityBase<int,string>
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string EncriptedPassword { get; set; }
        public string? Source { get; set; }

        //Nav Props
        public ApplicationUser User { get; set; }
    }
}
