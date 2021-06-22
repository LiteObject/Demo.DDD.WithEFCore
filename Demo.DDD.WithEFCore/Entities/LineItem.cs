namespace Demo.DDD.WithEFCore.Entities
{
    /// <summary>
    /// The LineItem/OrderItem - Child Entity
    /// </summary>
    public class LineItem : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double UnitPrice { get; set; }
        public double Quantity { get; set; }

        public double CalculateSubTotal() 
        { 
            return UnitPrice * Quantity;
        }
    }
}