/* Copyright 2009 Daniel Hölbling
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *     
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace ElmsConnector
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
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

        public void ProcessRequest(HttpContext context)
        {
            _logger.DebugFormat("Incoming request on Path: {0}", context.Request.Path);
            string path = context.Request.Path;
            if (path.EndsWith(".elms"))
            {
                string commandName = Regex.Match(path, @"(?<=/)\w*").Captures[0].Value;
                string commandClassName = String.Format("{0}Command", commandName);
                ICommand command;
                try
                {
                    command = _container.Resolve<ICommand>(commandClassName);
                }
                catch (Exception ex)
                {
                    _logger.WarnFormat(ex, "Command {0} could not be resolved", commandClassName);
                    throw new HttpException(404, String.Format("File {0} could not be found", path), ex);
                }

                ExecuteCommand(command);
                
            }
        }

        private void ExecuteCommand(ICommand command)
        {
            try
            {
                command.Execute();
            }
            catch (ThreadAbortException ex)
            {
                _logger.DebugFormat("Swallowed ThreadAbortException");
                //Swallow. A redirect has happened
            }
            catch (Exception ex)
            {
                _logger.WarnFormat(ex, "Failed to execute {0} command", command.ToString());
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}