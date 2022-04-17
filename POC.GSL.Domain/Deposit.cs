using POC.GSL.Infrastructure;

namespace POC.GSL.Domain
{
    public class Deposit : DomainEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
    }
}
