﻿using DataAccessLayer.Interface;
using Server.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Server.DataAccessLayer {
    public class CustomerDB : ICustomer {
        private string connectionString;

        // Database test constructor. Only used for unit testing.
        public CustomerDB(string connectionString) {
            this.connectionString = connectionString;
        }

        public CustomerDB() {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }

        public Customer Create(Customer Entity, bool test = false, bool testResult = false) {
            Customer customer = new Customer();
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                try {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand()) {
                        cmd.CommandText = "Insert into Customer(FirstName, LastName, Phone, Email, Address, ZipCode, City) OUTPUT INSERTED.CustomerID values" +
                                " (@FirstName, @LastName, @Phone, @Email, @Address, @ZipCode, @City)";
                        cmd.Parameters.AddWithValue("FirstName", Entity.FirstName);
                        cmd.Parameters.AddWithValue("LastName", Entity.LastName);
                        cmd.Parameters.AddWithValue("Phone", Entity.Phone);
                        cmd.Parameters.AddWithValue("Email", Entity.Email);
                        cmd.Parameters.AddWithValue("Address", Entity.Address);
                        cmd.Parameters.AddWithValue("ZipCode", Entity.ZipCode);
                        cmd.Parameters.AddWithValue("City", Entity.City);
                        customer.ID = (int)cmd.ExecuteScalar();
                        if(customer.ID < 1) {
                            customer.ErrorMessage = "Kunden findes ikke";
                        }
                    }
                }
                catch (SqlException e) {
                    customer.ErrorMessage = ErrorHandling.Exception(e);
                }
            }
            return customer;
        }



        // Method with optimistic concurreny. If anything is changed, we rollback our transaction after trying for 4 times
        public Customer Update(Customer Entity, bool test = false, bool testResult = false) {
            Customer customer = new Customer();
            for (int i = 0; i < 5; i++) {
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction()) {
                        byte[] rowId = null;
                        int rowCount = 0;
                        using (SqlCommand cmd = connection.CreateCommand()) {
                            cmd.Transaction = trans;
                            cmd.CommandText = "SELECT rowID FROM customer WHERE customerID = @customerID";
                            cmd.Parameters.AddWithValue("customerID", Entity.ID);
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read()) {
                                rowId = (byte[])reader["rowId"];
                            }
                            reader.Close();

                            try {
                                cmd.CommandText = "UPDATE Customer SET firstName = @firstName, lastname = @lastName, phone = @phone, " +
                                    "email = @email, address = @address, zipCode = @zipCode, city = @city WHERE customerID = @customerID AND rowID = @rowId";
                                cmd.Parameters.AddWithValue("firstName", Entity.FirstName);
                                cmd.Parameters.AddWithValue("lastName", Entity.LastName);
                                cmd.Parameters.AddWithValue("phone", Entity.Phone);
                                cmd.Parameters.AddWithValue("email", Entity.Email);
                                cmd.Parameters.AddWithValue("address", Entity.Address);
                                cmd.Parameters.AddWithValue("zipCode", Entity.ZipCode);
                                cmd.Parameters.AddWithValue("city", Entity.City);
                                cmd.Parameters.AddWithValue("rowID", rowId);
                                rowCount = cmd.ExecuteNonQuery();

                                // Used to unit test. If test is true, we can set rowCount to 0 and fake a optimistic concurreny problem
                                if (test) {
                                    rowCount = testResult ? 1 : 0;
                                }

                                if (rowCount == 0) {
                                    customer.ErrorMessage = "Kunden blev ikke opdateret. Prøv igen.";
                                    cmd.Transaction.Rollback();
                                }
                                else {
                                    customer.ErrorMessage = "";
                                    cmd.Transaction.Commit();
                                    break;
                                }
                            }
                            catch (SqlException e) {
                                customer.ErrorMessage = ErrorHandling.Exception(e);
                            }
                        }
                    }
                }
            }
            return customer;
        }


        public Customer Get(string select, string input) {
            Customer c = new Customer();
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                try {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand()) {
                        cmd.CommandText = "SELECT customerID, FirstName, LastName, Phone, Email, Address, ZipCode, City" +
                            " from Customer where " + select + " = @" + select;
                        cmd.Parameters.AddWithValue(select, input);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read()) {
                            c.ID = reader.GetInt32(reader.GetOrdinal("customerID"));
                            c.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                            c.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                            c.Phone = reader.GetInt32(reader.GetOrdinal("Phone"));
                            c.Email = reader.GetString(reader.GetOrdinal("Email"));
                            c.Address = reader.GetString(reader.GetOrdinal("Address"));
                            c.ZipCode = reader.GetInt32(reader.GetOrdinal("ZipCode"));
                            c.City = reader.GetString(reader.GetOrdinal("City"));
                        }
                        reader.Close();
                        
                        if(c.ID < 1) {
                            c.ErrorMessage = "Kunden findes ikke.";
                            c.Email = "deleted user";
                        }
                    }
                }
                catch (SqlException e) {
                    c.ErrorMessage = ErrorHandling.Exception(e);
                }
            }
            return c;
        }

        public Customer Delete(Customer Entity, bool test = false, bool testResult = false) {
            throw new NotImplementedException();
        }

        public Customer Get(int id) {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> GetAll() {
            throw new NotImplementedException();
        }
    }
}


