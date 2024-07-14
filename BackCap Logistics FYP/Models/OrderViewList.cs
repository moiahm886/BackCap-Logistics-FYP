using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class OrderViewList
    {
        [FirestoreProperty]
        public List<Order> order { get; set; }
        [FirestoreProperty]
        public Order SingleOrder { get; set; }

    }
}
