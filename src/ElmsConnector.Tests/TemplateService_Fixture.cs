namespace ElmsConnector.Tests
{
    using System;
    using System.IO;
    using Rhino.Mocks;
    using Services;
    using Xunit;

    public class TemplateService_Fixture
    {
        [Fact]
        public void GetsFilePathFromPathProvider()
        {
            var pathProvider = MockRepository.GenerateStub<IPathProvider>();
            pathProvider.Stub(p => p.GetPath("Login")).Return("/Login.htm");
            var templateProvider = new FileSystemTemplateService(pathProvider);

            try
            {
                templateProvider.GetTemplate("Login");
            }
            catch (Exception)
            {
                //It will throw due to Login.html not being present.
            }

            pathProvider.AssertWasCalled(p => p.GetPath("Login"));
        }

        [Fact]
        public void ReturnsContentOfFileFromDisk()
        {
            string expected = "file-content";

            var pathProvider = MockRepository.GenerateStub<IPathProvider>();
            pathProvider.Stub(p => p.GetPath("Login")).Return("test.htm");
            using (StreamWriter writer = File.CreateText("test.htm"))
            {
                writer.Write(expected);
                writer.Flush();
                writer.Close();
            }
            var provider = new FileSystemTemplateService(pathProvider);

            string template = provider.GetTemplate("Login");

            Assert.Equal(expected, template);

            File.Delete("test.htm"); //Cleanup
        }

        [Fact]
        public void TemplateContainsErrorMessageIfFileNotFound()
        {
            var pathProvider = MockRepository.GenerateStub<IPathProvider>();
            pathProvider.Stub(p => p.GetPath("Login")).Return("test.htm");

            var provider = new FileSystemTemplateService(pathProvider);
            string template = provider.GetTemplate("Login");

            Assert.Contains("Template File 'Login' could not be found at ", template);
        }

        [Fact]
        public void GetTemplateDoesNotThrowExceptionIfFileNotFound()
        {
            var pathProvider = MockRepository.GenerateStub<IPathProvider>();
            pathProvider.Stub(p => p.GetPath("Login")).Return("test.htm");

            var provider = new FileSystemTemplateService(pathProvider);

            Assert.DoesNotThrow(() => provider.GetTemplate("Login"));
        }
    }
}