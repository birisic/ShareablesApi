using Application.DTO.Workspace;
using Application.UseCases.Commands.Workspace;
using Application.UseCases.Queries.User;
using Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shareables.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspacesController : ControllerBase
    {
        public UseCaseHandler _useCaseHandler;
        public WorkspacesController(UseCaseHandler handler) 
        {
            _useCaseHandler = handler;
        }

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
        public IActionResult Post([FromBody] WorkspaceDto data, [FromServices] ICreateWorkspaceCommand cmd) 
        {
            _useCaseHandler.HandleCommand(cmd, data);
            return StatusCode(201); 
        }

        // Update Workspace Route
        // PUT /api/workspaces/3
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id) { return Ok(); }


        // Delete Workspace Route
        // DELETE /api/workspaces/3
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) { return Ok(); }

    }
}
