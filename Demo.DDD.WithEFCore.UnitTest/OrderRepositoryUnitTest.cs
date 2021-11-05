namespace Demo.DDD.WithEFCore.UnitTest
{
    using AutoMapper;
    using Demo.DDD.WithEFCore.Data;
    using Demo.DDD.WithEFCore.Data.Repositories;
    using Demo.DDD.WithEFCore.Entities;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class OrderRepositoryUnitTest
    {
        [Fact]
        public async Task OnSave_Should_Throw_Exception() 
        { 
            // ARRANGE
            var mockOrderDbSet = new Mock<DbSet<Order>>();
            var mockContext = new Mock<OrderDbContext>();
            mockContext.Setup(m => m.Set<Order>()).Returns(mockOrderDbSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))                
                .Throws<InvalidOperationException>().Verifiable();

            var mockMapper = new Mock<IMapper>();
            var repo = new GenericRepository<Order, OrderDbContext>(mockContext.Object, mockMapper.Object);

            var newOrder = new Order();

            // ACT
            await repo.Add(newOrder);            
            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.SaveChangesAsync());

            // ASSERT
            mockContext.Verify();
        }
    }
}
