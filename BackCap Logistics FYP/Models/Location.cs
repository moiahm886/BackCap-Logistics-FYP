using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Location
    {
        [FirestoreProperty]
        public double longitude { get; set; }
        [FirestoreProperty]
        public double latitude { get; set; }
        [FirestoreProperty]
        public string address { get; set; }

    }
}
