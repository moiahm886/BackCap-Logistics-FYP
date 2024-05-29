using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class OrderHistory
    {
        [FirestoreProperty]
        public List<Order> Orders { get; set; }
    }
}
