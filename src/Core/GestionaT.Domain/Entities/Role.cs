﻿using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Role : BaseEntity, ISoftDeletable
    {
        public Guid BusinessId { get; set; } // Cada negocio define sus propios roles
        public Business Business { get; set; }
        public required string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; }

        public bool IsDeleted { get; set; }
    }
}
