using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    public interface IQuery<TResult, TSearch> : IUseCase
        where TResult : class
    {
        TResult Execute(TSearch search);
    }
}
