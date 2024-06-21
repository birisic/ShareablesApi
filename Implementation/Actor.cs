using Application;

namespace Implementation
{
    public class Actor : IApplicationActor
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public IEnumerable<int> AllowedUseCases { get; set; }
    }

    public class UnauthorizedActor : IApplicationActor
    {
        public int Id => 0;
        public string Username => "unauthorized";
        public IEnumerable<int> AllowedUseCases => new List<int> { 1 };
    }
}
