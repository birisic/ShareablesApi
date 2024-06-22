using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shareables.API.Core;
using Shareables.API.DTO;
using Application.UseCases.Commands.User;
using Implementation.Validators.User;
using Application.DTO.User;
using Implementation.UseCases.Commands;
using Implementation.Validators;
using FluentValidation;
using Implementation.UseCases;

namespace Shareables.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public LoginRequestDtoValidator _loginValidator;
        public JwtTokenCreator _tokenCreator;
        public AuthController(LoginRequestDtoValidator loginValidator, JwtTokenCreator tokenCreator) 
        {
            _loginValidator = loginValidator;
            _tokenCreator = tokenCreator;
        }
        //Login route (adds the token to storage and returns it)
        //POST /api/auth
        [HttpPost]
        public IActionResult Post([FromBody] UserAuthRequestDto request)
        {
            _loginValidator.ValidateAndThrow(request);

            string token = _tokenCreator.Create(request.Username, request.Password);
            return Ok(new TokenResponseDto { Token = token });
        }

        //Logout route (removes the token from storage)
        //DELETE /api/auth
        [Authorize]
        [HttpDelete]
        public IActionResult Delete([FromServices] ITokenStorage storage)
        {
            //validate data
            storage.Remove(this.Request.GetTokenId().Value);

            return NoContent();
        }
    }
}
