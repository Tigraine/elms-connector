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

namespace ElmsConnector.Services
{
    using System;
    using Castle.Core.Logging;

    public class ElmsSessionRequestService : IElmsSessionRequestService
    {
        private readonly IRemoteRequestService _remoteRequestService;
        private readonly string _cgiConnector;
        private string _department = "Department";
        private ILogger _logger = NullLogger.Instance;

        public ElmsSessionRequestService(IRemoteRequestService remoteRequestService, string cgiConnector)
        {
            _remoteRequestService = remoteRequestService;
            _cgiConnector = cgiConnector;
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public string Department
        {
            get { return _department; }
            set { _department = value; }
        }

        public string SharedSecret { get; set; }
        public string AccountNumber { get; set; }

        public ElmsSessionServiceResponse OpenSession(string token, string username)
        {
            string requestUri = String.Format("{0}?account={1}&username={2}&key={3}", _cgiConnector,
                                              AccountNumber, username, SharedSecret);
            try
            {
                //TODO: There is no HTTP-Response checking going on here
                var elmsResponse = _remoteRequestService.RequestUri(requestUri);
                _logger.DebugFormat("Opening Session through Url: {0}\n\tResponse:", requestUri, elmsResponse);
                return new ElmsSessionServiceResponse() {HasSucceeded = true, ResponseBody = elmsResponse};
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Exception occured during ELMS Open Session request {0}");
                throw;
            }
        }
    }
}