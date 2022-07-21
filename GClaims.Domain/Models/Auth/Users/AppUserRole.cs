using GClaims.Core;

namespace GClaims.Domain.Models.Auth.Users
{
    public class AppUserRole : EntityBaseAudit
    {
        public string Name { get; set; }
        public List<AppUser> AppUsers { get; set; } = new List<AppUser>();

        public AppUserRole()
        {
        }
        public AppUserRole(Guid id) 
        {
            Id = id;
        }

    }

}
