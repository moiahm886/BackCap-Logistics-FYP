using BackCap_Logistics_FYP.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BackCap_Logistics_FYP.Controllers
{
    public class AuthenticationController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticationController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyBoxf4FnsvVAfqYCCHx71PSUVwAJcSf7m8"));
        }
        public IActionResult SignupView()
        {
            return View();
        }
        public async Task<IActionResult> Registration(Signup signup)
        {
            try
            {
                await auth.CreateUserWithEmailAndPasswordAsync(signup.Email, signup.Password,signup.Username,true);
                ModelState.AddModelError(string.Empty, "Please Verify your email then login.");
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(signup.Email, signup.Password);
                string token = fbAuthLink.FirebaseToken;
                if (token != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("_UserToken", token);
                    return RedirectToAction("AuthenticatingEmail", new { Token = token });
                }

            }
            catch(FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return RedirectToAction("SignUpView");
            }
            return View();
        } 
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> AuthenticatingEmail(string Token)
        {
            ViewBag.federatedid = Token;
            var user = await auth.GetUserAsync(Token);
            if (!user.IsEmailVerified)
            {
                return View();
            }
            else
            {
                _httpContextAccessor.HttpContext.Session.SetString("_UserToken", Token);
                return RedirectToAction("HomePage", "Admin");
            }
        }
        public async Task<IActionResult> SendVerificationEmail(string Token)
        {
            try
            {
                if (Token != null)
                {
                    await auth.SendEmailVerificationAsync(Token);
                }
                return RedirectToAction("AuthenticatingEmail", "Authentication", Token);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> LoggingIn(Login login)
        {
            try
            {
                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(login.Email, login.Password);
                string token = fbAuthLink.FirebaseToken;
                var user = await auth.GetUserAsync(fbAuthLink.FirebaseToken);
                if (!user.IsEmailVerified)
                {
                    return RedirectToAction("AuthenticatingEmail", new {Token = token});
                }
                if (token != null)
                {   OrganizationController organizationcontroller = new OrganizationController(_httpContextAccessor);
                    _httpContextAccessor.HttpContext.Session.SetString("_UserToken", token);
                    return RedirectToAction("HomePage", "Admin");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return RedirectToAction("Login");
            }
            return View();
        }
        private string HashEmail(string email)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(email));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("Login");
        }
    }
}
