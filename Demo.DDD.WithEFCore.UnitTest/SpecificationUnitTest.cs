using AutoMapper;
using Demo.DDD.WithEFCore.Data;
using Demo.DDD.WithEFCore.Data.Repositories;
using Demo.DDD.WithEFCore.Entities;
using Demo.DDD.WithEFCore.Entities.Enums;
using Demo.DDD.WithEFCore.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Demo.DDD.WithEFCore.UnitTest
{
    public class SpecificationUnitTest
    {
        private readonly ITestOutputHelper _output;

        public SpecificationUnitTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");

        [Fact]
        public async Task TestOrdersWithLongProcessingTimeAsync()
        {
            #region ARRANGE

            var options = new DbContextOptionsBuilder<OrderDbContext>()
               .LogTo(_output.WriteLine)
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;
            using var context = new OrderDbContext(options);
            IMapper mapper = new MapperConfiguration(cfg => { }).CreateMapper();
            var repo = new GenericRepository<Order, OrderDbContext>(context, mapper);

            var orders = new List<Order>
            {
                new()
                {
                    Note = "1 day older than Today",
                    OrderDate = DateTime.Today.AddDays(-1),
                    ShippingAddress = new Address("123 Street", "House #456", "Frisco", "Texas", "75033"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.ProcessingHalted,                    
                },
                new()
                {
                    Note = "5 Days older than today",
                    OrderDate = DateTime.Today.AddDays(-5),
                    ShippingAddress = new Address("456 Street", "House #789", "Dallas", "Texas", "75248"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.ProcessingStarted
                },
                new()
                {
                    Note = "7 Days older than today",
                    OrderDate = DateTime.Today.AddDays(-7),
                    ShippingAddress = new Address("456 Street", "House #789", "Dallas", "Texas", "75248"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.ProcessingHalted
                },
                new()
                {
                    Note = "7 Days older than today",
                    OrderDate = DateTime.Today.AddDays(-7),
                    ShippingAddress = new Address("456 Street", "House #789", "Dallas", "Texas", "75248"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.Created
                }
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            #endregion SETUP

            var ordersWithLongProcessingTimeSpec = new OrdersWithLongProcessingTime();

            // ACT
            var ordersWithLongProcessingTime = await repo.FindAsync(ordersWithLongProcessingTimeSpec, o => o.LineItems);
            this._output.WriteLine($"{nameof(ordersWithLongProcessingTime)}:\n {System.Text.Json.JsonSerializer.Serialize(ordersWithLongProcessingTime)}");

            // ASSERT
            Assert.All(ordersWithLongProcessingTime, order => {
                Assert.True(order.OrderDate.AddDays(5) <= DateTime.Today);
                Assert.True(
                    order.Status == OrderStatus.ProcessingStarted ||
                    order.Status == OrderStatus.ProcessingHalted ||
                    order.Status == OrderStatus.ProcessingEnded);                
            });
        }

        private List<LineItem> GetLineItems()
        {
            return new List<LineItem>
            {
                new LineItem { Name = "Apple", UnitPrice = 0.50, Quantity = 5 },
                new LineItem { Name = "Orange", UnitPrice = 0.65, Quantity = 2 },
                new LineItem { Name = "Mango", UnitPrice = 0.75, Quantity = 5 },
            };
        }
    }
}
