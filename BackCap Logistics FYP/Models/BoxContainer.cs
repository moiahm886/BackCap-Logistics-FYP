using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class BoxContainer
    {
        [FirestoreProperty]
        public double Height { get; set; }
        [FirestoreProperty]
        public double Width { get; set; }
        [FirestoreProperty]
        public double Length { get; set; }
        [FirestoreProperty]
        public double MaxWeight { get; set; }
    }
}
