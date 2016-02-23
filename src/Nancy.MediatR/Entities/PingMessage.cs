
using System;

namespace Nancy.MediatR.Entities
{
    public class PingMessage
    {
        public DateTime DateCreated { get; set; }
        public string Message { get; set; }
    }
}
