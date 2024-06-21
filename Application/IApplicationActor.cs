using System.Collections.Generic;

namespace Application
{
    public interface IApplicationActor
    {
        int Id { get; }
        string Username { get; }
        IEnumerable<int> AllowedUseCases { get; }
    }
}
