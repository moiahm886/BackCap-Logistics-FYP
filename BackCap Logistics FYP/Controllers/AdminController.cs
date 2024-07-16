﻿using BackCap_Logistics_FYP.Models;
using BackCap_Logistics_FYP.Services;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BackCap_Logistics_FYP.Controllers
{
    public class AdminController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly IHttpContextAccessor _httpContextAccessor;
        FireStoreService<Organization> service = new FireStoreService<Organization>();
        FireStoreService<Driver> Service = new FireStoreService<Driver>();
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
                OrganizationViewModel model = new OrganizationViewModel();
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
                Organization organization = await GetOrganization(user.LocalId);
                List<Driver> driver = await Service.GetAll("Drivers");
                driver = driver.Where(d=>d.organizationId==user.LocalId).ToList();
                model.organization = organization;
                model.DriverCount = driver.Count();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Login", "Authentication");
            }
        }
        public async Task<Organization> GetOrganization(string localId)
        {
            Organization organization = await service.Get(localId, "Organizations");
            return organization;
        }
    }
}
