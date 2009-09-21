namespace ElmsConnector.Commands
{
    using System;
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
            Logger.DebugFormat("Recieved Token {0}", token);

            string template = _templateProvider.GetTemplate("Login");

            template = PlaceErrorText(template);
            
            _response.Write(template);
        }

        private string PlaceErrorText(string template)
        {
            if (String.IsNullOrEmpty(template)) return template;
            if (_request["error"] == "true")
            {
                return template.Replace("$ERROR$", "<p class=\"error\">Login failed</p>");
            }
            return template.Replace("$ERROR$", "");
        }
    }
}