using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class LinkAccessLog : Entity
    {
        public int AccessCount { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        public DateTime? FirstAccessedAt { get; set; }
        public int LinkId { get; set; }

        public Link Link { get; set; }
    }
}
