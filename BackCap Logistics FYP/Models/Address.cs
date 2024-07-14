using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Address
    {
        [FirestoreProperty]
        public Location to { get; set; }
        [FirestoreProperty]
        public Location from { get; set; }
    }
}
