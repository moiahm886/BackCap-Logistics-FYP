namespace BackCap_Logistics_FYP.Models
{
    public class Package
    {
        public string loadingType { get; set; }
        public BoxContainer properties { get; set; }
        public int price { get; set; }
        public bool sharedDelivery { get; set; }
        public bool status { get; set; }
        public string vehicleCategory { get; set; }
        public string vehicleId { get; set; }
    }
}
