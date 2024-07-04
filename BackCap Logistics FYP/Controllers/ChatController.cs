using BackCap_Logistics_FYP.Models;
using BackCap_Logistics_FYP.Services;
using Firebase.Auth;
using FireSharp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BackCap_Logistics_FYP.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        FireStoreService<Driver> service = new FireStoreService<Driver>();
        FireStoreService<Chat> Service = new FireStoreService<Chat>();
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "AIzaSyBpWMFMzJrk-MYSTiFaR86L9O2C_-IhEic",
            BasePath = "https://swift-area-410014.firebaseio.com/"
        };
        IFirebaseClient client;
        FirebaseAuthProvider auth;
        public ChatController(IHttpContextAccessor httpContextAccessor)
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
        public async Task<IActionResult> ChatView(string DriverId)
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
                List<Driver> drivers = await service.GetAll("Drivers");
                List<Chat> chats = new List<Chat>();

                if (!string.IsNullOrEmpty(DriverId))
                {
                    ViewBag.DriverId = DriverId;
                    string Id = user.LocalId + "_" + DriverId;
                    Chat chat = new Chat();
                    chats = await Service.GetChat("Chats", Id);
                    if (chats.Count==0)
                    {
                        
                        long millisecondsSinceEpoch = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        string TimeStamp = millisecondsSinceEpoch.ToString();
                        chat.text = "Hello";
                        chat.email = user.Email;
                        chat.timestamp = TimeStamp;
                        chat.chatId = Id;
                        await Service.AddChat(chat,"Chats",Id,TimeStamp);
                        
                        chats = await Service.GetChat("Chats", Id);
                    }
                }

                var model = new ChatViewModel
                {
                    Drivers = drivers,
                    Chats = chats,
                    email = user.Email,
                    localId = user.LocalId
                };

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Getchat(string AdminId,string DriverId)
        {
            string Id = AdminId + "_" + DriverId;
            List<Chat>chat = await Service.GetChat("Chats",Id);
            return Json(chat);
        }
        public async Task<IActionResult> SendChat(string AdminId,string DriverId,Chat chat)
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
            string Id = AdminId + "_" + DriverId;
            chat.email = user.Email;
            long millisecondsSinceEpoch = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string TimeStamp = millisecondsSinceEpoch.ToString();
            await Service.AddChat(chat, "Chats", Id, TimeStamp);
            return RedirectToAction("ChatView", "Chat", new { DriverId = DriverId });
        }
    }
}
