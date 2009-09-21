namespace ElmsConnector.Commands
{
    using Castle.Core.Logging;

    public class LoginCommand : ICommand
    {
        private readonly IHttpRequest _request;
        private readonly IHttpResponse _response;
        private readonly IHttpSession _session;
        private readonly ITemplateProvider _templateProvider;

        private ILogger _logger = NullLogger.Instance;

        public LoginCommand(IHttpRequest request, IHttpResponse response, IHttpSession session, ITemplateProvider templateProvider)
        {
            _request = request;
            _response = response;
            _session = session;
            _templateProvider = templateProvider;
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public void Execute()
        {
            string token = _request["token"];
            _session["token"] = token;

            string template = _templateProvider.GetTemplate("Login");
            _response.Write(template);
        }
    }
}