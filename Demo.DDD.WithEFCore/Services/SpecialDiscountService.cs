﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.Services
{
    public class SpecialDiscountService : IDiscountService
    {
        public double Apply(double price)
        {
            if (price > 100)
            {
                return (price * 75) / 100;
            }

            return price;
        }

        public double ApplySpecialSprice(double price)
        {
            return (price * 75) / 100;
        }
    }
}
