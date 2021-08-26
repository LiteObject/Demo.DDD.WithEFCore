namespace Demo.DDD.WithEFCore
{
    using AutoMapper;
    using Demo.DDD.WithEFCore.Data;
    using Demo.DDD.WithEFCore.Data.Repositories;
    using Demo.DDD.WithEFCore.Entities;
    using Demo.DDD.WithEFCore.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Polly;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    class Program
    {
        private static readonly ServiceProvider Provider;

        static Program()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Provider = serviceCollection.BuildServiceProvider();
        }

        static async Task Main(string[] args)
        {
            var context = Provider.GetRequiredService<OrderDbContext>();

            /* var testOrder = await context.Orders.FirstOrDefaultAsync(o => o.Status == Entities.Enums.OrderStatus.Created);
            testOrder.Status = Entities.Enums.OrderStatus.CancelledByBuyer;
            await context.SaveChangesAsync();*/

            /* var currentOrder = await context.Orders.Include(o => o.LineItems).FirstOrDefaultAsync();            
            PrintInfo(currentOrder.GetOrderDetails()); */

            IMapper mapper = new MapperConfiguration(cfg => { }).CreateMapper();
            var repo = new GenericRepository<Order, OrderDbContext>(context, mapper);

            // Demo how to update with OrderRepository's update method. Nested collection not updated.
            var orderWithUpdatedValues = new Order
            {
                // Id = 8,
                Note = "This is a special order + 123",
                OrderDate = DateTime.UtcNow,
                ShippingAddress = new Address("456 Street", "House #456", "Frisco", "Texas", "75033"),
                LineItems = new List<LineItem>
                    {
                    new LineItem { Name = "Apple 123", UnitPrice = 1.50, Quantity = 5 },
                    /* new LineItem { Id = 2, Name = "Orange", UnitPrice = 1.65, Quantity = 2 },
                    new LineItem { Id = 3, Name = "Mango", UnitPrice = 1.75, Quantity = 5 }, */
                }
            };

            // context.Attach(orderWithUpdatedValues).Property("Note").IsModified = true;
            context.Orders.Add(orderWithUpdatedValues);
            await context.SaveChangesAsync();

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

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<OrderDbContext>(sp =>
            {
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
                   .EnableSensitiveDataLogging()
                   .LogTo(Console.WriteLine, LogLevel.Debug)
                   .EnableDetailedErrors()
                   .UseSqlServer(
                   "Server=(LocalDb)\\MSSQLLocalDB;Database=DemoOwnedEntity;Trusted_Connection=True;MultipleActiveResultSets=true",
                   option => { option.EnableRetryOnFailure(); });

                var context = new OrderDbContext(dbContextOptionsBuilder.Options);

                // context.OnSaveEventHandlers = EntityEventHandler.OnSave;
                context.OnSaveEventHandlers += (entries) =>
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(">>>>> Delegate has been invoked on save.");
                    Console.ResetColor();
                };

                // Option #2:
                context.SavingChanges += Context_SavingChanges;

                return context;
            });
        }

        /// <summary>
        /// For more: https://docs.microsoft.com/en-us/dotnet/api/system.data.objects.objectcontext.savingchanges?view=netframework-4.8&viewFallbackFrom=net-5.0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Context_SavingChanges(object sender, SavingChangesEventArgs e)
        {
            OrderDbContext context = sender as OrderDbContext;

            if (context is not null)
            {
                var entries = context.ChangeTracker.Entries()
                    .Where(e => e.Entity is Order && e.State == EntityState.Added);

                foreach (var entry in entries)
                {
                    if ((entry.Entity as Order).LineItems.Count == 0)
                    {
                        throw new InvalidOperationException($"Atleast one {nameof(Order.LineItems)} is needed in {nameof(Order)}");
                    }
                }
            }
        }

        private static void DoSomething(int number)
        {
            Console.WriteLine($"Invoked {nameof(DoSomething)}");

            var random = new Random().Next(1, 3);

            if (random == number)
            {
                throw new InvalidOperationException("Invalid param");
            }
        }
    }
}
