using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.DTO
{
    public class userForLoginDTO
    {
        [Required(ErrorMessage = "Must Supply value in Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Must Supply value in Password")]
        [StringLength(int.MaxValue, MinimumLength = 4, ErrorMessage = "Password requires atleast 4 characters")]
        public string Password { get; set; }
    }
}
