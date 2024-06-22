using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shareables.API.Core;
using Shareables.API.DTO;

namespace Shareables.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenCreator _tokenCreator;

        public AuthController(JwtTokenCreator tokenCreator)
        {
            _tokenCreator = tokenCreator;
        }

        //Login route (adds the token to storage and returns it)
        //POST /api/auth
        [HttpPost]
        public IActionResult Post([FromBody] AuthRequestDto request)
        {
            string token = _tokenCreator.Create(request.Username, request.Password);

            return Ok(new AuthResponseDto { Token = token });
        }

        //Logout route (removes the token from storage)
        //DELETE /api/auth
        [Authorize]
        [HttpDelete]
        public IActionResult Delete([FromServices] ITokenStorage storage)
        {
            storage.Remove(this.Request.GetTokenId().Value);

            return NoContent();
        }
    }
}
