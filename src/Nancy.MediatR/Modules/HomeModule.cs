using MediatR;
using Nancy.MediatR.Messages;
using Nancy.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.ConfigurationModel;


namespace Nancy.MediatR.Modules
{
    public class HomeModule : NancyModule
    {
        private readonly IMediator _bus;
        private readonly ILogger _logger;
            
        public HomeModule(IMediator mediator/*, ILoggerFactory loggerFactory*/)
        {
            _bus = mediator;
            //_logger = loggerFactory.CreateLogger<HomeModule>();
           

            Get["/"] = _ => "helo world";
            Post["/",true] = async(_,token) => 
            {
                var message = this.Bind<Ping>();
               // _logger.LogInformation($"Creating ping {message.Name}");
                var res = await _bus.SendAsync(message);
                return res;
            };

            Post["/search",true] = async(x,token) => 
            {
                var query = this.Bind<PingSearch>();
                var result = await _bus.SendAsync(query);
                return result;
            };
        }
    }
}
