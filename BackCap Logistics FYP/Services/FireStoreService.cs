using BackCap_Logistics_FYP.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Reflection;
namespace BackCap_Logistics_FYP.Services
{
    public class FireStoreService<T>
    {
        string projectid;
        FirestoreDb firestoreDb;

        public FireStoreService()
        {
            string appDirectory = Directory.GetCurrentDirectory();
            string webRootPath = Path.Combine(appDirectory, "wwwroot");
            string scriptPath = Path.Combine(webRootPath, "Script", "JSON FILE");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", scriptPath);
            projectid = "backcaps-logistics";
            firestoreDb = FirestoreDb.Create(projectid);
        }
        public async Task<List<T>> GetChat(string path,string id)
        {
            try
            {
                Query query = firestoreDb.Collection(path).Document(id).Collection("Messages");
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                List<T> result = new List<T>();
                return result;
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

        public async Task AddChat(T t, string address, string id, string timestamp)
        {
            try
            {
                CollectionReference collectionReference = firestoreDb.Collection(address).Document(id).Collection("Messages");
                DocumentReference documentReference = collectionReference.Document(timestamp);
                await documentReference.SetAsync(t);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding document to Firestore: {ex}");
            }
        }

        public async Task Update(T t, string id,string path)
        {
            try
            {
                DocumentReference docRef = firestoreDb.Collection(path).Document(id);
                await docRef.SetAsync(t);
                Console.WriteLine($"Document {id} updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating document {id}: {ex.Message}");
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
        public async Task<bool> CreateAsync(Chat chat)
        {
            try
            {
                CollectionReference collection = firestoreDb.Collection("Chats");

                Dictionary<string, object> dummyMap = new Dictionary<string, object>();
                await collection
                    .Document(chat.chatId)
                    .Collection("Messages")
                    .Document(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString())
                    .SetAsync(chat);
                await collection.Document(chat.chatId).SetAsync(dummyMap);

                Console.WriteLine("Document created successfully.");
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception in adding order to database: {exception.Message}");
                return false;
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
        public async Task<List<Vehicle>>GetVehicle(string organizationId)
        {
            try
            {
                DocumentReference organizationDocRef = firestoreDb.Collection("Organizations").Document(organizationId);
                DocumentSnapshot snapshot = await organizationDocRef.GetSnapshotAsync();
                Organization organization = snapshot.ConvertTo<Organization>();
                List<Vehicle> vehicles = organization.Vehicles?.ToList();
                return vehicles;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the vehicle count: {ex.Message}");
            }
            List<Vehicle> vehicles1 = new List<Vehicle>();
            return vehicles1;
        }
        public async Task<T> Get(string Id,string path)
        {
            try
            {
                DocumentReference organizationDocRef = firestoreDb.Collection(path).Document(Id);
                DocumentSnapshot snapshot = await organizationDocRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    T t = snapshot.ConvertTo<T>();
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

    }

}
