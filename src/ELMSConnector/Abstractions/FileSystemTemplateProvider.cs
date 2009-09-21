namespace ElmsConnector.Abstractions
{
    using System;
    using System.IO;

    public class FileSystemTemplateProvider : ITemplateProvider
    {
        private readonly IPathProvider _pathProvider;

        public FileSystemTemplateProvider(IPathProvider pathProvider)
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