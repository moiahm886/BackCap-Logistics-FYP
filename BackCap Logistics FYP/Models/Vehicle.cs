using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Vehicle
    {
        [FirestoreProperty]
        public string VehicleId { get; set; }
        [FirestoreProperty]
        public string Category { get; set; }
        [FirestoreProperty]
        public BoxContainer CaintainerCapacity { get; set; }
        [FirestoreProperty]
        public double Hp { get; set; }
        [FirestoreProperty]
        public double EngineCapacity { get; set; }
        [FirestoreProperty]
        public int Model { get; set; }
        [FirestoreProperty]
        public int  MaxSpeed { get; set; }
        [FirestoreProperty]
        public VehiclePermit Permit { get; set; }
    }
}
