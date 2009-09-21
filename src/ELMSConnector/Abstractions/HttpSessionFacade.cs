namespace ElmsConnector.Abstractions
{
    using System.Web.SessionState;

    public class HttpSessionFacade : IHttpSession
    {
        private readonly HttpSessionState _sessionState;

        public HttpSessionFacade(HttpSessionState sessionState)
        {
            _sessionState = sessionState;
        }

        public object this[string key]
        {
            get { return _sessionState[key]; }
            set { _sessionState[key] = value; }
        }
    }
}