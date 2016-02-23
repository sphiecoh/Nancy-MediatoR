using MediatR;
using Nancy.MediatR.Entities;
using Raven.Client;
using System;
using System.Threading.Tasks;

namespace Nancy.MediatR.Messages
{
    public class Ping : IAsyncRequest<PingMessage>
    {
        public string Name { get; set; }
    }

    public class PingHandler : IAsyncRequestHandler<Ping, PingMessage>
    {
        private readonly IDocumentStore _store;
       
        public PingHandler(IDocumentStore store)
        {
            _store = store;
          
        }
       async Task<PingMessage> IAsyncRequestHandler<Ping, PingMessage>.Handle(Ping message)
        {
            var ping = new PingMessage { DateCreated = DateTime.Now, Message = message.Name };
            using (var session = _store.OpenAsyncSession())
            {
              await  session.StoreAsync(ping);
              await session.SaveChangesAsync();
              return ping;
            }
           
        }
    }

}
