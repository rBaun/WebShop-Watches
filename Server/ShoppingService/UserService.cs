﻿using DataAccessLayer.Interface;
using Server.BusinessLogic;
using Server.DataAccessLayer;
using Server.Domain;

namespace Server.ServiceLayer {
    public class UserService : IUserService {
        private UserLogic userLogic;
        private IUserDB userDB;
        private CustomerLogic customerLogic;
        private CustomerDB customerDB;

        public UserService() {
            userLogic = new UserLogic();
            userDB = new UserDB();
            customerLogic = new CustomerLogic();
            customerDB = new CustomerDB();
        }

        public User GetUser(string email) {
            return userDB.GetUser("email", email);
        }

        public User GetUserWithOrders(string email) {
            return userLogic.GetUserWithOrders(email);  
        }

        public User GetUserWithOrdersAndOrderlines(string email) {
            return userLogic.GetUserWithOrdersAndOrderlines(email);
        }

        public Customer GetCustomerByMail(string email) {
            return customerDB.Get("email", email);
        }

        public Customer UpdateCustomer(string firstName, string lastName, int phone, string email, string address, int zipCode, string city, string existingemail) {
            return customerLogic.UpdateCustomer(firstName, lastName, phone, email, address, zipCode, city, existingemail);
        }

        public User DeleteUser(string email) {
            return userDB.DeleteUser(email);
        }
    }
}
