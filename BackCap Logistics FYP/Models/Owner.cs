using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Owner
    {
        [FirestoreProperty]
        public int OwnerID { get; set; }
        [FirestoreProperty]
        public int Earning  { get; set; }
        [FirestoreProperty]
        public int Profit { get; set; }
        [FirestoreProperty]
        public bool Verified { get; set; }

    }
}
