using BackCap_Logistics_FYP.Models;
using Firebase.Auth;
using FireSharp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackCap_Logistics_FYP.Controllers
{
    public class MapController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "AIzaSyBpWMFMzJrk-MYSTiFaR86L9O2C_-IhEic",
            BasePath = "https://swift-area-410014.firebaseio.com/"
        };
        IFirebaseClient client;
        FirebaseAuthProvider auth;
        public MapController(IHttpContextAccessor httpContextAccessor)
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyBoxf4FnsvVAfqYCCHx71PSUVwAJcSf7m8"));
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> DisplayMap(string sourceLatitude,string sourceLongitude,string destinationLatitude,string destinationLongitude)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            var user = await auth.GetUserAsync(check);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            Map map = new Map();
            map.SourceLatitude = sourceLatitude;
            map.SourceLongitude = sourceLongitude;
            map.DestinationLatitude = destinationLatitude;
            map.DestinationLongitude = destinationLongitude;
            return View(map);
        }
    }
}
