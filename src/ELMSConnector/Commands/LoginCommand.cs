namespace ElmsConnector.Commands
{
    using System;

    public class LoginCommand : ICommand
    {
        private readonly IHttpRequest _request;
        private readonly IHttpResponse _response;
        private readonly IHttpSession _session;

        public LoginCommand(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            _request = request;
            _response = response;
            _session = session;
        }

        public void Execute()
        {
            string token = _request["token"];
            if (String.IsNullOrEmpty(token)) throw new ArgumentNullException("token");
            _session["token"] = token;
            _response.Write(token);
        }
    }
}