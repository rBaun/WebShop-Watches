﻿using Server.DataAccessLayer;
using Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface {
    public interface IProduct : ICRUD<Product> {
        List<Image> GetProductImages(int id);
        Product Get(string select, string input);
    }
}
