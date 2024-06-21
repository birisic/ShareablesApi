using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Log
{
    public class UseCaseLogDto
    {
        public string Username { get; set; }
        public string UseCaseName { get; set; }
        public object UseCaseData { get; set; }
    }
}
