using Google.Cloud.Firestore;
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
        public async Task Add(T t)
        {
            try
            {
                CollectionReference collectionReference = firestoreDb.Collection("Organizations");
                DocumentReference documentReference = await collectionReference.AddAsync(t);
                string documentId = documentReference.Id;
                PropertyInfo property = typeof(T).GetProperty("OrganizationId");
                if (property != null && property.CanWrite)
                {
                    property.SetValue(t, documentId);
                }
                await documentReference.UpdateAsync("OrganizationId", documentId);
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
    }
}
