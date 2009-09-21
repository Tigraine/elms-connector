namespace ElmsConnector
{
    public interface IHttpRequest
    {
        object this[string key] { get; }
    }
}