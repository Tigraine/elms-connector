namespace ElmsConnector.Services
{
    using System.Web;
    using Castle.Core.Logging;
    using Model;

    public interface ISessionTokenService
    {
        SessionToken GetSessionToken(HttpRequest request);
    }

    public class SessionTokenService : ISessionTokenService
    {
        private ILogger _logger = NullLogger.Instance;
        private IDateTimeService _dateTimeService = Services.DateTimeService.Default;

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public IDateTimeService DateTimeService
        {
            get { return _dateTimeService; }
            set { _dateTimeService = value; }
        }

        public SessionToken GetSessionToken(HttpRequest request)
        {
            string token = request["Token"];
            string returnUrl = request["returnUrl"];
            _logger.DebugFormat("Obtained Token {0} with returnUrl {1}", token, returnUrl);
            return new SessionToken(_dateTimeService.Now, token, returnUrl);
        }
    }
}