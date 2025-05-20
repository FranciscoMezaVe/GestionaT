using System.Data;
using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class User : BaseEntity
    {
        public required string Name { get; set; }
        public Guid RoleId { get; set; }
        //public Role Role { get; set; }
        public Guid BusinessId { get; set; }
        public Business Business { get; set; }
    }
}
