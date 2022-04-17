using POC.GSL.Infrastructure;

namespace POC.GSL.Domain
{
    public class Iventory : DomainEntity
    {
        public string MerchandiseId { get; set; }
        public string DepositId { get; set; }
        public int InventoryQuantity { get; set; }
    }
}