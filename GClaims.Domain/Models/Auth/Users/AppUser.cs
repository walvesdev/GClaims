using GClaims.Core;

namespace GClaims.Domain.Models.Auth.Users
{
    public partial class AppUser : EntityBaseAudit
    {
        #region Base properties

        public virtual string UserName { get; set; }

        public virtual string Name { get; set; }

        public virtual string Surname { get; set; }

        public virtual string Email { get; set; }

        public virtual bool EmailConfirmed { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        public virtual string Password { get; set; }

        public AppUserRole Role { get; set; }

        public Guid RoleId { get; set; }

        public Guid TenantId { get; set; }

        #endregion

        public AppUser()
        {
        }

        public AppUser(Guid id)
        {
            Id = id;
        }
    }
}