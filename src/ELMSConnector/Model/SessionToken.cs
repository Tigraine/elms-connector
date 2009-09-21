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