using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GLSPM.Domain.Entities
{
    public class Card : CriticalEntityBase<int,string>
    {
        [Required]
        [StringLength(250,MinimumLength =3)]
        public string HolderName { get; set; }
        [Required]
        public string EncriptedCardNumber { get; set; }
        [Required]
        public int ExpiryMonth { get; set; }
        [Required]
        public int ExpiryYear { get; set; }
        [Required]
        public string EncriptedCVV { get; set; }

        //Nav Props
        public ApplicationUser User { get; set; }
    }
}
