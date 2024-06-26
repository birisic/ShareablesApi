using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Workspace
{
    public class WorkspaceDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Contents { get; set; }
        public int? ParentId { get; set; }
    }
}
