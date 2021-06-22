using Demo.DDD.WithEFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Demo.DDD.WithEFCore.Data.Configurations
{
    public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            var defaultBy = "System";

            // *.HasDefaultValueSql(Database.IsSqlite() ? "datetime('now', 'utc')" : "getutcdate()")

            builder.Property(p => p.Created).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");
            builder.Property(p => p.CreatedBy).ValueGeneratedOnAdd().HasDefaultValue(defaultBy);

            builder.Property(p => p.Modified).ValueGeneratedOnUpdate().HasDefaultValueSql("getutcdate()");
            builder.Property(p => p.ModifiedBy).ValueGeneratedOnUpdate().HasDefaultValue(defaultBy);
        }
    }
}
