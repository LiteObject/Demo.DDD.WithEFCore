using Demo.DDD.WithEFCore.Data.Configurations;
using Demo.DDD.WithEFCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.Data
{
    /// <summary>
    /// To learn more about "Owned Types":
    /// https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities
    /// </summary>
    public class OrderDbContext : DbContext
    {
        public OrderDbContext() { }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<LineItem> LineItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(LocalDb)\\MSSQLLocalDB;Database=DemoOwnedEntity;Trusted_Connection=True;MultipleActiveResultSets=true",
                options => options.EnableRetryOnFailure()); // Connection Resiliency

            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*************************************************************************************
             * You can use the OwnsOne method in OnModelCreating or annotate the 
             * type with OwnedAttribute to configure the type as an owned type.
             *************************************************************************************/
            // modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress);

            /*************************************************************************************
             * If the ShippingAddress property is private in the Order type, 
             * you can use the string version of the OwnsOne method:
             * 
             * modelBuilder.Entity<Order>().OwnsOne(typeof(StreetAddress), "ShippingAddress");
             *************************************************************************************/

            // modelBuilder.ApplyConfiguration(new OrderConfiguration());
            // modelBuilder.ApplyConfiguration(new LineItemConfiguration());
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
