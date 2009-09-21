namespace ElmsConnector
{
    public interface IHttpResponse
    {
        void Write(string response);
        void Redirect(string url);
    }
}