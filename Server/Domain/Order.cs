﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Domain {
    [DataContract]
    public class Order {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public decimal Total { get; set; }
        [DataMember]
        public Customer Customer { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public List<OrderLine> Orderlines { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }

        public Order(Customer customer) {
            Customer = customer;
            Orderlines = new List<OrderLine>();
            DateCreated = DateTime.Today;
            ErrorMessage = "";
        }

        public Order() {
            Orderlines = new List<OrderLine>();
            ErrorMessage = "";
            Customer = new Customer();
        }
    }

}
