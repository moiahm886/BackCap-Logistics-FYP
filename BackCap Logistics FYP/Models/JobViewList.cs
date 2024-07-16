using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class JobViewList
    {
        [FirestoreProperty]
        public Job SingleJob { get; set; }
        [FirestoreProperty]
        public List<Users> users { get; set; }
        [FirestoreProperty]
        public List<Job> jobs { get; set; }
        [FirestoreProperty]
        public string name { get; set; }
        [FirestoreProperty]
        public string email  { get; set; }
    }
}
