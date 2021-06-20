﻿using Client.ControlLayer;
using Client.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Client.Webshop.Controllers {
    public class ProductViewController : Controller {
        ProductController pc = new ProductController();
        OrderController oc = new OrderController();

        public ActionResult Index(int id) {
            //Checks if orderline session ticks has exceeded, if it has exceeded, removes orderline from session.
            long timeNow = DateTime.Now.Ticks;
            List<Orderline> orderlines = Session["cart"] as List<Orderline>;
            if (orderlines != null) {
                foreach (Orderline orderLine in orderlines.ToList<Orderline>()) {
                    if (orderLine.TimeStamp < timeNow) {
                        orderlines.Remove(orderLine);

                        oc.DeleteOrderLine(orderLine.Product.ID, orderLine.SubTotal, orderLine.Quantity);

                    }
                }
                Session["cart"] = orderlines;
            }

            Product product = pc.GetProductWithImagesAndReviews("productID", id.ToString());
            if (product.ID > 0) {
                return View(product);
            }
            else {
                return RedirectToAction("Index", "Home");
            }


        }

        
        public ActionResult AddProduct(int id, int quantity, string url) {
            Product product = pc.GetProductWithImages("productID", id.ToString());
            decimal subTotal = product.Price * quantity;
            Orderline ol = new Orderline(quantity, subTotal, product);
            Orderline errorOrderline = oc.CreateOrderLine(quantity, subTotal, product.ID);

            List<Orderline> cart;
            bool flag = true;

            if (ol != null && errorOrderline.ErrorMessage == "") {
                if (Session["cart"] == null) {
                    cart = new List<Orderline>();
                    cart.Add(ol);
                }
                else {
                    cart = (List<Orderline>)Session["cart"];

                    foreach (Orderline orderline in cart) {
                        if (orderline.Product.ID == ol.Product.ID) {
                            orderline.SubTotal += ol.SubTotal;
                            orderline.Quantity += ol.Quantity;
                            flag = false;
                        }
                    }

                    if (flag) {
                        cart.Add(ol);
                    }
                }
                Session["cart"] = cart;
            }

            if (errorOrderline.ErrorMessage == "") {
                TempData["Message"] = "Produktet blev lagt i kurven";
            }
            else {
                TempData["Message"] = errorOrderline.ErrorMessage;
            }
            

            return Redirect(url);
        }

        public ActionResult CreateReview(string reviewText, int productID, string url) {
            User user = (User)Session["user"];
            Review review = pc.CreateReview(reviewText, productID, user.ID);
            
            if(review.ErrorMessage == "") {
                TempData["ReviewMessage"] = "Tak for din anmeldelse af produktet";
            }
            else {
                TempData["ReviewMessage"] = review.ErrorMessage; 
            }
            return Redirect(url);
        }

        public ActionResult DeleteReview(int reviewID, int reviewUserID, string url) {
            User user = (User)Session["user"];
            Review r = new Review();
            if(reviewID > 0 && reviewUserID > 0) {
                r = pc.DeleteReview(reviewID, reviewUserID);
                if(r.ErrorMessage == "") {
                    TempData["DeleteReviewMessage"] = "Din anmeldelse blev slettet";
                }
                else {
                    TempData["DeleteReviewMessage"] = r.ErrorMessage;
                }
                return Redirect(url);
            }
            else {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}