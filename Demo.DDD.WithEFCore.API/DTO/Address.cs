using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.API.DTO
{
    public class Address
    {
        public Address()
        {
        }

        public Address(string street1, string street2, string city, string state, string zip)
        {
            Street1 = street1;
            Street2 = street2;
            City = city;
            State = state;
            Zip = zip;
        }

        public string Street1 { get; private set; }
        public string Street2 { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Zip { get; private set; }
    }
}
