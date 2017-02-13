using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecm.Controllers
{

    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private ecm.Models.ecmEntities2 db = new ecm.Models.ecmEntities2();

        public ActionResult Dashboard()
        {
            ViewBag.title = "Dashboard";

            return View(db.items.ToList());
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
                user.UserType = "ADMIN";
                string email = (string)Session["username"];
                var userr = db.users.FirstOrDefault(u => u.Email == email);
                user.UserId = userr.UserId;
                db.Entry(userr).CurrentValues.SetValues(user);
                db.SaveChanges();
                TempData["success"] = "Successfully Updated";
                return RedirectToAction("userInformation", "Admin");
            }
            else
            {
                TempData["error"] = "Wrong Password entered";
            }
            return RedirectToAction("userInformation", "Admin");
        }

        public ActionResult Orders()
        {
            ViewBag.title = "Orders";
            return View();
        }

        public ActionResult Items(string catagory = "")
        {
            ViewBag.title = "Items";

            if (!catagory.Equals(""))
            {
               List<Models.item> l = db.items.Where(i => i.catagory.Contains(catagory)).ToList();
               return View(l);
            }

            return View(db.items.ToList());
        }

        public ActionResult AddNewItem()
        {
            ViewBag.title = "Add New Items";
            return View();
        }

        [HttpPost]
        public ActionResult AddNewItem(Models.item items)
        {
            if (!checkName2(items.name))
            {
                if (ModelState.IsValid)
                {
                    using (var db = new ecm.Models.ecmEntities2())
                    {
                        var newItem = db.items.Create();
                        newItem.catagory = items.catagory;
                        newItem.name = items.name;
                        newItem.description = items.description;
                        newItem.unit_price = items.unit_price;
                        newItem.quantity = items.quantity;

                        string[] paths = new string[3];
                        for (int i = 0; i < Request.Files.Count; i++)
			            {
			                HttpPostedFileBase file = Request.Files[i];
                            var filename = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/images/items/"), filename);
                            file.SaveAs(path);
                            paths[i] = file.FileName;
			            }

                        newItem.photo1 = "items/" + paths[0];
                        newItem.photo2 = "items/" + paths[1];
                        newItem.photo3 = "items/" + paths[2];

                        db.items.Add(newItem);
                        db.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }

                // after successfully uploading redirect the user
                return RedirectToAction("Items", "Admin");
               
            }
            else
            {
                TempData["error"] = "This Item Name already existe!";
            }

            return View();
            
        }

        public ActionResult Edit(int id)
        {
            ecm.Models.item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Models.item items, string photo_1_change, string photo_2_change, string photo_3_change)
        {

            var item = db.items.Find(items.Id);

            for (int i = 0; i < Request.Files.Count; i++)
			{
			    HttpPostedFileBase file = Request.Files[i];
                

                if (i == 0 && photo_1_change == "1")
                {
                    if (!file.FileName.Equals(""))
	                {
		                // Case 1: Check if photo 1 has value? if yes remove old photo 1 and save new photo 1
                        var filename = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/items/"), filename);
                        file.SaveAs(path);
                        items.photo1 = "items/" + filename;

                        deletePhoto(item.photo1);
	                }
                    else
                    {
                        // Case 2: if photo 1 has no value then remove the old photo 1

                        deletePhoto(item.photo1);

                    }
                }
                else if(i == 0)
                {
                    items.photo1 = item.photo1;
                }
                
                if (i == 1 && photo_2_change == "1")
                {
                    if (!file.FileName.Equals(""))
                    {
                        // Case 1: Check if photo 2 has value? if yes remove old photo 2 and save new photo 2
                        var filename = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/items/"), filename);
                        file.SaveAs(path);
                        items.photo2 = "items/" + filename;

                        deletePhoto(item.photo2);
                    }
                    else
                    {
                        // Case 2: if photo 2 has no value then remove the old photo 2
                        deletePhoto(item.photo2);
                    }
                }
                else if (i == 1)
                {
                    items.photo2 = item.photo2;
                }

                if (i == 2 && photo_3_change == "1")
                {
                    if (!file.FileName.Equals(""))
                    {
                        // Case 1: Check if photo 3 has value? if yes remove old photo 3 and save new photo 3
                        var filename = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/items/"), filename);
                        file.SaveAs(path);
                        items.photo3 = "items/" + filename;

                        deletePhoto(item.photo3);
                    }
                    else
                    {
                        // Case 2: if photo 3 has no value then remove the old photo 3
                        deletePhoto(item.photo3);
                    }
                }
                else if (i == 2)
                {
                    items.photo3 = item.photo3;
                }

               
			}

            if (Request.Files.Count == 2)
            {
                items.photo3 = item.photo3;
            }

            db.Entry(item).CurrentValues.SetValues(items);
            //db.Entry(items).State = EntityState.Modified;
            db.SaveChanges();
               
            return RedirectToAction("items");
        }

        public ActionResult Delete(int id)
        {
            ecm.Models.item item = db.items.Find(id);
            db.items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("items");
        }

        public JsonResult checkName()
        {
            string itemName = Request["itemName"];
            using (var db = new ecm.Models.ecmEntities2())
            {
                var item = db.items.FirstOrDefault(i => i.name == itemName);
                if (item != null)
                {
                    return this.Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return this.Json(false, JsonRequestBehavior.AllowGet);
        }

        public bool checkName2(string name)
        {
            string itemName = name;
            bool isValid = false;
            using (var db = new ecm.Models.ecmEntities2())
            {
                var item = db.items.FirstOrDefault(i => i.name == itemName);
                if (item != null)
                {
                    isValid = true;
                }
            }
            return isValid;
        } 

        private void deletePhoto(string path)
        {
            string fullPath = Request.MapPath("~/Images/" + path);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
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
