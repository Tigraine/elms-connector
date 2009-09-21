namespace ElmsConnector.Services
{
    using System;
    using Model;

    public class ElmsLoginService
    {
        private readonly string _campus;

        public ElmsLoginService(string campus)
        {
            _campus = campus;
        }

        public string GetRedirectUrl(SessionToken token, string userId)
        {
            return
                String.Format(
                    "https://msdn40.e-academy.com/elms/Security/IntegratedLogin.aspx?campus={0}&token={1}&uid={2}",
                    _campus, token.Token, userId);
        }
    }
}