using Application.DTO.User;
using Application.UseCases.Commands.User;
using Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shareables.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UseCaseHandler _useCaseHandler;

        public UsersController(UseCaseHandler handler)
        {
            _useCaseHandler = handler;
        }


        //Register route (doesn't return token)
        //POST api/users
        [HttpPost]
        public IActionResult Post([FromBody] RegisterUserDto dto, [FromServices] IRegisterUserCommand cmd)
        {
            _useCaseHandler.HandleCommand(cmd, dto);
            return StatusCode(201);
        }
    }
}
