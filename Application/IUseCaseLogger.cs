using System;
using System.Collections.Generic;
using System.Text;
using Application.DTO.Log;

namespace Application
{
    public interface IUseCaseLogger
    {
        void Log(UseCaseLogDto log);
    }
}
