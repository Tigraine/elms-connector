namespace ElmsConnector
{
    public interface IHttpSession
    {
        object this[string key] { get; set; }
    }
}