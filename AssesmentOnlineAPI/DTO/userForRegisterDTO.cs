using System.ComponentModel.DataAnnotations;

namespace AssesmentOnlineAPI.DTO
{
    public class userForRegisterDTO
    {
        [Required(ErrorMessage = "Must Supply value in Username")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Username requires between 4 to 20 characters")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Must Supply value in FirstName")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "FirstName requires atleast 1 character")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Must Supply value in LastName")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "LastName requires atleast 1 character")]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 4, ErrorMessage = "Password requires atleast 4 characters")]
        public string Password { get; set; }
       
        [Range(0.0,double.MaxValue,ErrorMessage ="Initial Balance should be greater than or equal to 0")]
        public decimal InitialBalance { get; set; }
    }
}
