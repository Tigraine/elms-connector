namespace ElmsConnector.Abstractions
{
    using System.Web;

    public class HttpRequestFacade : IHttpRequest
    {
        private readonly HttpRequest _httpRequest;

        public HttpRequestFacade(HttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        public string this[string key]
        {
            get { return _httpRequest[key]; }
        }
    }
}