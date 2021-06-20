﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Domain;

namespace Server.DataAccessLayer {
    public interface IReview : ICRUD<Review> {
        Review CreateReview(Review review, int productID, int userID, bool test = false, bool testResult = false);
    }
}
