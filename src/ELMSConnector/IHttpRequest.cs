namespace ElmsConnector
{
    public interface IHttpRequest
    {
        string this[string key] { get; }
    }
}