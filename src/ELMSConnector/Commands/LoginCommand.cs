namespace ElmsConnector.Commands
{
    using System;

    public class LoginCommand : ICommand
    {
        private readonly IHttpRequest _request;

        public LoginCommand(IHttpRequest request)
        {
            _request = request;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}