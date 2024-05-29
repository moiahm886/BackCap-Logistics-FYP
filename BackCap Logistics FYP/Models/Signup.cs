using System.ComponentModel.DataAnnotations;

namespace BackCap_Logistics_FYP.Models
{
    public class Signup
    {

        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
