namespace Demo.DDD.WithEFCore.IntegrationTest
{
    using AutoMapper;
    using Demo.DDD.WithEFCore.API.Controllers;
    using Demo.DDD.WithEFCore.Data;
    using Demo.DDD.WithEFCore.Data.Repositories;
    using Demo.DDD.WithEFCore.Entities;
    using Demo.DDD.WithEFCore.Entities.Enums;
    using Demo.DDD.WithEFCore.Specifications;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class OrdersControllerTest
    {
        private readonly ITestOutputHelper _output;

        public OrdersControllerTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");

        /// <summary>
        /// For more examples: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-5.0
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnOk()
        {
            // Overwhelming details - not readable

            #region arrange

            // ARRANGE
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

            var pageNumber = 1;
            var pageSize = 2;

            var controller = new OrdersController(repo);

            #endregion arrange

            // ACT
            var result = await controller.Get(pageNumber, pageSize);

            // ASSERT
            var viewResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IActionResult>(viewResult);

            var model = Assert.IsAssignableFrom<List<Order>>(viewResult.Value);
            Assert.NotEmpty(model);
            Assert.True(orders.Count > pageSize);
            Assert.Equal(pageSize, model.Count);
        }

        private List<LineItem> GetLineItems()
        {
            return new List<LineItem>
            {
                new LineItem { Name = "Apple", UnitPrice = 1.50, Quantity = 5 },
                new LineItem { Name = "Orange", UnitPrice = 1.65, Quantity = 2 },
                new LineItem { Name = "Mango", UnitPrice = 1.75, Quantity = 5 },
            };
        }
    }
}
