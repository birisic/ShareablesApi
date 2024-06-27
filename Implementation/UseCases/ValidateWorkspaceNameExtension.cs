using Application.DTO.Workspace;
using Application.UseCases;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Implementation.UseCases
{
    public static class ValidateWorkspaceNameExtension
    {
        public static void ValidateWorkspaceName(this IUseCase cmd, WorkspaceDto dto, CustomContext Context, WorkspaceType workspaceType)
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
    }
}
