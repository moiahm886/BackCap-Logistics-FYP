using BackCap_Logistics_FYP.Models;
using BackCap_Logistics_FYP.Services;
using Firebase.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackCap_Logistics_FYP.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        FireStoreService<Organization> service = new FireStoreService<Organization>();
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "AIzaSyBpWMFMzJrk-MYSTiFaR86L9O2C_-IhEic",
            BasePath = "https://swift-area-410014.firebaseio.com/"
        };
        IFirebaseClient client;
        FirebaseAuthProvider auth;
        public OrganizationController(IHttpContextAccessor httpContextAccessor)
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyBoxf4FnsvVAfqYCCHx71PSUVwAJcSf7m8"));
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> IsEmailVerified(string token)
        {
            var User = await auth.GetUserAsync(token);
            return User.IsEmailVerified;
        }
        public async Task<IActionResult> AddOrganization()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null) {
                return RedirectToAction("Login","Authentication");
            }
            var User = await auth.GetUserAsync(check);
            if (User.IsEmailVerified)
            {
                if (!(await DocumentExists()))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("LoggingIn", "Authentication", new { tokens = check });
                }
            }
            else
            {
                return RedirectToAction("AuthenticatingEmail", "Authentication");
            }
        }
        public async Task<IActionResult> AddingOrganization(Organization organization)
        {
            try
            {
                var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
                if (check == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }
                var user = await auth.GetUserAsync(check);
                if (user == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }
                await AddOrganizationToFirebase(organization, user.LocalId);

                ModelState.AddModelError(string.Empty, "Organization added successfully!");
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("AddOrganization");

        }

        private async Task AddOrganizationToFirebase(Organization organization, string userId)
        {
            await service.Add(organization,"Organizations",userId);
        }
        private async Task<Organization> GetOrganizationById(string organizationId,string localid)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse response = await client.GetAsync($"{localid}/organizations/{organizationId}");

                if (response.Body != "null")
                {
                    Organization organization = JsonConvert.DeserializeObject<Organization>(response.Body);
                    return organization;
                }
                else
                {
                    return new Organization { };
                }
            }
        }


        private async Task<List<Organization>> GetOrganization(string localId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse response = await client.GetAsync($"{localId}/organizations");

                if (response.Body != "null")
                {
                    var organizations = new List<Organization>();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                    var list = new List<Organization>();
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Organization>(((JProperty)item).Value.ToString()));
                    }
                    return list;
                }

                else
                {
                    return new List<Organization> { };
                }
            }
        }
        public async Task<int> GetOrganizationNumber(string localId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse response = await client.GetAsync($"{localId}/organizations");

                if (response.Body != "null")
                {
                    var organizations = new List<Organization>();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                    var list = new List<Organization>();
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Organization>(((JProperty)item).Value.ToString()));
                    }
                    return list.Count;
                }

                else
                {
                    return 0;
                }
            }
        }
        public async Task<IActionResult> DisplayOrganizations()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Organization> organization = await GetOrganization(user.LocalId);
            return View(organization);
        }
        public async Task<IActionResult> UpdateOrganization(string organizationId)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            Organization existingOrganization = await GetOrganizationById(organizationId, user.LocalId);
            if (existingOrganization == null)
            {
                return RedirectToAction("OrganizationNotFound");
            }
            return View(existingOrganization);
        }
        public async Task<IActionResult> UpdatingOrganization(Organization organization)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            using (var client = new FireSharp.FirebaseClient(config))
            {
                string path = $"{user.LocalId}/organizations/{organization.OrganizationId}";
                var response = await client.SetAsync(path, organization);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update organization in Firebase.");
                }
            }

            return RedirectToAction("UpdateOrganization", new { organization.OrganizationId });
        }
        public async Task<bool> DocumentExists()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            return await service.DocumentExists(User.LocalId, "Organizations");
        }

    }
}
