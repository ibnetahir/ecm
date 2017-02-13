using ecm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
namespace ecm.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        ecmEntities2 db = new ecmEntities2();
        List<OrderDetail> detail = new List<OrderDetail>();
        List<Order> order = new List<Order>();
        public ActionResult Cart(int? page)
        {
            ViewBag.title = "Cart";

            if(Session["detail"] =="")
            {
                Session["detail"] = detail;
            }
            else
            {
                detail = (System.Collections.Generic.List<OrderDetail>)Session["detail"];
            }
            if(Session["order"] =="")
            {
                Session["order"] = order;
            }
            else
            {
                order = (System.Collections.Generic.List<Order>)Session["order"];
            }
            if(Session["tprice"]=="" || Session["titems"]=="0" || Session["titems"]==null || Session["tprice"]==null)
            {
                Session["order"] = ""; Session["detail"] = "";
            }
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(detail.ToPagedList(1, 500));
            //return View();
        }
        
        //
        // GET: /ShoppingCart/CartSummary
        public ActionResult addToCart(int id)
        {
            if (Session["detail"] == "")
            {
                Session["detail"] = detail;
            }
            else
            {
                detail = (System.Collections.Generic.List<OrderDetail>)Session["detail"];
            }
            if (Session["order"] == "")
            {
                Session["order"] = order;
            }
            else
            {
                order = (System.Collections.Generic.List<Order>)Session["order"];
            }
            //
            //
            string quantity = Request.Form["quanitySniper"].ToString();
            OrderDetail d = new OrderDetail();
            var data = db.items.FirstOrDefault(s => s.Id == id);
            d.name = data.name;
            d.quantity = quantity;
            d.item_id = id;
            d.price = ((data.unit_price )* (Convert.ToDouble( quantity))).ToString();
            Session["tprice"] = (Convert.ToDouble(Session["tprice"]) + Convert.ToDouble(d.price));
            Session["titems"] = (Convert.ToInt16(Session["titems"]) + 1);
            d.image = data.photo1;
            detail.Add(d);
            Session["detail"] = detail;
            //return RedirectToAction("cart");
            return View("cart",detail.ToPagedList(1,100) );
        }
        // remover from cart
        public ActionResult removeFromCart(int id)
        {
            detail = (List<OrderDetail>) Session["detail"];
            var d = detail.FirstOrDefault(s => s.item_id == id);
            detail.Remove(d);
            Session["detail"] = detail;
            Session["tprice"] = (Convert.ToDouble(Session["tprice"]) - Convert.ToDouble(d.price));
            Session["titems"] = (Convert.ToInt16(Session["titems"]) - 1);
            return View("cart",  detail.ToPagedList(1, 100));
        }
        public ActionResult CheckoutBefore()
        {
            Order o = new Order();
            o.user = Session["username"].ToString();
            o.total_price = Session["tprice"].ToString();
            o.date = DateTime.Now;
            o.status = "true";
            db.Orders.Add(o);
            db.SaveChanges();
            int id = o.order_id;
            // for order detail entering
            detail = (List<OrderDetail>)Session["detail"];
            foreach(var row in detail)
            {
                row.order_id = id;
                db.OrderDetails.Add(row);
                db.SaveChanges();
            }

            Session["order_id"] = id;
            ViewBag.title = "Authentication";
            return View("thanksfororder");
        }

        public ActionResult Step1()
        {
            ViewBag.title = "Step1";
            return View();
        }

        public ActionResult Step2()
        {
            ViewBag.title = "Step2";
            return View();
        }

        public ActionResult Step3()
        {
            ViewBag.title = "Step3";
            return View();
        }

        public ActionResult Step4()
        {
            ViewBag.title = "Step4";
            return View();
        }

        public ActionResult Step5()
        {
            ViewBag.title = "Step5";
            return View();
        }

        public ActionResult ThanksForOrder()
        {
            ViewBag.title = "Thanks For Order";
            return View();
        }

    }
}
