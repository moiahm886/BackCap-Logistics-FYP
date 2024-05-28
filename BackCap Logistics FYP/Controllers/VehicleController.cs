﻿using BackCap_Logistics_FYP.Models;
using Firebase.Auth;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BackCap_Logistics_FYP.Controllers
{
    public class VehicleController : Controller
    {
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "AIzaSyBpWMFMzJrk-MYSTiFaR86L9O2C_-IhEic",
            BasePath = "https://swift-area-410014.firebaseio.com/"
        };
        IFirebaseClient client;
        FirebaseAuthProvider auth;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VehicleController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            auth = new FirebaseAuthProvider(
                            new Firebase.Auth.FirebaseConfig("AIzaSyBoxf4FnsvVAfqYCCHx71PSUVwAJcSf7m8"));
        }
        public async Task<IActionResult> AddVehicle()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            if (User.IsEmailVerified)
            {
                Console.WriteLine(User.LocalId);
                return View();
            }
            else
            {
                return RedirectToAction("AuthenticatingEmail", "Authentication");
            }
        }
        public async Task<IActionResult> AddingVehicle(Vehicle vehicle)
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
                await AddVehicleToFirebase(vehicle, user.LocalId);

                ModelState.AddModelError(string.Empty, "Vehicle added successfully!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("AddVehicle");
        }

        private async Task<string> AddVehicleToFirebase(Vehicle vehicle, string userId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                string vehiclePath = $"{userId}/vehicles/";
                var vehicleresponse = await client.PushAsync(vehiclePath, new
                {
                    vehicle.Category,
                    vehicle.Hp,
                    vehicle.EngineCapacity,
                    vehicle.Model,
                    vehicle.MaxSpeed,
                    vehicle.CaintainerCapacity.Width,
                    vehicle.CaintainerCapacity.Height,
                    vehicle.CaintainerCapacity.Length,
                    vehicle.CaintainerCapacity.MaxWeight,
                    vehicle.Permit.NumberPlate,
                    vehicle.Permit.PermitNumber
                });
                if (vehicleresponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to add vehicle to Firebase.");
                }
                string vehicleId = vehicleresponse.Result.name;
                if (string.IsNullOrEmpty(vehicleId))
                {
                    throw new Exception("Failed to allocate unique VehicleId.");
                }
                vehicle.VehicleId = vehicleId;
                var updateResponse = await client.SetAsync($"{userId}/vehicles/{vehicleId}", vehicle);

                if (updateResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update organization with OrganizationId: " + vehicleId);
                }
                return vehicleId;
            }
        }

        public async Task<Vehicle> GetVehicleById(string vehicleId, string localid)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse response = await client.GetAsync($"{localid}/vehicles/{vehicleId}");

                if (response.Body != "null")
                {
                    Vehicle vehicle = JsonConvert.DeserializeObject<Vehicle>(response.Body);
                    return vehicle;
                }
                else
                {
                    return new Vehicle{ };
                }
            }
        }
        public async Task<List<Vehicle>> GetVehicle(string localId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse vehicleresponse = await client.GetAsync($"{localId}/vehicles");
                if (vehicleresponse.Body != "null")
                {
                    var vehicleList = new List<Vehicle>();
                    var data = JsonConvert.DeserializeObject<dynamic>(vehicleresponse.Body);

                    foreach (var vehicleData in data)
                    {
                        Vehicle vehicle = JsonConvert.DeserializeObject<Vehicle>(vehicleData.Value.ToString());
                        if (!vehicle.Equals(null))
                        {
                            vehicleList.Add(vehicle);
                        }
                    }
                    return vehicleList;
                }
                else
                {
                    return new List<Vehicle>();
                }
            }
        }

        public async Task<int> GetVehicleCount(string localId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse vehicleresponse = await client.GetAsync($"{localId}/vehicles");
                if (vehicleresponse.Body != "null")
                {
                    var vehicleList = new List<Vehicle>();
                    var data = JsonConvert.DeserializeObject<dynamic>(vehicleresponse.Body);

                    foreach (var vehicleData in data)
                    {
                        Vehicle vehicle = JsonConvert.DeserializeObject<Vehicle>(vehicleData.Value.ToString());
                        if (!vehicle.Equals(null))
                        {
                            vehicleList.Add(vehicle);
                        }
                    }
                    return vehicleList.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

            public async Task<IActionResult> DisplayVehicles()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Vehicle> vehicles = await GetVehicle(user.LocalId);
            return View(vehicles);
        }
        public async Task<IActionResult> UpdateVehicle(string vehicleId)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            Vehicle existingVehicle = await GetVehicleById(vehicleId, user.LocalId);
            if (existingVehicle == null)
            {
                return RedirectToAction("VehicleNotFound");
            }
            return View(existingVehicle);
        }
        public async Task<IActionResult> UpdatingVehicle(Vehicle vehicle)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            using (var client = new FireSharp.FirebaseClient(config))
            {
                string path = $"{user.LocalId}/vehicles/{vehicle.VehicleId}";
                var response = await client.SetAsync(path, vehicle);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update vehicle in Firebase.");
                }
            }
            return RedirectToAction("UpdateVehicle", new { vehicle.VehicleId});
        }
    }
}