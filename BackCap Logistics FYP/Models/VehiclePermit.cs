using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class VehiclePermit
    {
        [FirestoreProperty]
        public string NumberPlate  { get; set; }
        [FirestoreProperty]
        public string PermitNumber { get; set; }
    }
}
