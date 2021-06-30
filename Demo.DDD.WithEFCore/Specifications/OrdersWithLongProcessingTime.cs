using Demo.DDD.WithEFCore.Entities;
using Demo.DDD.WithEFCore.Specifications.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.Specifications
{
    /// <summary>
    /// A order in a Processing status and was created 2 days ago.
    /// </summary>
    public class OrdersWithLongProcessingTime : Specification<Order>
    {        
        public override Expression<Func<Order, bool>> ToExpression() =>
           order =>
               (order.Status == Entities.Enums.OrderStatus.ProcessingStarted ||
               order.Status == Entities.Enums.OrderStatus.ProcessingHalted ||
               order.Status == Entities.Enums.OrderStatus.ProcessingEnded) &&
               order.OrderDate <= DateTime.UtcNow.AddDays(-2);
    }
}
