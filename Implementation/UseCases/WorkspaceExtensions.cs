using Application;
using Application.DTO.Workspace;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Implementation.UseCases
{
    public static class WorkspaceExtensions
    {
        public static void ValidateWorkspaceName(this WorkspaceDto dto, CustomContext Context, WorkspaceType workspaceType)
        {
            // Get all workspace names with the same base name under the same parent ID
            string workspaceName = dto.Name;

            var existingNames = Context.Workspaces
            .Where(w => w.ParentId == dto.ParentId &&
                                                    w.Type == workspaceType &&
                                                    (w.Name == workspaceName || w.Name.StartsWith(workspaceName + " (")))
                                        .Select(w => w.Name)
                                        .ToList();

            if (existingNames.Contains(workspaceName))
            {
                // Extract counters from the existing names
                var counters = existingNames
                    .Select(name =>
                    {
                        var match = Regex.Match(name, @"\((\d+)\)$");
                        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
                    })
                    .ToList();

                // Find the next available counter
                int counter = counters.Count > 0 ? counters.Max() + 1 : 1;
                dto.Name = $"{workspaceName} ({counter})";
            }
        }
        
        public static bool DoAncestorWorkspacesHaveRetrieavalUseCase(this Workspace workspace, 
            CustomContext context, IApplicationActor actor)
        {
            var actorWorkspaces = actor.WorkspacesUseCases;
            var actorWorkspacesIds = actorWorkspaces.Select(wus => wus.WorkspaceId);
            var ancestorWorkspaces = context.Workspaces.Where(w => actorWorkspacesIds.Contains(w.Id))
                                                        .Select(w => new Workspace
                                                        {
                                                            Id = w.Id,
                                                            ParentId = w.ParentId
                                                        })
                                                        .ToList();

            while (workspace != null)
            {
                bool hasWorkspaceRetrievalUseCase = actorWorkspaces.Any(wus =>
                    wus.WorkspaceId == workspace.Id &&
                    wus.UseCaseIds.Contains((int)UseCasesEnum.WorkspaceRetrieval));

                if (!hasWorkspaceRetrievalUseCase) return false;

                workspace = ancestorWorkspaces.FirstOrDefault(w => w.Id == workspace.ParentId);
            }

            return true;
        }
    }
}
