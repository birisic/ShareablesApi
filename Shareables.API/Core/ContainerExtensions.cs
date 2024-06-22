using Application;
using Application.UseCases.Commands.User;
using Implementation;
using Implementation.Logging;
using Implementation.UseCases.Commands.User;
using Implementation.Validators.User;
using System.IdentityModel.Tokens.Jwt;

namespace Shareables.API.Core
{
    public static class ContainerExtensions
    {
        public static void AddUseCases(this IServiceCollection services)
        {
            //services.AddTransient<UpdateUserAccessDtoValidator>();
            //services.AddTransient<IUpdateUseAccessCommand, EfUpdateUserAccessCommand>();
            services.AddTransient<UseCaseHandler>();
            services.AddTransient<IRegisterUserCommand, EfRegisterUserCommand>();
            services.AddTransient<IUseCaseLogger, EfUseCaseLogger>();
            services.AddTransient<RegisterUserDtoValidator>();
            services.AddTransient<LoginRequestDtoValidator>();
        }

        public static Guid? GetTokenId(this HttpRequest request)
        {
            if (request == null || !request.Headers.ContainsKey("Authorization"))
            {
                return null;
            }

            string authHeader = request.Headers["Authorization"].ToString();

            if (authHeader.Split("Bearer ").Length != 2)
            {
                return null;
            }

            string token = authHeader.Split("Bearer ")[1];

            var handler = new JwtSecurityTokenHandler();

            var tokenObj = handler.ReadJwtToken(token);

            var claims = tokenObj.Claims;

            var claim = claims.First(x => x.Type == "jti").Value;

            var tokenGuid = Guid.Parse(claim);

            return tokenGuid;
        }
    }
}
