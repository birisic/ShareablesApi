using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.User
{
    public class UserWorkspaceUseCaseDto
    {
        public int UserId { get; set; }
        public int WorkspaceId { get; set; }
        public int UseCaseId { get; set; }
        public string Action { get; set; }
    }

    public enum UseCaseAction
    {
        Store,
        Destroy
    }
}
