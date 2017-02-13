using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ecm.Models;
using System.Data.Entity.Validation;

namespace ecm.Controllers
{
    public class AuthController : Controller
    {
        //
        // GET: /Auth/
        private ecm.Models.ecmEntities2 db = new ecm.Models.ecmEntities2();
        private static bool flag;
        private static int Uid;

        public ActionResult Authentication()
        {
            ViewBag.title = "Authentication";
            if (flag)
            {
                var user = db.users.FirstOrDefault(u => u.UserId == Uid);
                return View(user);
            }
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(Models.user userr)
        {
             
            if (IsValid(userr.Email, userr.Password))
            {
                Session["username"] = userr.Email;

                var user = db.users.FirstOrDefault(u => u.UserId == Uid);

                Session["userType"] = user.UserType;
                //FormsAuthentication.SetAuthCookie(userr.Email, false);
                Response.Redirect(Request.UrlReferrer.ToString());
                return RedirectToAction("Authentication", "Auth");
            }
            else
            {
                TempData["message"] = "Invalid Username / Password";
            }
            return RedirectToAction("Authentication", "Auth");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(user user)
        {
            if (!checkUser(user.Email))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        using (var db = new ecm.Models.ecmEntities2())
                        {
                            var crypto = new SimpleCrypto.PBKDF2();
                            var encrypPass = crypto.Compute(user.Password);
                            var newUser = db.users.Create();
                            newUser.Email = user.Email;
                            newUser.Password = encrypPass;
                            newUser.PasswordSalt = crypto.Salt;
                            newUser.FirstName = user.FirstName;
                            newUser.LastName = user.LastName;
                            newUser.UserType = user.UserType;

                            db.users.Add(newUser);
                            db.SaveChanges();
                            Session["username"] = user.Email;
                            Session["userType"] = user.UserType;
                            //FormsAuthentication.SetAuthCookie(user.Email, false);
                            return RedirectToAction("dashboard", "User");
                        }
                    }
                    
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            else
            {
                TempData["error"] = "This email already exists!";
            }
            return View("Authentication");

        }

        public ActionResult LogOut()
        {

            Session.Clear();
            return RedirectToAction("Index", "Pages");
        }

        private bool IsValid(string email, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool IsValid = false;

            using (var db = new ecm.Models.ecmEntities2())
            {
                var user = db.users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    if (user.Password == crypto.Compute(password, user.PasswordSalt))
                    {
                        IsValid = true;
                        flag = true;
                        Uid = user.UserId;
                    }
                }
            }
            return IsValid;
        }

        private bool checkUser(string email)
        {
            bool IsUserExists = false;

            using (var db = new ecm.Models.ecmEntities2())
            {
                var user = db.users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    IsUserExists = true;
                }
            }
            return IsUserExists;
        } 

    }
}
