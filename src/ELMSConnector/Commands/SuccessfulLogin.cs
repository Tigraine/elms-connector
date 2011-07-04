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

namespace ElmsConnector.Commands
{
    using System;

    internal class SuccessfulLogin
    {
        private readonly IHttpResponse response;
        private readonly IElmsSessionRequestService sessionRequestService;

        public SuccessfulLogin(IHttpResponse response, IElmsSessionRequestService sessionRequestService)
        {
            this.response = response;
            this.sessionRequestService = sessionRequestService;
        }

        public void Execute(string token, string username, string returnUrl)
        {
            var serviceResponse = sessionRequestService.OpenSession(token, username);

            response.Redirect(serviceResponse.ResponseBody); //This is a blocking operation
        }
    }
}