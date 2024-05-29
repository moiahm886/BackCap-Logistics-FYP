using Google.Cloud.Firestore;
using Microsoft.Owin.BuilderProperties;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Order
    {
        [FirestoreProperty]
        public string OrderId { get; set; }
        [FirestoreProperty]
        public Location Source { get; set; }
        [FirestoreProperty]
        public Location Destination { get; set; }
        [FirestoreProperty]
        public DateTime EstimatedTime { get; set; }
        [FirestoreProperty]
        public DateTime OrderTime { get; set; }
        [FirestoreProperty]
        public string Status { get; set; }
        [FirestoreProperty]
        public Vehicle VehicleDetail { get; set; }
    }
}
