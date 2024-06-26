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
