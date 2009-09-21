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
        public void GetTemplate_GetsFilePathFromPathProvider()
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
        public void GetTemplate_ReturnsContentOfFileFromDisk()
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
        public void GetTemplate_ReturnsErrorMessage_IfFileNotFound()
        {
            var pathProvider = MockRepository.GenerateStub<IPathProvider>();
            pathProvider.Stub(p => p.GetPath("Login")).Return("test.htm");

            var provider = new FileSystemTemplateService(pathProvider);
            string template = provider.GetTemplate("Login");

            Assert.Contains("Template File 'Login' could not be found at ", template);
        }

        [Fact]
        public void GetTemplate_DoesNotThrowException_IfFileNotFound()
        {
            var pathProvider = MockRepository.GenerateStub<IPathProvider>();
            pathProvider.Stub(p => p.GetPath("Login")).Return("test.htm");

            var provider = new FileSystemTemplateService(pathProvider);

            Assert.DoesNotThrow(() => provider.GetTemplate("Login"));
        }
    }
}