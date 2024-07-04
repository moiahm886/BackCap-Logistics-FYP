using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Chat
    {
        [FirestoreProperty]
        public string chatId { get; set; }
        [FirestoreProperty]
        public string text { get; set; }
        [FirestoreProperty]
        public string email { get; set; }
        [FirestoreProperty]
        public string timestamp { get; set; }
    }
}
