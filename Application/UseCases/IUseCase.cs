using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    public interface IUseCase
    {
        int Id { get; }
        string Name { get; }
    }
}
