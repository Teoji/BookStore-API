using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.DTOs
{
    public class UserDTO
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage ="Password is limites from {2} to {1}"), MinLength (6)]
        public string Password { get; set; }

    }
}
