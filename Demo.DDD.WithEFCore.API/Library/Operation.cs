using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.API.Library
{
    public class Operation
    {
        public object Value { get; set; }

        public string Path { get; set; }

        public string Op { get; set; }
    }
}
