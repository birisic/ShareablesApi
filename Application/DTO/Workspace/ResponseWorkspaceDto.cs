using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Workspace
{
    public class ResponseWorkspaceDto : WorkspaceDto
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
    }
}
