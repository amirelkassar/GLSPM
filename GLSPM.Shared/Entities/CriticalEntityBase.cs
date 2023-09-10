using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GLSPM.Domain.Entities
{
    public abstract class CriticalEntityBase<TKey,TUserID>
    {
        public CriticalEntityBase()
        {
            CreationDate = DateTime.Now;
            IsSoftDeleted = false;
        }
        [Key]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TKey ID { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Required]
        [StringLength(50,MinimumLength =2)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Title { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string? LogoPath { get; set; }
        public string? AdditionalInfo { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsSoftDeleted { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TUserID UserID { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
