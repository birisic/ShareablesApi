using Application;
using Application.UseCases.Commands.User;
using Application.UseCases.Commands.Workspace;
using Application.UseCases.Queries.User;
using Implementation;
using Implementation.Logging;
using Implementation.UseCases.Commands.User;
using Implementation.UseCases.Commands.Workspace;
using Implementation.UseCases.Queries.Workspace;
using Implementation.Validators.User;
using Implementation.Validators.Workspace;
using System.IdentityModel.Tokens.Jwt;

namespace Shareables.API.Core
{
    public static class ContainerExtensions
    {
        public static void AddUseCases(this IServiceCollection services)
        {
            //miscellaneous
            services.AddTransient<UseCaseHandler>();
            services.AddTransient<IUseCaseLogger, EfUseCaseLogger>();

            //queries
            services.AddTransient<IGetWorkspaceQuery, EfGetWorkspaceQuery>();

            //commands
            services.AddTransient<IRegisterUserCommand, EfRegisterUserCommand>();
            services.AddTransient<ICreateWorkspaceCommand, EfCreateWorkspaceCommand>();
            //services.AddTransient<IUpdateUseAccessCommand, EfUpdateUserAccessCommand>();

            //validators
            services.AddTransient<WorkspaceValidator>();
            services.AddTransient<RegisterUserDtoValidator>();
            services.AddTransient<LoginRequestDtoValidator>();
            //services.AddTransient<UpdateUserAccessDtoValidator>();
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
