using BackCap_Logistics_FYP.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class Organization
{ 
    [FirestoreProperty]
    public string Email { get; set; }

    [FirestoreProperty]
    public string OrganizationId { get; set; }

    [FirestoreProperty]
    public int AvailableVehicle { get; set; }

    [FirestoreProperty]
    public List<Vehicle> Vehicles { get; set; }

    [FirestoreProperty]
    public int DelayedDeliveries { get; set; }

    [FirestoreProperty]
    public string Description { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public int NumberofDrivers { get; set; }

    [FirestoreProperty]
    public int NumberofVehicle { get; set; }

    [FirestoreProperty]
    public int OnTimeDelivery { get; set; }

    [FirestoreProperty]
    public int VehicleOnRoute { get; set; }
}
