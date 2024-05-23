namespace BackCap_Logistics_FYP.Models
{
    public class User
    {
        public Location Address { get; set; }
        public string CNIC  { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool EmailVerified { get; set; }
        public Rating Ratings { get; set; }
        public DateTime Registration { get; set; }

    }
}
