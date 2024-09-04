using Application.UseCases;
using Application;
using Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Log;

namespace Implementation
{
    public class UseCaseHandler
    {
        private readonly IApplicationActor _actor;
        private readonly IUseCaseLogger _logger;
        public UseCaseHandler(IApplicationActor actor, IUseCaseLogger logger)
        {
            _actor = actor;
            _logger = logger;
        }
        public void HandleCommand<TData>(ICommand<TData> command, TData data)
        {
            HandleCrossCuttingConcerns(command, data);
            command.Execute(data);
        }

        public TResult HandleQuery<TResult, TSearch>(IQuery<TResult, TSearch> query, TSearch search)
            where TResult : class

        {
            HandleCrossCuttingConcerns(query, search);
            return query.Execute(search);
        }

        private void HandleCrossCuttingConcerns(IUseCase useCase, object data)
        {
            //Autorizacija
            if (!_actor.WorkspacesUseCases.Any(wus => wus.UseCaseIds.Contains(useCase.Id)) && useCase.Name != UseCasesEnum.WorkspaceRetrievalByLink)
            {
                throw new UnauthorizedAccessException();
            }

            if (useCase.Name != UseCasesEnum.UserRegistration)
            {
                _logger.Log(new UseCaseLogDto
                {
                    UseCaseData = data,
                    UseCaseName = useCase.Name.ToString(),
                    Username = _actor.Username,
                });
            }
        }
    }

}
