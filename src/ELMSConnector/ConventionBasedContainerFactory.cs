/* Copyright 2009 Daniel Hölbling
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *     
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

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
                    .UsingFactoryMethod(() => new HttpSessionFacade(HttpContext.Current.Session)),
                Component.For<IPathProvider>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => new ServerPathProvider(HttpContext.Current.Server)));
            return container;
        }

        #endregion
    }
}