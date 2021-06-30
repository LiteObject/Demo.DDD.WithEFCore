using AutoMapper;
using Demo.DDD.WithEFCore.Data;
using Demo.DDD.WithEFCore.Data.Repositories;
using Demo.DDD.WithEFCore.Entities;
using Demo.DDD.WithEFCore.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static async Task Main(string[] args)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Warning)
                .EnableDetailedErrors()
                .UseSqlServer(
                "Server=(LocalDb)\\MSSQLLocalDB;Database=DemoOwnedEntity;Trusted_Connection=True;MultipleActiveResultSets=true",
                option => { option.EnableRetryOnFailure(); });

            using var context = new OrderDbContext(dbContextOptionsBuilder.Options);

            /* var testOrder = await context.Orders.FirstOrDefaultAsync(o => o.Status == Entities.Enums.OrderStatus.Created);
            testOrder.Status = Entities.Enums.OrderStatus.CancelledByBuyer;
            await context.SaveChangesAsync();*/            

            /* var currentOrder = await context.Orders.Include(o => o.LineItems).FirstOrDefaultAsync();            
            PrintInfo(currentOrder.GetOrderDetails()); */

            IMapper mapper = new MapperConfiguration(cfg => { }).CreateMapper();
            var repo = new GenericRepository<Order, OrderDbContext>(context, mapper);

            var cancelledOrders = new CancelledOrders();
            var ordersWithLongProcessingTime = new OrdersWithLongProcessingTime();

            var troubledOrders = await repo.FindAsync(cancelledOrders.Or(ordersWithLongProcessingTime), o => o.LineItems);

            troubledOrders.ForEach(o => PrintInfo(o.GetOrderDetails()));
        }

        private static void PrintInfo(string value) 
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        private static async Task SeedDatabase(OrderDbContext context) 
        {
            var orders = DataUtil.GenerateOrders();
            await context.Orders.AddRangeAsync(orders);
            int count = await context.SaveChangesAsync();
        }
    }
}
