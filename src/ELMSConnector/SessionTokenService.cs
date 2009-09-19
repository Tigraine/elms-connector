namespace ELMSConnector
{
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;
    using Castle.Core.Logging;

    public class SessionTokenService
    {
        private readonly string campus;
        private ILogger logger = NullLogger.Instance;

        public SessionTokenService(string campus)
        {
            this.campus = campus;
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public string GetSessionToken()
        {
            var request =
                HttpWebRequest.Create(string.Format("https://msdn40.e-academy.com/elms/Security/Login.aspx?campus={0}", campus));

            WebResponse response = request.GetResponse();
            string query = response.ResponseUri.Query;
            var returnUrl = Regex.Match(query, "(?<=return_url=).*").Captures[0].Value;
            returnUrl = HttpUtility.UrlDecode(returnUrl);
            var token = Regex.Match(query, "(?<=token=)[0-9]*").Captures[0].Value;
            logger.DebugFormat("Obtained Token {0} with returnUrl {1}", token, returnUrl);
            return token;
        }
    }
}