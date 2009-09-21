namespace ElmsConnector.Model
{
    using System;

    public class SessionToken
    {
        private const int TokenTimeout = 5;
        public SessionToken(DateTime time, string token, string url)
        {
            CreationTimestamp = time;
            Expiration = time.AddMinutes(TokenTimeout);
            Token = token;
            ReturnUrl = url;
        }

        public DateTime Expiration { get; set; }
        public string Token { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string ReturnUrl { get; set; }
    }
}