using POC.GSL.Infrastructure;

namespace POC.GSL.Domain
{
    public class Order: DomainEntity
    {
        public string CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<OrderItem>? OrderItem { get; set; }

    }
}
