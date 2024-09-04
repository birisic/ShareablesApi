using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Workspace : Entity
    {
        public string Name { get; set; } = "New Workspace";
        public WorkspaceType Type { get; set; } = WorkspaceType.Workspace;
        public string Contents { get; set; }
        public int OwnerId { get; set; }
        public int? ParentId { get; set; }

        public virtual User Owner { get; set; }
        public virtual Workspace Parent { get; set; }
        public virtual ICollection<Workspace> Children { get; set; } = new List<Workspace>();
        public virtual ICollection<WorkspacesMedia> WorkspacesMedia { get; set; } = new List<WorkspacesMedia>();
        public virtual ICollection<UserWorkspace> UsersWorkspaces { get; set; } = new List<UserWorkspace>();
        public virtual ICollection<Link> Links { get; set; } = new List<Link>();
    }

    public enum WorkspaceType
    {
        Workspace,
        Directory,
        Document
    }
}
