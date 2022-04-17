using POC.GSL.Infrastructure;

namespace POC.GSL.Domain
{
    public class User : DomainEntity
    {
        public string Name { get; set; }
        public string Mail { get; set; }
        public string? UId { get; set; }
        public string Password { get; set; }
        public string Document { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Profile { get; set; }
    }
}
