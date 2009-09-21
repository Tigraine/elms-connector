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
    using System.DirectoryServices;
    using Castle.Core.Logging;

    public class LdapAuthenticationService : IAuthenticatonService
    {
        private readonly string _domain;
        private readonly string _ldapPath;

        private ILogger _logger = NullLogger.Instance;

        public LdapAuthenticationService(string ldapPath, string domain)
        {
            _ldapPath = ldapPath;
            _domain = domain;
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public bool AuthenticateUser(string username, string password)
        {
            return DirectLdap(username, password);
        }

        private bool DirectLdap(string user, string password)
        {
            try
            {
                using (var directoryEntry = new DirectoryEntry(_ldapPath,
                                                               _domain + "\\" + user, password,
                                                               AuthenticationTypes.Secure))
                {
                    var ldap = directoryEntry.NativeObject != null;
                    Logger.DebugFormat("Authenticated User {0} with result {1}", user, ldap);
                    return ldap;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error during LDAP Authentication", e);
                return false;
            }
        }
    }
}