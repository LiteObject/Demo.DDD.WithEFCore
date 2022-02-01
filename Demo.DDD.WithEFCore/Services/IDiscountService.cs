using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.Services
{
    public interface IDiscountService
    {
        public double Apply(double price);
    }
}
