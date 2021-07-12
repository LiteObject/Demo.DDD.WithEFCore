using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.API.DTO
{
    public class Address
    {
        public string Street1 { get; private set; }
        public string Street2 { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Zip { get; private set; }
    }
}
