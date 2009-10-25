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

    public class LoginCommand : AbstractCommandBase
    {
        private readonly ISessionAuthenticationService authenticationService;
        private readonly IElmsSessionRequestService elmsSessionRequestService;

        public LoginCommand(ISessionAuthenticationService authenticationService, IElmsSessionRequestService elmsSessionRequestService)
        {
            this.authenticationService = authenticationService;
            this.elmsSessionRequestService = elmsSessionRequestService;
        }

        public LoginCommand()
        {
            
        }

        public override void Execute()
        {
            string token = Request["token"];
            string returnUrl = Request["return_url"];

            if (authenticationService != null && authenticationService.IsAlreadyAuthenticated())
            {
                var login = new SuccessfulLogin(Response, elmsSessionRequestService);
                login.Execute(token, authenticationService.Username, returnUrl);
                return;
            }

            Session["token"] = token;
            Session["returnUrl"] = returnUrl;
            Logger.DebugFormat("Recieved Token {0}", token);

            string template = TemplateProvider.GetTemplate("Login");

            template = PlaceErrorText(template);
            template = PlaceFileExtension(template);
            
            Response.Write(template);
        }

        private string PlaceFileExtension(string template)
        {
            if (String.IsNullOrEmpty(template)) return template;
            return template.Replace("$EXTENSION$", FileExtensionProvider.Extension);
        }

        private string PlaceErrorText(string template)
        {
            if (String.IsNullOrEmpty(template)) return template;
            if (Request["error"] == "true")
            {
                return template.Replace("$ERROR$", "<p class=\"error\">Login failed</p>");
            }
            return template.Replace("$ERROR$", "");
        }
    }
}