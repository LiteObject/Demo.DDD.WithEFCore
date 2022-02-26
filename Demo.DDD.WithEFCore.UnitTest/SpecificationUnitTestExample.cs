using Demo.DDD.WithEFCore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Demo.DDD.WithEFCore.UnitTest
{
    public class SpecificationUnitTestExample : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly OrderDbContext context;

        public SpecificationUnitTestExample(ITestOutputHelper output)
        {
            this._output = output;

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            this.context = new OrderDbContext(options);

            var orders = DataUtil.GenerateOrders();
            context.Orders.AddRange(orders);
            context.SaveChanges();

            _output.WriteLine($"Instantiated \"{nameof(SpecificationUnitTestExample)}\"");
        }

        public void TestOrdersWithLongProcessingTime()
        {
            // 
        }

        public void Dispose()
        {
            // context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
