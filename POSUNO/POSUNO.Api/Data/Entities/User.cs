using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace POSUNO.Api.Data.Entities
{
    public class User : IdentityUser
    {

        [Required]
        [MaxLength(50)]
        public string FirsName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }



    }
}
