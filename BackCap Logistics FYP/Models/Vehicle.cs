namespace BackCap_Logistics_FYP.Models
{
    public class Vehicle
    {
        public string VehicleId { get; set; }
        public string Category { get; set; }
        public BoxContainer CaintainerCapacity { get; set; }
        public double Hp { get; set; }
        public double EngineCapacity { get; set; }
        public int Model { get; set; }
        public int  MaxSpeed { get; set; }
        public VehiclePermit Permit { get; set; }
    }
}
