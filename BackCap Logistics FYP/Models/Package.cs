﻿using Google.Cloud.Firestore;

namespace BackCap_Logistics_FYP.Models
{
    [FirestoreData]
    public class Package
    {
        [FirestoreProperty]
        public string loadingType { get; set; }
        [FirestoreProperty]
        public BoxContainer properties { get; set; }
 
      
    }
}