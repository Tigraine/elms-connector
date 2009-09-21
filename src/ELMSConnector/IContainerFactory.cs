namespace ElmsConnector
{
    using Castle.Windsor;

    public interface IContainerFactory
    {
        IWindsorContainer CreateContainer();
    }
}