using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Link : Entity
    {
        public string Token {  get; set; }
        public DateTime Expires_at { get; set; }
        public int DocumentId { get; set; }

        public Workspace Document { get; set; }
        public LinkAccessLog AccessLog { get; set; }
    }
}
