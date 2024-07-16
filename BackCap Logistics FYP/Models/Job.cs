using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Job
    {
        [FirestoreProperty]
        public string jobId { get; set; }
        [FirestoreProperty]
        public string jobRequest { get; set; }
        [FirestoreProperty]
        public string jobRequestTo { get; set; }    
        [FirestoreProperty]
        public string jobType  { get; set; }
        [FirestoreProperty]
        public string startDate { get; set; }
        [FirestoreProperty]
        public string status { get; set; }
    }
}
