using Microsoft.Owin.BuilderProperties;

namespace BackCap_Logistics_FYP.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public Location Source { get; set; }
        public Location Destination { get; set; }
        public DateTime EstimatedTime { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public Vehicle VehicleDetail { get; set; }
    }
}
