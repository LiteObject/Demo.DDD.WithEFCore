using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.API.DTO
{
    public class LineItem
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        [Required]
        public double Quantity { get; set; }
    }
}
