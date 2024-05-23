using System.ComponentModel;
using System.Xml.Schema;

namespace BackCap_Logistics_FYP.Models
{
    public class Driver
    {
        public string DriverID { get; set; }
        public string Name { get; set; }
        public int ExperienceinYears { get; set; }
        public int licenseNumber { get; set; }

        public bool Verified { get; set; }
    }
}
