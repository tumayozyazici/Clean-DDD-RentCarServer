using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarServer.Domain.Abstractions
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = new IdentityId(Guid.CreateVersion7());
            isActive = true;
        }
        public IdentityId Id { get; private set; }
        public bool isActive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public IdentityId CreatedBy { get; private set; } = default!;
        public DateTimeOffset? UpdatedAt { get; private set; }
        public IdentityId? UpdatedBy { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTimeOffset? DeletedAt { get; private set; }
        public IdentityId? DeletedBy { get; private set; }

        public void SetStatus(bool IsActive)
        {
            isActive = IsActive;
        }
        public void Delete()
        {
            IsDeleted = true;
        }
    }
    public sealed record IdentityId(Guid Value)
    {
        public static implicit operator Guid(IdentityId id) => id.Value;
        public static implicit operator string(IdentityId id) => id.Value.ToString();
    }
}
