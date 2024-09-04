using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Media : NamedEntity
    {
        public string Path { get; set; }
        public virtual ICollection<WorkspacesMedia> WorkspacesMedia { get; set; } = new List<WorkspacesMedia>();
    }
}
