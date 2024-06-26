using Application;
using Application.DTO.User;
using Application.UseCases.Commands.User;
using Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shareables.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UseCaseHandler _useCaseHandler;
        private IApplicationActor _actor;

        public UsersController(UseCaseHandler handler, IApplicationActor actor)
        {
            _useCaseHandler = handler;
            _actor = actor;
        }


        //Register route (doesn't return token)
        //POST api/users
        [HttpPost]
        public IActionResult Post([FromBody] UserAuthRequestDto dto, [FromServices] IRegisterUserCommand cmd)
        {
            _useCaseHandler.HandleCommand(cmd, dto);
            return StatusCode(201);
        }


        //Update/Store usecases route 
        //POST api/users/usecases
        [HttpPost("usecases")]
        public IActionResult UpdateUseCase([FromBody] UserWorkspaceUseCaseDto dto, [FromServices] IUpdateUserWorkspaceUseCaseCommand cmd)
        {
            _useCaseHandler.HandleCommand(cmd, dto);

            if (dto.Action == UseCaseAction.Store.ToString())
            {
                return StatusCode(201);
            }

            if (dto.Action == UseCaseAction.Delete.ToString())
            {
                return NoContent();
            }

            return Ok();
        }


        //Test route 
        //GET api/users
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
