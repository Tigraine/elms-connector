namespace ElmsConnector
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Commands;

    public class ElmsHandler : IHttpHandler
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
            string s = context.Server.MapPath("test.htm");
            context.Response.Write(s);
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