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
    using System.Web.SessionState;
    using Castle.Core;
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;

    public class ConventionBasedContainerFactory : IContainerFactory
    {
        public IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer(new XmlInterpreter("elms.xml"));

            ConfigureFacilities(container);
            RegisterServices(container);
            RegisterFactories(container);
            
            return container;
        }

        public IWindsorContainer ConfigureFacilities(IWindsorContainer container)
        {
            var facility = new LoggingFacility(LoggerImplementation.Console);
            container.AddFacility<FactorySupportFacility>();
            container.AddFacility("logging", facility);

            return container;
        }

        public IWindsorContainer RegisterServices(IWindsorContainer container)
        {
            Assembly assembly = typeof(ConventionBasedContainerFactory).Assembly;
            container.Register(
                AllTypes.Pick().FromAssembly(assembly)
                    .If(p => p.Namespace.EndsWith("Services") || p.Namespace.EndsWith("Commands"))
                    .Configure(p => p.Named(p.Implementation.Name))
                    .Configure(p => p.LifeStyle.Is(LifestyleType.PerWebRequest))
                    .WithService.FirstInterface());
            return container;
        }

        public IWindsorContainer RegisterFactories(IWindsorContainer container)
        {
            container.Register(
                Component.For<HttpRequest>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => HttpContext.Current.Request),
                Component.For<HttpResponse>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => HttpContext.Current.Response),
                Component.For<HttpSessionState>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => HttpContext.Current.Session),
                Component.For<HttpServerUtility>()
                    .LifeStyle.PerWebRequest
                    .UsingFactoryMethod(() => HttpContext.Current.Server));
            return container;
        }
    }
}