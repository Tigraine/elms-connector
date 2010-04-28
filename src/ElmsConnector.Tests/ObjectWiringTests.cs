using System;
using System.Web;
using System.Web.SessionState;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using ElmsConnector.Commands;
using Rhino.Mocks;
using Xunit;

namespace ElmsConnector.Tests
{
    public class ObjectWiringTests
    {
        [Fact]
        public void CanSuccessfullyConstructLoginCommand()
        {
            IWindsorContainer windsorContainer = CreateContainer();
            var command = windsorContainer.Resolve<ICommand>("LoginCommand");
        }

        [Fact]
        public void CanSuccessfullyConstructLoginCommandWithExtendedAuthService()
        {
            IWindsorContainer windsorContainer = CreateContainer("elms-extended.xml");
            var command = windsorContainer.Resolve<ICommand>("LoginCommand");
        }

        private IWindsorContainer CreateContainer(string elmsConfig)
        {
            var factory = new ConventionBasedContainerFactory();
            var container = new WindsorContainer(new XmlInterpreter(elmsConfig));

            container.Register(
                Component.For<IHttpRequest>()
                    .Instance(MockRepository.GenerateStub<IHttpRequest>()),
                Component.For<IHttpResponse>()
                    .Instance(MockRepository.GenerateStub<IHttpResponse>()),
                Component.For<IHttpSession>()
                    .Instance(MockRepository.GenerateStub<IHttpSession>()),
                Component.For<IPathProvider>()
                    .Instance(MockRepository.GenerateStub<IPathProvider>()));

            factory.ConfigureFacilities(container);
            factory.RegisterServices(container);

            return container;
        }

        private IWindsorContainer CreateContainer()
        {
            return CreateContainer("elms.xml");
        }
    }

    public class FakeAuthService : IAuthenticationService
    {
        public bool AuthenticateUser(string username, string password)
        {
            return true;
        }
    }
}