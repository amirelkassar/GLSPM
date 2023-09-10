using System;
using System.Collections.Generic;
using System.Text;

namespace GLSPM.Domain.Dtos.Passwords
{
    public class GeneratePasswordDto
    {
        public GeneratePasswordDto()
        {
            Length = 10;
            IncludeLowerCase = true;
            IncludeUpperCase = true;
            IncludeNumbers = true;
            IncludeSymbols = true;

        }
        public int Length { get; set; }
        public bool IncludeLowerCase { get; set; }
        public bool IncludeUpperCase { get; set; } 
        public bool IncludeNumbers { get; set; }
        public bool IncludeSymbols { get; set; }
    }
}
