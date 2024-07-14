using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class BoxContainer
    {
        [FirestoreProperty]
        public double height { get; set; }
        [FirestoreProperty]
        public double width { get; set; }
        [FirestoreProperty]
        public double length { get; set; }
        [FirestoreProperty]
        public double maxWeight { get; set; }
    }
}
