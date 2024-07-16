using Google.Cloud.Firestore;
using System.ComponentModel;
using System.Xml.Schema;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Driver
    {
        [FirestoreProperty]
        public string DriverID { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public int ExperienceinYears { get; set; }

        [FirestoreProperty]

        public bool Verified { get; set; }
    }
}
