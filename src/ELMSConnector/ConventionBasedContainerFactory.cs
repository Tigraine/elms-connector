namespace ElmsConnector
{
    using System.Reflection;
    using System.Web;
    using Abstractions;
    using Castle.Core;
    using Castle.Core.Resource;
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;

    public class ConventionBasedContainerFactory : IContainerFactory
    {
        #region IContainerFactory Members

        public IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer(new XmlInterpreter(new ConfigResource("ioc")));
            var facility = new LoggingFacility(LoggerImplementation.Console);
            container.AddFacility<FactorySupportFacility>();
            container.AddFacility("logging", facility);
            Assembly assembly = typeof (ConventionBasedContainerFactory).Assembly;
            container.Register(
                AllTypes.Pick().FromAssembly(assembly)
                    .If(p => p.Namespace.EndsWith("Services") || p.Namespace.EndsWith("Commands"))
                    .Configure(p => p.Named(p.Implementation.Name))
                    .Configure(p => p.LifeStyle.Is(LifestyleType.PerWebRequest))
                    .WithService.FirstInterface());
            container.Register(
                Component.For<IHttpRequest>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => new HttpRequestFacade(HttpContext.Current.Request)),
                Component.For<IHttpResponse>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => new HttpResponseFacade(HttpContext.Current.Response)),
                Component.For<IHttpSession>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => new HttpSessionFacade(HttpContext.Current.Session)));
            return container;
        }

        #endregion
    }
}