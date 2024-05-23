using System.ComponentModel.DataAnnotations;

namespace BackCap_Logistics_FYP.Models
{
    public class Signup
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        public User user { get; set; }
    }
}
