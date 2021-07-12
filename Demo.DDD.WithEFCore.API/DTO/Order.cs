using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.DDD.WithEFCore.API.DTO
{
    public class Order
    {
        [Required]
        public int Id { get; set; }

        public string Note { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [MinLength(1)]
        public ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();

        [Required]
        public Address ShippingAddress { get; set; }
    }
}
