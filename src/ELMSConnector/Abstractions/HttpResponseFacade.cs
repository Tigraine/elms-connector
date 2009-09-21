namespace ElmsConnector.Abstractions
{
    using System.Web;

    public class HttpResponseFacade : IHttpResponse
    {
        private readonly HttpResponse _response;

        public HttpResponseFacade(HttpResponse response)
        {
            _response = response;
        }

        public void Write(string response)
        {
            _response.Write(response);
        }

        public void Redirect(string url)
        {
            _response.Redirect(url);
        }
    }
}