using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace Implementation.UseCases
{
    public abstract class EfUseCase
    {
        private readonly CustomContext _context;

        protected EfUseCase(CustomContext context)
        {
            _context = context;
        }

        protected CustomContext Context => _context;
    }
}
