﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ixq.Demo.Domain;
using Ixq.Demo.Entities;
using Ixq.Demo.Web.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Ixq.Core.DependencyInjection.Extensions;

namespace Ixq.Demo.Web.Areas.Hplus.Controllers
{
    public class AccountController : BaseController
    {
        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public ApplicationRoleManager RoleManager =>
            HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();

        public ApplicationUserManager UserManager =>
            HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        // GET: Hplus/Account
        public ActionResult Index()
        {
            var b1 = HttpContext.GetOwinContext().Get<ApplicationRoleManager>().GetHashCode();
            var b5 = HttpContext.GetOwinContext().Get<ApplicationRoleManager>().GetHashCode();
            var b2 = RoleManager.GetHashCode();

            return View();
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string userName, string password, string code, string returnUrl)
        {
            var user = UserManager.Find(userName, password);
            if (user != null)
            {
                var identity = SignInManager.CreateUserIdentity(user);
                AuthenticationManager.SignIn(identity);
                if (string.IsNullOrWhiteSpace(returnUrl))
                    return RedirectToAction("Index", "Home");
                return Redirect(returnUrl);
            }

            ViewBag.ErrorMessage = "登录失败";
            return View();

        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }

    }
}