﻿using BackCap_Logistics_FYP.Models;
using BackCap_Logistics_FYP.Services;
using Firebase.Auth;
using FireSharp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackCap_Logistics_FYP.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        FireStoreService<Users> service = new FireStoreService<Users>();
        FireStoreService<Organization> Service = new FireStoreService<Organization>();
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "AIzaSyBpWMFMzJrk-MYSTiFaR86L9O2C_-IhEic",
            BasePath = "https://swift-area-410014.firebaseio.com/"
        };
        IFirebaseClient client;
        FirebaseAuthProvider auth;
        public UserController(IHttpContextAccessor httpContextAccessor)
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
        public async Task<IActionResult> AddUser()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            if (User.IsEmailVerified)
            {
                if (await DocumentExists())
                {
                    return RedirectToAction("AddOrganization", "Organization");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AuthenticatingEmail", "Authentication");
            }
        }
        public async Task<IActionResult> AddingUser(Users users,string address)
        {
            try
            {
                var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
                if (check == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }
                var User = await auth.GetUserAsync(check);
                if (User == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }
                if (users.location == null)
                {
                    users.location = new Location();
                }
                users.location.address = address;
                users.verified = User.IsEmailVerified;
                users.email = User.Email;
                users.adminID = User.LocalId;
                users.role = "Role.Admin";
                await AddUserToFirebase(users, User.LocalId);
                ModelState.AddModelError(string.Empty, "User added successfully!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("AddUser");

        }
        public async Task<int> CountUser() {
            int count = await service.CountDocuments("Users");
            return count;
        }

        private async Task AddUserToFirebase(Users user, string userId)
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            await service.Add(user, "Users",User.LocalId);
        }
        public async Task<bool> DocumentExists()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            return await service.DocumentExists(User.LocalId, "Users");
        }
        public async Task<IActionResult> ViewUser()
        {
            var check = _httpContextAccessor.HttpContext.Session.GetString("_UserToken");
            if (check == null)
            {
                RedirectToAction("Login", "Authentication");
            }
            var User = await auth.GetUserAsync(check);
            Users user = await service.Get(User.LocalId, "Users");
            return View(user);
        }
    }
}
