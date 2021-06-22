using Demo.DDD.WithEFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.Data.Configurations
{
    public class LineItemConfiguration : BaseConfiguration<LineItem>
    {
        public override void Configure(EntityTypeBuilder<LineItem> builder)
        {
            builder.Property(p => p.UnitPrice).HasDefaultValue(0.01);
            builder.Property(p => p.Quantity).HasDefaultValue(1);

            base.Configure(builder);
        }
    }
}
