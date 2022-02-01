using Demo.DDD.WithEFCore.API.Controllers;
using Demo.DDD.WithEFCore.Data.Repositories;
using Demo.DDD.WithEFCore.Entities;
using Demo.DDD.WithEFCore.Entities.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.UnitTest
{
    public class OrdersControllerFixture : IDisposable
    {
        public OrdersController ordersController { get; private set; }
        public List<Order> testData { get; set; } = default;

        public OrdersControllerFixture() 
        {
            var mockRepo = new Mock<IRepository<Order>>();
            mockRepo.Setup(repo => repo.GetAllAsync(1, 10)).Returns(Task.FromResult<List<Order>>(testData));
            ordersController = new OrdersController(mockRepo.Object);
        }

        public void Dispose()
        {            
        }
    }
}
