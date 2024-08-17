using System.Collections.Generic;

namespace Domain
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Workspace> Workspaces { get; set; } = new HashSet<Workspace>();
        public virtual ICollection<UserWorkspace> UsersWorkspaces { get; set; } = new List<UserWorkspace>();
    }
}
