using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Dtos.Identity
{
    public class LoginResponseDto
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Token { get; set; }
        public bool IsAppAdmin { get; set; }
        public DateTime TokenExpirationDate { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
