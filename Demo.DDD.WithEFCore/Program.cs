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
using System.Linq;
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
                .LogTo(Console.WriteLine, LogLevel.Debug)
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

            // Demo how to update with OrderRepository's update method. Nexted collection not updated.
            var orderWithUpdatedValues = new Order
            {
                Id = 1,
                Note = "This is a special order + 456",
                OrderDate = DateTime.UtcNow,
                ShippingAddress = new Address("123 Street", "House #456", "Frisco", "Texas", "75033"),
                LineItems = new List<LineItem>
                    {
                        new LineItem { Id = 1, Name = "Apple", UnitPrice = 1.50, Quantity = 5 },
                        new LineItem { Id = 2, Name = "Orange", UnitPrice = 1.65, Quantity = 2 },
                        new LineItem { Id = 3, Name = "Mango", UnitPrice = 1.75, Quantity = 5 },
                    }
            };


            context.Orders.Update(orderWithUpdatedValues);
            context.SaveChanges();

            var orderRepo = new OrderRepository(context, mapper);
            var testOrder = (await orderRepo.FindAsync(o => o.Id == orderWithUpdatedValues.Id, o => o.LineItems)).FirstOrDefault();
            orderRepo.Update(orderWithUpdatedValues);
            var x = await orderRepo.SaveChangesAsync();
            PrintInfo(x.ToString());

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
