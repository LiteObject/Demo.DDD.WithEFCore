using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.Entities
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets Id - The primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or set Created date. This value should be in UTC.
        /// </summary>
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or set Modified date. This value should be in UTC.
        /// </summary>
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; }
    }
}
