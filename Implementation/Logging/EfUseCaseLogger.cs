using Application;
using Application.DTO.Log;
using DataAccess;
using Domain;
using Newtonsoft.Json;

namespace Implementation.Logging
{
    public class EfUseCaseLogger : IUseCaseLogger
    {
        private CustomContext _context;
        public EfUseCaseLogger(CustomContext ctx) => _context = ctx; 
        public void Log(UseCaseLogDto log)
        {
            string username = log.Username;
            string useCase = log.UseCaseName;
            string useCaseData = JsonConvert.SerializeObject(log.UseCaseData);

            _context.UseCaseLogs.Add(new UseCaseLog
            {
                Username = username,
                UseCaseName = useCase,
                UseCaseData = useCaseData,
                ExecutedAt = DateTime.UtcNow
            });

            _context.SaveChanges();

        }
    }
}
