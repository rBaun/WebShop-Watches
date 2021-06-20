﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.DataAccessLayer;
using Server.BusinessLogic;
using Server.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace UnitTest {
    [TestClass]
    public class OrderTest {
        private static string connectionString = "Server=kraka.ucn.dk; Database=dmab0917_1067354; User Id=dmab0917_1067354; Password=Password1! ";
        private OrderDB orderDB;
        private OrderLogic orderLogic;
        private CustomerDB customerDB;
        private ProductDB productDB;

        [TestInitialize]
        public void SetUp() {
            orderDB = new OrderDB(connectionString);
            productDB = new ProductDB(connectionString);
            customerDB = new CustomerDB(connectionString);
            orderLogic = new OrderLogic(connectionString);
        }

        [TestMethod]
        public void CreateOrderWithReturnedPrimaryKey() {
            Customer c = customerDB.Get("email", "Morten@hotmail.com");
            Order o = new Order(c);
            o.Orderlines.Add(new OrderLine(2, 200, productDB.Get("productID", 1.ToString())));

            o = orderDB.Create(o);

            Assert.IsTrue(o.ID > 0);
        }

        [TestMethod]
        public void ValidateOrderLinePricesToPreventInsecureDirectObjectReference() {
            Product p = productDB.Get("productID", 1.ToString());
            //Price on OrderLine is incorrect and should be caught
            OrderLine ol = new OrderLine(1, 100, p);
            List<OrderLine> orderLines = new List<OrderLine>();
            orderLines.Add(ol);

            Order o = orderLogic.CreateOrder("Rune", "Andersen", "Istedgade 10", 9000, "Aalborg", "denbedste@gmail.com", 12341234, orderLines);


            Assert.AreEqual(o.ErrorMessage, "");

        }

        [TestMethod]
        public void FindOrder() {

            Order o = orderLogic.GetOrder(1);

            Assert.IsNotNull(o);
        }

        [TestMethod]
        public void  DeleteOrder() {

            Customer c = customerDB.Get("email", "Morten@hotmail.com");
            Order o = new Order(c);
            o.Orderlines.Add(new OrderLine(2, 200, productDB.Get("productID", 1.ToString())));

            o = orderDB.Create(o);

            Order order = orderDB.Delete(o, true, true);

            Assert.AreEqual(order.ErrorMessage, "");
        }



    }
}