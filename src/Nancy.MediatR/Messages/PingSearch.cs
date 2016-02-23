using MediatR;
using Nancy.MediatR.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nancy.MediatR.Messages
{
    public class PingSearch : IAsyncRequest<List<PingMessage>>
    {
        public string message { get; set; }
    }
    public class PingSearchHandler : IAsyncRequestHandler<PingSearch, List<PingMessage>>
    {
        private readonly IDocumentStore _store;
        public Task<List<PingMessage>> Handle(PingSearch message)
        {
            using (var session = _store.OpenSession())
            {
                return Task.FromResult(session.Query<PingMessage>().Search(x => x.Message,message.message, escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard).ToList());
            }
            
        }
        public PingSearchHandler(IDocumentStore store)
        {
            _store = store;
        }
    }
}
