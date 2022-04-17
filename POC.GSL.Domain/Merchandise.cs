using POC.GSL.Infrastructure;

namespace POC.GSL.Domain
{
    public class Merchandise : DomainEntity
    {
        public string Name { get; set;}
        public string Description { get; set; }
        public string UserId { get; set; }
        public Double UnitaryValue { get; set; }
    }
}
