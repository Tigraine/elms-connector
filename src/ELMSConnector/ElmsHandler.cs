namespace ElmsConnector
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.SessionState;
    using Castle.Core.Logging;
    using Castle.Windsor;

    public class ElmsHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly IWindsorContainer _container;
        private ILogger _logger = NullLogger.Instance;

        public ElmsHandler() : this(new ConventionBasedContainerFactory())
        {
        }

        public ElmsHandler(IContainerFactory factory)
        {
            _container = factory.CreateContainer();
            _logger = _container.Resolve<ILogger>();
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            _logger.DebugFormat("Incoming request on Path: {0}", context.Request.Path);
            string path = context.Request.Path;
            if (path.EndsWith(".elms"))
            {
                string commandName = Regex.Match(path, @"(?<=/)\w*").Captures[0].Value;
                string commandClassName = String.Format("{0}Command", commandName);
                try
                {
                    var command = _container.Resolve<ICommand>(commandClassName);
                    try
                    {
                        command.Execute();
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnFormat(ex, "Failed to execute {0} command", commandClassName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.WarnFormat(ex, "Command {0} could not be resolved", commandClassName);
                    throw new HttpException(404, String.Format("File {0} could not be found", path), ex);
                }
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        #endregion
    }
}