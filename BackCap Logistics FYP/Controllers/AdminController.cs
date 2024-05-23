using BackCap_Logistics_FYP.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BackCap_Logistics_FYP.Controllers
{
    public class AdminController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyBoxf4FnsvVAfqYCCHx71PSUVwAJcSf7m8"));
        }
        public async Task<IActionResult> HomePage()
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
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Login", "Authentication");
            }
        }
    }
}
