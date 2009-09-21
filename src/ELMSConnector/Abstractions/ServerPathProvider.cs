namespace ElmsConnector.Abstractions
{
    using System;
    using System.Web;

    public class ServerPathProvider : IPathProvider
    {
        private readonly HttpServerUtility _server;

        public ServerPathProvider(HttpServerUtility utility)
        {
            _server = utility;
        }

        public string GetPath(string path)
        {
            return _server.MapPath(path);
        }
    }
}