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
    using System.Web;

    public class HttpResponseFacade : IHttpResponse
    {
        private readonly HttpResponse _response;

        public HttpResponseFacade(HttpResponse response)
        {
            _response = response;
        }

        public void Write(string response)
        {
            _response.Write(response);
        }

        public void Redirect(string url)
        {
            _response.Redirect(url);
        }
    }
}