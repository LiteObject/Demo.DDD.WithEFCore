namespace Demo.DDD.WithEFCore.UnitTest
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
    using Moq;
    using System.Linq;

    public class OrdersControllerUnitTest : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public OrdersControllerUnitTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");

        [Fact(Skip = "Testing SKIP option")]
        public void Skip_This()
        {
        }

        /// <summary>
        /// 
        /// Original Article:
        /// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-5.0
        /// </summary>
        /// <returns></returns>
        [Fact]
        [Trait("Category", "Controller")]
        public async Task Should_Return_400_BadRequest_When_ModelState_IsInvalid()
        {
            // ARRANGE
            var mockRepo = new Mock<IRepository<Order>>();
            var controller = new OrdersController(mockRepo.Object);

            // An invalid model state is tested by adding errors using AddModelError. This should produce 400 (Bad Request). 
            controller.ModelState.AddModelError(nameof(API.DTO.Order.OrderDate), "Required");

            var orderDto = new API.DTO.Order
            {
                Id = 1,
                ShippingAddress = new API.DTO.Address(),
                LineItems = new List<API.DTO.LineItem>(),
            };

            // ACT
            var result = await controller.Put(1, orderDto);

            // ASSERT
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsAssignableFrom<IActionResult>(viewResult);
        }

        [Fact]
        [Trait("Category", "Controller")]
        public async Task Should_Return_404_NotFound()
        {
            // ARRANGE
            var mockRepo = new Mock<IRepository<Order>>();
            mockRepo.Setup(repo => repo.GetAllAsync(1, 10)).Returns(Task.FromResult<List<Order>>(default));
            // mockRepo.Setup(repo => repo.GetAllAsync(1, 10)).ReturnsAsync((List<Order>)null);

            var controller = new OrdersController(mockRepo.Object);

            // ACT
            var result = await controller.Get(1, 10);

            // ASSERT
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsAssignableFrom<IActionResult>(notFoundObjectResult);
        }

        [Fact]
        [Trait("Category", "Controller")]
        public async Task Should_Return_200_Ok_With_Orders()
        {
            // ARRANGE
            var mockRepo = new Mock<IRepository<Order>>();
            var testOrders = GetTestOrders();

            //  Verifiable is to enlist a Setup into a set of "deferred Verify(...) calls"
            //  which can then be triggered via mock.Verify().
            mockRepo.Setup(repo => repo.GetAllAsync(1, 10)).ReturnsAsync(testOrders).Verifiable();

            var controller = new OrdersController(mockRepo.Object);

            // ACT
            var result = await controller.Get(1, 10);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Order>>(okResult.Value);
            mockRepo.Verify();
            var testOrder = returnValue.FirstOrDefault();
            Assert.Equal(testOrders[0].Note, testOrder.Note);
        }

        [Fact]
        [Trait("Category", "Controller")]
        public async Task Should_Throw_InvalidOperationException()
        {
            // ARRANGE
            var mockRepo = new Mock<IRepository<Order>>();
            var controller = new OrdersController(mockRepo.Object);

            var exceptionMessage = "Testing Exception";

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Get(exceptionMessage));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        public void Dispose()
        {
            _output.WriteLine("\"Dispose\" method has been invoked.");
        }


        private List<Order> GetTestOrders()
        {
            return new List<Order>
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
