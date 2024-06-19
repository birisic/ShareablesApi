using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
