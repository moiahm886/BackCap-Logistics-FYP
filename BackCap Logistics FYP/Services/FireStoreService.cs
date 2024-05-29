using BackCap_Logistics_FYP.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
namespace BackCap_Logistics_FYP.Services
{
    public class FireStoreService<T>
    {
        string projectid;
        FirestoreDb firestoreDb;

        public FireStoreService()
        {
            string filepath = "C:\\FireStoreApiKey\\swift-area-410014-firebase-adminsdk-dgcu0-3a793c1679.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectid = "swift-area-410014";
            firestoreDb = FirestoreDb.Create(projectid);
        }
        public async Task<List<T>>GetAll(string path)
        {
            try
            {
                Query query = firestoreDb.Collection(path);
                QuerySnapshot documentSnapshots = await query.GetSnapshotAsync();
                List<T> result = new List<T>();
                foreach (var documentSnapshot in documentSnapshots)
                {
                    if (documentSnapshots.Count > 0)
                    {
                        Dictionary<String, Object> dict = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(dict);
                        T t = JsonConvert.DeserializeObject<T>(json);
                        result.Add(t);
                    }
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task Add(T t,string address,string id)
        {
            try
            {
                CollectionReference collectionReference = firestoreDb.Collection(address);
                DocumentReference documentReference = collectionReference.Document(id);
                await documentReference.SetAsync(t);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding document to Firestore: {ex}");
            }
        }



        public async Task<T> Update(string id,string path)
        {
            try
            {
                DocumentReference documentReference = firestoreDb.Collection(path).Document(id);
                DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync();
                if (documentSnapshot != null)
                {
                    T t = documentSnapshot.ConvertTo<T>();
                    return t;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async void Delete(string id,string path)
        {
            DocumentReference documentReference = firestoreDb.Collection(path).Document(id);
            await documentReference.DeleteAsync();
        }
        public async Task<int> CountDocuments(string path)
        {
            try
            {
                Query query = firestoreDb.Collection(path);
                QuerySnapshot documentSnapshots = await query.GetSnapshotAsync();
                return documentSnapshots.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while counting documents in Firestore: {ex}");
                throw;
            }
        }
        public async Task<bool> DocumentExists(string documentId, string collectionPath)
        {
            try
            {
                DocumentReference documentReference = firestoreDb.Collection(collectionPath).Document(documentId);
                DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync();
                return documentSnapshot.Exists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking if document exists in Firestore: {ex}");
                throw;
            }
        }
        public async Task AddVehicleToOrganization(string organizationId, Vehicle vehicle)
        {
            try
            {
                DocumentReference organizationDocRef = firestoreDb.Collection("Organizations").Document(organizationId);
                DocumentSnapshot snapshot = await organizationDocRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    Organization organization = snapshot.ConvertTo<Organization>();

                    if (organization.Vehicles == null)
                    {
                        organization.Vehicles = new List<Vehicle>();
                    }
                    organization.Vehicles.Add(vehicle);
                    await organizationDocRef.SetAsync(organization, SetOptions.Overwrite);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the vehicle: {ex.Message}");
            }
        }
        public async Task<int> GetVehicleCount(string organizationId)
        {
            try
            {
                DocumentReference organizationDocRef = firestoreDb.Collection("Organizations").Document(organizationId);
                DocumentSnapshot snapshot = await organizationDocRef.GetSnapshotAsync();
                Organization organization = snapshot.ConvertTo<Organization>();
                int vehicleCount = organization.Vehicles?.Count ?? 0;
                return vehicleCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the vehicle count: {ex.Message}");
            }
            return -1;
        }

    }

}
