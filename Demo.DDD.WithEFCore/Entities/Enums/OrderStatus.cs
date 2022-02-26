using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.Entities.Enums
{
    public enum OrderStatus
    {
        [Description("Order request has been accepted")]
        Created,
        [Description("Assigned to an angent for processing")]
        Assigned,

        [Description("Order is being processed")]
        ProcessingStarted,
        [Description("Order processing has been halted")]
        ProcessingHalted,
        [Description("Order has been processed and ready to be shipped")]
        ProcessingEnded,

        [Description("Order has been cancelled by the buyer")]
        CancelledByBuyer,
        [Description("Order has been cancelled by the system")]
        CancelledBySystem,

        [Description("Order has been shipped")]
        Shipped,
        [Description("Order has been received by the buyer")]
        Completed
    }
}
