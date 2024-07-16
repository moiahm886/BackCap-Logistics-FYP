using BackCap_Logistics_FYP.Models;
using Firebase.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BackCap_Logistics_FYP.Services;


namespace BackCap_Logistics_FYP.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private FireStoreService<Order> service = new FireStoreService<Order>();
        private FireStoreService<Job> Service = new FireStoreService<Job>();
        private FireStoreService<Users> Userservice = new FireStoreService<Users>();
        private FireStoreService<Driver> Driverservice = new FireStoreService<Driver>();
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
        public async Task<IActionResult> OrderScreen(string Id)
        {
            var model = new OrderViewList();
            try
            {
                List<Order> orders = new List<Order>();
                orders = await GetOrder();
                orders = orders.Where(o => o.status == "OrderStatus.Pending").ToList();
                Order singleorder = new Order();
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
                if (Id != null)
                {
                    ViewBag.Id = Id;
                    singleorder = orders.FirstOrDefault(o=>o.timestamp == Id);
                }
                else
                {
                    ViewBag.Id = "null";
                }
                model.order = orders;
                model.SingleOrder = singleorder;
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);  
        }
        public async Task<IActionResult> ViewResource(string Id,string names)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var model = new JobViewList();
            if (check == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            Job singlejob = new Job();

            List<Job> job = await Service.GetAll("Jobs");
            job = job.Where(j => j.jobRequestTo == user.LocalId && j.status=="Pending").ToList();
            if (Id != null && names!=null)
            {
                singlejob = job.FirstOrDefault(j => j.jobRequest == Id);
            }
            List<Users> users = new List<Users>();
            for(int i = 0; i < job.Count; i++)
            {
                Users user1 = new Users();
                user1 = await Userservice.Get(job[i].jobRequest,"Users");
                users.Add(user1);
            }
            model.jobs = job;
            model.users = users;
            model.SingleJob = singlejob;
            if (names != null)
            {
                model.name = names;
            }
            else
            {
                model.name = "-";
            }
            if (Id != null) {
                Users user2 = await Userservice.Get(Id, "Users");
                model.email = user2.email;
            }
            else
            {
                model.email= "-";
            }
            return View(model);
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
                //order.VehicleDetail = vehicle;
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
                //order.Status = "Pending";
                //order.EstimatedTime=estimatedTime;

                var response = await client.PushAsync($"{userId}/orders", order);
                string orderId = response.Result.name;
                if (string.IsNullOrEmpty(orderId))
                {
                    throw new Exception("Failed to allocate unique OrganizationId.");
                }
                //order.OrderId = orderId;
                var updateResponse = await client.SetAsync($"{userId}/orders/{orderId}", order);

                if (updateResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update organization with OrganizationId: " + orderId);
                }
                return orderId;
            }
        }
        public async Task<IActionResult> ViewOrders()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            List<Order> orders = await GetOrder();
            orders = orders.Where(o => o.status != "OrderStatus.Pending").ToList();
            return View(orders);
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


        public async Task<List<Order>> GetOrder()
        {
            List<Order> order =  await service.GetAll("Orders");
            return order;  
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
                //order.Status = "Accepted";
                string path = $"{user.LocalId}/orders/{OrderId}";
                var response = await client.SetAsync(path, order);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to update order in Firebase.");
                }
            }
            return RedirectToAction("DisplayOrders");
        }
        public async Task<IActionResult> AcceptJob(string Id)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            List<Job> job = await Service.GetAll("Jobs");
            Job singlejob = job.FirstOrDefault(j=>j.jobRequest == Id);
            if (singlejob != null)
            {
                singlejob.status = "Accepted";
            }
            Users users = await Userservice.Get(Id, "Users");
            Driver driver = new Driver();
            driver.ExperienceinYears = 1;
            driver.DriverID = Id;
            driver.Name = users.name;
            driver.Verified = true;
            await Driverservice.Add(driver, "Drivers", user.LocalId);
            await Service.Update(singlejob,singlejob.jobId,"Jobs");
            return RedirectToAction("ViewResource", "Order");
        }
        public async Task<IActionResult> RejectJob(string Id)
        {
            Job singlejob = await Service.Get(Id, "Jobs");
            singlejob.status = "Rejected";
            return RedirectToAction("ViewResource", "Order");
        }
    }

}

