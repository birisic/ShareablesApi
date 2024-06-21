using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Media : NamedEntity
    {
        public string Path { get; set; }
        public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
    }
}
