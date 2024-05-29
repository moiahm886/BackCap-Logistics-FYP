using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class VehicleCategory
    {
        [FirestoreProperty]
        public List<String> VehicleType { get; set; }
    }
}
