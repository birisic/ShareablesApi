using Application;
using Application.DTO.Workspace;
using Application.UseCases.Queries.Workspace;
using DataAccess;

namespace Implementation.UseCases.Queries.Workspace
{
    public class EfGetWorkspaceByLinkQuery(CustomContext context) : EfUseCase(context), IGetWorkspaceByLinkQuery
    {
        public int Id => (int)UseCasesEnum.WorkspaceRetrievalByLink;
        public UseCasesEnum Name => UseCasesEnum.WorkspaceRetrievalByLink;

        public WorkspaceDto Execute(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return new WorkspaceDto { Contents = null, Name = "Document not found." };
            }

            WorkspaceDto workspaceDto = Context.Links
                                .Where(l => l.Token == link && l.Expires_at > DateTime.UtcNow)
                                .Select(l => new WorkspaceDto { Name = l.Document.Name, Contents = l.Document.Contents })
                                .FirstOrDefault();

            if (workspaceDto == null) return new WorkspaceDto { Contents = null, Name = "Document Not Found" };

            return workspaceDto;
        }
    }
}
