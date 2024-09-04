using System.Collections.Generic;

namespace Application.DTO.Workspace
{
    public class WorkspaceDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Contents { get; set; }
        public int? ParentId { get; set; }
        public IEnumerable<string> Images { get; set; }
    }
}
