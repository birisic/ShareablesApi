using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Workspace
{
    public class WorkspaceUseCases
    {
        public int WorkspaceId { get; set; }
        public IEnumerable<int> UseCaseIds { get; set; }
    }
}
