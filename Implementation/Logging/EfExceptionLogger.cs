using Application;
using DataAccess;
using Domain;
using Shareables.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Logging
{
    public class EfExceptionLogger : IExceptionLogger
    {
        private readonly CustomContext _context;

        public EfExceptionLogger(CustomContext context)
        {
            _context = context;
        }

        public Guid Log(Exception ex, IApplicationActor actor)
        {
            Guid id = Guid.NewGuid();

            //ID, Message, Time, StrackTrace
            ErrorLog log = new()
            {
                ErrorId = id,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                OccurredAt = DateTime.UtcNow
            };

            _context.ErrorLogs.Add(log);

            _context.SaveChanges();

            return id;
        }
    }

}
