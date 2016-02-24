using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Nancy.MediatR.Entities;
using Nancy.MediatR.Messages;
using Nancy.MediatR.Modules;
using Nancy.Testing;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;

namespace Nancy.MediatR.Tests
{
    public class HomeModuleTest
    {
        PingMessage pingMessage = new PingMessage { Message = "testing", DateCreated = DateTime.Now };
        PingSearch search = new PingSearch { message = "testing" };

        [Fact]
        public void Create()
        {
            var ping = new Ping { Name = "testing" };
            var mediator = new Mock<IMediator>();
            var logFactory = new Mock<ILoggerFactory>();
            //logFactory.Setup(x => x.CreateLogger<HomeModule>());
            mediator.Setup(x => x.SendAsync(It.IsAny<Ping>())).Returns(Task.FromResult(pingMessage));
            var browser = new Browser(_ => _.Module<HomeModule>().Dependencies(mediator.Object));
            var result =browser.Post("/", with => {
                with.JsonBody(ping);
                with.HttpRequest();
                with.Header("Accept", "application/json");
            }).Body.DeserializeJson<PingMessage>();

            result.Should().NotBeNull();
            result.Should().Equals(pingMessage);
        }

        [Fact]
        public void Search()
        {
            var mediator = new Mock<IMediator>();
           
            mediator.Setup(x => x.SendAsync(It.IsAny<PingSearch>())).Returns(Task.FromResult(new List<PingMessage> { pingMessage }));
            var browser = new Browser(_ => _.Module<HomeModule>().Dependencies(mediator.Object));
            var response = browser.Post("/search", _ => 
            {
                _.HttpRequest();
                _.JsonBody(search);
                _.Header("Accept", "application/json");
            }).Body.DeserializeJson<List<PingMessage>>();

            response.Should().NotBeNull();
            response.Should().BeOfType<List<PingMessage>>();
            response.Should().HaveCount(1);
        }

        [Fact]
        public void Get()
        {
            var mediator = new Mock<IMediator>();
            var browser = new Browser(_ => _.Module<HomeModule>().Dependencies(mediator.Object));
            var response = browser.Get("/", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json");
            }).Body.AsString();

            response.Should().Contain("helo world");

        }

    }
}
