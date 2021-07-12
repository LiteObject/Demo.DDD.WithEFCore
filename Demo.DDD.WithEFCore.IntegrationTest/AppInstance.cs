using Demo.DDD.WithEFCore.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.IntegrationTest
{
    public class AppInstance: WebApplicationFactory<Startup>
    {
    }
}
