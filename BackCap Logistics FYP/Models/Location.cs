using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Location
    {
        [FirestoreProperty]
        public double Longitude { get; set; }
        [FirestoreProperty]
        public double Latitude { get; set; }
        [FirestoreProperty]
        public string Address { get; set; }

    }
}
