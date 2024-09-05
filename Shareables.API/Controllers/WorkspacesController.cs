using Application;
using Application.DTO.Workspace;
using Application.UseCases.Commands.Workspace;
using Application.UseCases.Queries.User;
using Application.UseCases.Queries.Workspace;
using DataAccess;
using Domain;
using Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shareables.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspacesController(UseCaseHandler handler, CustomContext context, IApplicationActor actor) : ControllerBase
    {
        public UseCaseHandler _useCaseHandler = handler;
        public CustomContext _context = context;
        public IApplicationActor _actor = actor;


        // Get Workspace Via Link Route
        // GET /api/workspaces/links/{link}
        [HttpGet("links/{link}")]
        public IActionResult Get(string link, [FromServices] IGetWorkspaceByLinkQuery query)
            => Ok(_useCaseHandler.HandleQuery(query, link));


        // Get Workspace Route
        // GET /api/workspaces/3
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id, [FromServices] IGetWorkspaceQuery query) 
            => Ok(_useCaseHandler.HandleQuery(query, id));


        // Create Workspace Route
        // POST /api/workspaces
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] WorkspaceDto dto, [FromServices] ICreateWorkspaceCommand cmd) 
        {
            _useCaseHandler.HandleCommand(cmd, dto);
            return StatusCode(201); 
        }


        // Update Workspace Route
        // PUT /api/workspaces/3
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] WorkspaceDto dto, [FromServices] IUpdateWorkspaceCommand cmd) 
        {
            dto.Id = id;
            _useCaseHandler.HandleCommand(cmd, dto);
            return NoContent();
        }


        // Destroy Workspace Route
        // DELETE /api/workspaces/3
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromBody] WorkspaceDto dto, [FromServices] IDeleteWorkspaceCommand cmd) 
        { 
            dto.Id = id; // dto should contain ParentId
            _useCaseHandler.HandleCommand(cmd, dto);
            return NoContent();
        }


        // Create Workspace Link Route
        // POST /api/workspaces/link/7
        [Authorize]
        [HttpPost("link/{workspaceId}")]
        public IActionResult Post(int workspaceId)
        {
            var workspace = _context.Workspaces.Find(workspaceId);

            if (workspace == null || workspace.Type != WorkspaceType.Document)
                return NotFound(new { message = "Document not found." });

            if (workspace.OwnerId != _actor.Id)
                return Forbid("Couldn't create a link for the provided workspace id.");

            if (workspace.Links.Any(l => l.Expires_at > DateTime.UtcNow))
                return Forbid("Couldn't create a link for the provided workspace because an active link already exists.");

            var token = GenerateRandomString(150);

            var link = new Link
            {
                Token = token,
                DocumentId = workspaceId,
                Expires_at = DateTime.UtcNow.AddMinutes(3)
            };

            _context.Links.Add(link);
            _context.SaveChanges();

            var linkUrl = $"{Request.Scheme}://{Request.Host}/api/workspaces/links/{token}";

            return StatusCode(201, new { link = linkUrl });
        }

        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
