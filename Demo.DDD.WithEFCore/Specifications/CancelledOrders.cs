using Demo.DDD.WithEFCore.Entities;
using Demo.DDD.WithEFCore.Specifications.Core;
using System;
using System.Linq.Expressions;

namespace Demo.DDD.WithEFCore.Specifications
{
    public class CancelledOrders : Specification<Order>
    {
        public override Expression<Func<Order, bool>> ToExpression() =>
            order =>
                order.Status == Entities.Enums.OrderStatus.CancelledByBuyer ||
                order.Status == Entities.Enums.OrderStatus.CancelledBySystem;
    }
}
