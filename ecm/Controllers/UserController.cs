using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ecm.Models;
using PagedList;
using PagedList.Mvc;
namespace ecm.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        private ecm.Models.ecmEntities2 db = new ecm.Models.ecmEntities2();

        public ActionResult Dashboard()
        {
            ViewBag.title = "Dashboard";
            
            return View();
        }

        public ActionResult UserInformation()
        {
            ViewBag.title = "User Information";

            string email = (string)Session["username"];
            var user = db.users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult UpdateUser(Models.user user, string PasswordNew)
        {
            if (CheckPassword(user.Password))
            {
                var crypto = new SimpleCrypto.PBKDF2();
                var encrypPass = crypto.Compute(PasswordNew);
                user.Password = encrypPass;
                user.PasswordSalt = crypto.Salt;
                user.UserType = "USER";
                string email = (string)Session["username"];
                var userr = db.users.FirstOrDefault(u => u.Email == email);
                user.UserId = userr.UserId;
                db.Entry(userr).CurrentValues.SetValues(user);
                db.SaveChanges();
                TempData["success"] = "Successfully Updated";
                return RedirectToAction("userInformation", "User");
            }
            else
            {
                TempData["error"] = "Wrong Password entered";
            }
            return RedirectToAction("userInformation", "User");
        }

        public ActionResult OrderList(int? page)
        {
            ViewBag.title = "Order List";
            return View(db.Orders.OrderBy(i => i.order_id).ToPagedList(1, 500));
        }

        public ActionResult OrderStatus()
        {
            ViewBag.title = "Order Status";
            return View();
        }

        private bool CheckPassword(string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool IsMatch = false;

            using (var db = new ecm.Models.ecmEntities2())
            {
                string email = (string)Session["username"];
                var user = db.users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    if (user.Password == crypto.Compute(password, user.PasswordSalt))
                    {
                        IsMatch = true;
                        
                    }
                }
            }
            return IsMatch;
        } 
    }
}
