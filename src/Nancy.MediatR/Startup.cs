using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using MediatR;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Framework.ConfigurationModel;

namespace Nancy.MediatR
{
    public class Startup
    {
      
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            app.UseIISPlatformHandler();
            app.UseOwin(pipeline => pipeline.UseNancy(options => options.Bootstrapper = new BootStrapper(env, appEnv)));
           
        }
 
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
