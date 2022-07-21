using GClaims.Core;

namespace GClaims.Domain.Models.Auth.Users
{
    public class AppTenant : EntityBase
    {
        public AppTenant()
        {
        }

        public AppTenant(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public virtual string Name
        {
            get;
            set;
        }
    }
}
