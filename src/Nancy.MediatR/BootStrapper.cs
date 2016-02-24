using Nancy.Bootstrapper;
using StructureMap;
using Nancy.MediatR.Messages;
using MediatR;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Raven.Client.Document;
using Raven.Client;
using Microsoft.Extensions.Configuration;

namespace Nancy.MediatR
{
    public class BootStrapper : Bootstrappers.StructureMap.StructureMapNancyBootstrapper
    {
        private readonly IApplicationEnvironment appEnv;
        private readonly IHostingEnvironment env;
        public BootStrapper(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            this.appEnv = appEnv;
            this.env = env;
        }

        protected override void ApplicationStartup(IContainer container, IPipelines pipelines)
        {
            var factory = new LoggerFactory();
            var logger = new LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger();
            factory.AddSerilog(logger);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"config.{env.EnvironmentName}.json", optional: true).Build();
           
           
                container.Configure(registry => registry.Scan(scanner => {
                scanner.AssemblyContainingType<Ping>();
                scanner.AssemblyContainingType<IMediator>();
               scanner.WithDefaultConventions();
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                registry.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                registry.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                registry.For<ILoggerFactory>().Use(factory).Singleton();
                registry.For<IConfiguration>().Use(config).Singleton();
                registry.For<IDocumentStore>().Use(CreateStore(config["Data:RavenConnection"])).Singleton();
            }));
            
          
        }

        private  IDocumentStore CreateStore(string connection)
        {
            IDocumentStore store = new DocumentStore()
            {
                Url = connection,
                DefaultDatabase = "Northwind"
            }.Initialize();

            return store;
        }
    }
}
