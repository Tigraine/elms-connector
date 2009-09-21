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
    using System.IO;

    public class FileSystemTemplateService : ITemplateProvider
    {
        private readonly IPathProvider _pathProvider;

        public FileSystemTemplateService(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public string GetTemplate(string templateName)
        {
            string path = _pathProvider.GetPath(templateName);
            try
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    var reader = new StreamReader(stream);
                    string template = reader.ReadToEnd();
                    reader.Close();
                    return template;
                }
            }
            catch (FileNotFoundException ex)
            {
                return String.Format("Template File '{0}' could not be found at {1}", templateName,  ex.FileName);
            }
        }
    }
}