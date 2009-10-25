namespace ElmsConnector.Commands
{
    using System;

    internal class SuccessfulLogin
    {
        private readonly IHttpResponse response;
        private readonly IElmsSessionRequestService sessionRequestService;

        public SuccessfulLogin(IHttpResponse response, IElmsSessionRequestService sessionRequestService)
        {
            this.response = response;
            this.sessionRequestService = sessionRequestService;
        }

        public void Execute(string token, string username, string returnUrl)
        {
            sessionRequestService.OpenSession(token, username);

            var url = String.Format("{0}&token={1}&uid={2}", returnUrl, token, username);
            response.Redirect(url); //This is a blocking operation
        }
    }
}