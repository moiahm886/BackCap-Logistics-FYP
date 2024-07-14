using Google.Cloud.Firestore;
using Microsoft.Owin.BuilderProperties;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Order
    {
        [FirestoreProperty]
        public Address address { get; set; }
        [FirestoreProperty]
        public string customerId { get; set; }
        [FirestoreProperty]
        public string customerImage { get; set; }
        [FirestoreProperty]
        public string driverId { get; set; }

        [FirestoreProperty]
        public int numberOfPackage { get; set; }

        [FirestoreProperty]
        public Package package { get; set; }
        [FirestoreProperty]
        public string customerName { get; set; }
    }
}
