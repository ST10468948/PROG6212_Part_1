using System.Collections.Generic;

namespace RaceDay.Api.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        public ICollection<AppUser> Users { get; set; }
    }
}