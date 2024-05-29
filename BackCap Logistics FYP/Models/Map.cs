using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Map
    {
        [FirestoreProperty]
        public string SourceLatitude { get; set; }
        [FirestoreProperty]
        public string SourceLongitude { get; set; }
        [FirestoreProperty]
        public string DestinationLatitude { get; set; }
        [FirestoreProperty]
        public string DestinationLongitude { get; set; }
    }
}
