﻿using System;
using System.Collections.Generic;
using System.Configuration;
using Server.Domain;
using System.Data.SqlClient;

namespace Server.DataAccessLayer {
    public class TagDB {
        private string connectionString;
        private ProductDB productDB;

        // Database test constructor. Only used for unit testing.
        public TagDB(string connectionString) {
            this.connectionString = connectionString;
            productDB = new ProductDB(connectionString);
        }

        public TagDB() {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            productDB = new ProductDB();
        }

        // Gets a tag with products
        public Tag Get(string name) {
            Tag t = new Tag();
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                
                try {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand()) {

                        cmd.CommandText = "SELECT tagID From Tag Where Name = @Name";
                        cmd.Parameters.AddWithValue("Name", name);
                        SqlDataReader tagIDReader = cmd.ExecuteReader();
                        while (tagIDReader.Read()) {
                            t.Name = name;
                            t.ID = tagIDReader.GetInt32(tagIDReader.GetOrdinal("tagID"));
                        }
                        tagIDReader.Close();

                        cmd.CommandText = "Select productID From ProductTag Where tagID = @tagID";
                        cmd.Parameters.AddWithValue("tagID", t.ID);
                        SqlDataReader productReader = cmd.ExecuteReader();
                        while (productReader.Read()) {
                            int foundProductID;
                            foundProductID = productReader.GetInt32(productReader.GetOrdinal("productID"));

                            Product p = productDB.Get("productID", foundProductID.ToString());
                            if (p.IsActive) {
                                t.Products.Add(p);
                            }
                        }
                        productReader.Close();
                    }

                    // Error handling
                    if (t.ID < 1) {
                        t.ErrorMessage = "Kategorien findes ikke";
                    }
                }
                catch (SqlException e) {
                    t.ErrorMessage = ErrorHandling.Exception(e);
                }
            }
            return t;
        }

    }
}

