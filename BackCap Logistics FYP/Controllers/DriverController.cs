using BackCap_Logistics_FYP.Models;
using Firebase.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Mail;

namespace BackCap_Logistics_FYP.Controllers
{
    public class DriverController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "AIzaSyBpWMFMzJrk-MYSTiFaR86L9O2C_-IhEic",
            BasePath = "https://swift-area-410014.firebaseio.com/"
        };
        IFirebaseClient client;
        FirebaseAuthProvider auth;
        public DriverController(IHttpContextAccessor httpContextAccessor)
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
        public async Task<IActionResult> AddDriver()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            if (User.IsEmailVerified)
            {
                return View();
            }
            else
            {
                return RedirectToAction("AuthenticatingEmail", "Authentication");
            }
        }
        public async Task<IActionResult> AddingDriver(Driver driver)
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
                await AddDriverToFirebase(driver,user.LocalId);
                ModelState.AddModelError(string.Empty, "Driver added successfully!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("AddDriver");
        }

        private async Task<string> AddDriverToFirebase(Driver driver,string userId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                driver.Verified = true;
                var response = await client.PushAsync($"{userId}/drivers", driver);
                string driverId = response.Result.name;
                driver.DriverID = driverId;
                if (string.IsNullOrEmpty(driverId))
                {
                    throw new Exception("Failed to allocate unique DriverId.");
                }
                var updateResponse = await client.SetAsync($"{userId}/drivers/{driverId}", driver);

                if (updateResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update driver with DriverId: " + driverId);
                }
                return driverId;
            }
        }
        private async Task<Driver> GetDriverById(string DriverId, string localid)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse response = await client.GetAsync($"{localid}/drivers/{DriverId}");

                if (response.Body != "null")
                {
                    Driver driver = JsonConvert.DeserializeObject<Driver>(response.Body);
                    return driver;
                }
                else
                {
                    return new Driver { };
                }
            }
        }


        private async Task<List<Driver>> GetDriver(string localId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse driverresponse = await client.GetAsync($"{localId}/drivers");
                if (driverresponse.Body != "null")
                {
                    var drivers = new List<Driver>();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(driverresponse.Body);
                    var list = new List<Driver>();
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Driver>(((JProperty)item).Value.ToString()));
                    }
                    return list;
                }
                else
                {
                    return new List<Driver>();
                }
            }
        }
        public async Task<IActionResult> DisplayDrivers()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Driver> drivers = await GetDriver(user.LocalId);
            return View(drivers);
        }
        public async Task<IActionResult> UpdateDriver(string driverId)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            Driver existingDriver = await GetDriverById(driverId, user.LocalId);
            if (existingDriver == null)
            {
                return RedirectToAction("DriverNotFound");
            }
            return View(existingDriver);
        }
        public async Task<IActionResult> UpdatingDriver(Driver driver)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            using (var client = new FireSharp.FirebaseClient(config))
            {
                Driver drivers = await GetDriverById(driver.DriverID, user.LocalId);
                string path = $"{user.LocalId}/drivers/{drivers.DriverID}";
                var response = await client.SetAsync(path, driver);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update driver in Firebase.");
                }
            }
            return RedirectToAction("Displaydrivers");
        }

    }
}

