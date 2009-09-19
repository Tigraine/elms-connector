namespace ELMSConnector
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    public class IoC
    {
        public IoC()
        {
            var container = new WindsorContainer();
        }
    }

    public class Configuration
    {
        public string CampusIdentifier { get; set; }
    }
}