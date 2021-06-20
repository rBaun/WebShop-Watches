﻿using Client.ControlLayer;
using Client.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Client.Webshop.Controllers {
    public class SearchController : Controller {

        //ProductController pc = new ProductController();
        TagController tc = new TagController();
        OrderController orderController = new OrderController();

        [HttpPost]
        public ActionResult Search(string searchString) {
            //Checks if orderline session ticks has exceeded, if it has exceeded, removes orderline from session.
            long timeNow = DateTime.Now.Ticks;
            List<Orderline> orderlines = Session["cart"] as List<Orderline>;
            if (orderlines != null) {
                foreach (Orderline orderLine in orderlines.ToList<Orderline>()) {
                    if (orderLine.TimeStamp < timeNow) {
                        orderlines.Remove(orderLine);

                        orderController.DeleteOrderLine(orderLine.Product.ID, orderLine.SubTotal, orderLine.Quantity);

                    }
                }
                Session["cart"] = orderlines;
            }

            Session["Search"] = searchString;
           
            Tag t = tc.FindTagByName(searchString);

            return View(t.Products);
        }
    }
}