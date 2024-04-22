using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticationManager _authenticationManager => HttpContext.GetOwinContext().Authentication;
        public AccountController()
        {

        }
        
        public ActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            //TODO: Implementation of Validation
            var isValid = true;
            var identity = AdminUser(model);
            if (isValid)
            {
                var props = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    ExpiresUtc = model.RememberMe ? DateTime.UtcNow.AddMinutes(15) : DateTime.UtcNow.AddHours(10),
                    IsPersistent = true
                };
                _authenticationManager.SignIn(props, identity);
                return RedirectToLocal(returnUrl);
            }
            return View(model);
        }

        public ViewResult Logout()
        {
            _authenticationManager.SignOut();
            return View();
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Dashboard", "Home");
        }
        private ClaimsIdentity AdminUser(LoginViewModel model)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Email, model.UserName),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("RememberMe", model.RememberMe.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationType);
            return claimsIdentity;
        }
    }
}