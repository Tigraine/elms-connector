namespace ElmsConnector.Tests
{
    using Commands;
    using Rhino.Mocks;
    using Services;
    using Xunit;

    public class FileExtension_Fixture
    {
        [Fact]
        public void FileExtensionIsCorrectlySetInLoginView()
        {
            var command = new LoginCommand();
            var responseMock = MockRepository.GenerateMock<IHttpResponse>();
            command.Response = responseMock;
            var templateProvider = MockRepository.GenerateStub<ITemplateProvider>();
            templateProvider.Stub(p => p.GetTemplate(null)).IgnoreArguments().Return("$EXTENSION$");
            command.TemplateProvider = templateProvider;
            command.FileExtensionProvider = new FileExtensionProvider() { Extension = "EXT"};
            command.Request = MockRepository.GenerateStub<IHttpRequest>();
            command.Session = MockRepository.GenerateStub<IHttpSession>();

            command.Execute();

            responseMock.AssertWasCalled(p => p.Write("EXT"));
        }
    }
}