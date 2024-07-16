using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Users
    {
        [FirestoreProperty]
        public string cnic { get; set; }
        [FirestoreProperty]
        public string customerID { get; set; }
        [FirestoreProperty]
        public string DriverID { get; set; }
        [FirestoreProperty]
        public string email { get; set; }
        [FirestoreProperty]
        public string license { get; set; }
        [FirestoreProperty]
        public Location location { get; set; }
        [FirestoreProperty]
        public string name { get; set; }
        [FirestoreProperty]
        public string phoneNumber { get; set; }
        [FirestoreProperty]
        public double rating { get; set; }

        [FirestoreProperty]
        public string role { get; set; }
        [FirestoreProperty]
        public string username { get; set; }
        [FirestoreProperty]
        public bool verified { get; set; }
        [FirestoreProperty]
        public string adminID { get; set; }


    }
}
