using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Drawing;
using System.Net.Mail;
using System.Net;

namespace ecm.Controllers
{
    public class PagesController : Controller
    {
        //
        // GET: /Pages/
        private ecm.Models.ecmEntities2 db = new ecm.Models.ecmEntities2();

        public ActionResult Index()
        {
            Session["detail"] = "";
            Session["order"] = "";
            ViewBag.title = "Home";
            return View(db.items.ToList());
        }

        public ActionResult AboutUs()
        {
            ViewBag.title = "About Us";
            return View();
        }

        public ActionResult ContactUs()
        {
            ViewBag.title = "Contact Us";
            return View();
        }

        public ActionResult Products(int? page, string catagory = "")
        {
            ViewBag.title = "Products";
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            if (!catagory.Equals(""))
            {
                List<Models.item> l = db.items.Where(i => i.catagory.Contains(catagory)).ToList();
                TempData["catagory"] = catagory;
                return View(l.ToPagedList(pageNumber, pageSize));
            }
            return View(db.items.OrderBy(i => i.Id).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ProductDetails(int id)
        {
            ViewBag.title = "Product Details";
            ecm.Models.item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost]
        public ActionResult sendEmail(string name, string email, string message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderMail = new MailAddress("faheemajmal2014@gmail.com", "Sender");
                    var receiverMial = new MailAddress("khaleeq419@gmail.com", "Receiver");

                    var password = "laravel6";
                    var sub = "TSHOP Mail ";
                    var body = "Name: " + name + ".\nEmail: " + email + ".\nMessage:\n\t" + message;

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderMail.Address, password)
                    };

                    using (var msg = new MailMessage(senderMail, receiverMial)
                    {
                        Subject = sub,
                        Body = body
                    })
                    {
                        smtp.Send(msg);
                    }
                    return View("aboutus");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = ("There is Some Problem In Sending Email.");
            }

            return View("aboutus");
        }


    }
}
