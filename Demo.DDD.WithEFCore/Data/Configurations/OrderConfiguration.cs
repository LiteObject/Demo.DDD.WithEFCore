using Demo.DDD.WithEFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Demo.DDD.WithEFCore.Data.Configurations
{
    public class OrderConfiguration : BaseConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.OwnsOne(x => x.ShippingAddress);
            builder.Property<DateTime>("OrderDate").IsRequired();

            // To rename column:
            builder.OwnsOne(p => p.ShippingAddress).Property(p => p.Street1).HasColumnName("Shipping_Street1");
            builder.OwnsOne(p => p.ShippingAddress).Property(p => p.Street2).HasColumnName("Shipping_Street2");
            builder.OwnsOne(p => p.ShippingAddress).Property(p => p.City).HasColumnName("Shipping_City");
            builder.OwnsOne(p => p.ShippingAddress).Property(p => p.State).HasColumnName("Shipping_State");
            builder.OwnsOne(p => p.ShippingAddress).Property(p => p.Zip).HasColumnName("Shipping_Zip");

            base.Configure(builder);
        }
    }
}
