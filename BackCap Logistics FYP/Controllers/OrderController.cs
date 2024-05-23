using BackCap_Logistics_FYP.Models;
using Firebase.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BackCap_Logistics_FYP.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "AIzaSyBpWMFMzJrk-MYSTiFaR86L9O2C_-IhEic",
            BasePath = "https://swift-area-410014.firebaseio.com/"
        };
        IFirebaseClient client;
        FirebaseAuthProvider auth;
        public OrderController(IHttpContextAccessor httpContextAccessor)
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
        public async Task<IActionResult> AddOrder()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            if (User.IsEmailVerified)
            {
                VehicleController controller = new VehicleController(_httpContextAccessor);
                List<Vehicle> vehicle = await controller.GetVehicle(User.LocalId);
                ViewBag.Vehicles = vehicle;
                return View();
            }
            else
            {
                return RedirectToAction("AuthenticatingEmail", "Authentication");
            }
        }
        public async Task<IActionResult> AddingOrder(Order order,string VehicleId)
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
                VehicleController controller = new VehicleController(_httpContextAccessor);
                Vehicle vehicle = await controller.GetVehicleById(VehicleId, user.LocalId);
                order.VehicleDetail = vehicle;
                await AddOrderToFirebase(order, user.LocalId,DateTime.UtcNow);
                ModelState.AddModelError(string.Empty, "Order added successfully!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("AddOrder");
        }

        private async Task<string> AddOrderToFirebase(Order order, string userId,DateTime estimatedTime)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                order.Status = "Pending";
                order.EstimatedTime=estimatedTime;

                var response = await client.PushAsync($"{userId}/orders", order);
                string orderId = response.Result.name;
                if (string.IsNullOrEmpty(orderId))
                {
                    throw new Exception("Failed to allocate unique OrganizationId.");
                }
                order.OrderId = orderId;
                var updateResponse = await client.SetAsync($"{userId}/orders/{orderId}", order);

                if (updateResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update organization with OrganizationId: " + orderId);
                }
                return orderId;
            }
        }
        private async Task<Order> GetOrderById(string orderId, string localid)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse response = await client.GetAsync($"{localid}/orders/{orderId}");

                if (response.Body != "null")
                {
                    Order order = JsonConvert.DeserializeObject<Order>(response.Body);
                    return order ;
                }
                else
                {
                    return new Order { };
                }
            }
        }


        private async Task<List<Order>> GetOrder(string localId)
        {
            using (var client = new FireSharp.FirebaseClient(config))
            {
                FirebaseResponse orderresponse = await client.GetAsync($"{localId}/orders");
                if (orderresponse.Body != "null")
                {
                    var orderList = new List<Order>();
                    var data = JsonConvert.DeserializeObject<dynamic>(orderresponse.Body);

                    foreach (var orderData in data)
                    {
                        Order order = JsonConvert.DeserializeObject<Order>(orderData.Value.ToString());
                        if (!order.Equals(null))
                        {
                            orderList.Add(order);
                        }
                    }
                    return orderList;
                }
                else
                {
                    return new List<Order>();
                }
            }
        }
        public async Task<IActionResult> DisplayOrders()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Order> orders = await GetOrder(user.LocalId);
            return View(orders);
        }

        public async Task<IActionResult> UpdatingOrder(string OrderId)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            using (var client = new FireSharp.FirebaseClient(config))
            {
                Order order = await GetOrderById(OrderId,user.LocalId);
                order.Status = "Accepted";
                string path = $"{user.LocalId}/orders/{OrderId}";
                var response = await client.SetAsync(path, order);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update order in Firebase.");
                }
            }
            return RedirectToAction("DisplayOrders");
        }

    }
}

