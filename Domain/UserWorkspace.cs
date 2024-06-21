using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class UserWorkspace
    {
        public int UserId { get; set; }
        public int WorkspaceId { get; set; }
        public int UseCaseId { get; set; }

        public User User { get; set; }
        public Workspace Workspace { get; set; }
    }
}
