/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using biz.dfch.CS.Abiquo.Client;
using biz.dfch.CS.Abiquo.Client.Factory;
using biz.dfch.CS.Abiquo.Client.v1.Model;
using biz.dfch.CS.Testing.Attributes;
using biz.dfch.CS.Testing.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace biz.dfch.PS.Abiquo.Client.Tests
{
    [TestClass]
    public class ExportConfigurationTest
    {
        public static BaseAbiquoClient Client;
        public static User User;

        private readonly Type sut = typeof(ExportConfiguration);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            User = new User()
            {
                Active = true,
                AuthType = "Abiquo",
                AvailableVirtualDatacenters = "all",
                Description = "arbitrary-description",
                Email = "someone@example.com",
                FirstLogin = false,
                Id = EnterServer.TENANT_ID_DEFAULT_VALUE,
                Locale = "en-us",
            };

            // this must be inside ClassInitialize - otherwise the tests will only work one at a time
            Client = Mock.Create<CS.Abiquo.Client.v1.AbiquoClient>(Behavior.CallOriginal);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Mock.Arrange(() => Client.CurrentUserInformation)
                .IgnoreInstance()
                .Returns(User);

            Mock.SetupStatic(typeof(AbiquoClientFactory));
            Mock.Arrange(() => AbiquoClientFactory.GetByVersion(Arg.IsAny<string>()))
                .Returns(Client);

            // strange - the mock inside the PSCmdlet only works when we invoke the mocked methods here first
            // this seems to be related to the Lazy<T> we use to initialise the Abiquo client via the factory
            var currentClient = ModuleConfiguration.Current.Client;
        }
        
        //[TestMethod]
        //[ExpectParameterBindingException(MessagePattern = "Path")]
        //public void ExportConfigurationWithEmptyPathThrowsContractException()
        //{
        //    var parameters = @"-Path ''";
        //    PsCmdletAssert.Invoke(sut, parameters);
        //}

        //[TestMethod]
        //[ExpectContractFailure(MessagePattern = "Directory.Exists.Path.FullName.+existing-directory")]
        //public void ExportConfigurationWithExitingDirectoryAsPathThrowsContractException()
        //{
        //    Mock.SetupStatic(typeof(Directory));
        //    Mock.Arrange(() => Directory.Exists(Arg.IsAny<string>()))
        //        .OnAllThreads()
        //        .Returns(true);

        //    var parameters = @"-Path existing-directory";
        //    PsCmdletAssert.Invoke(sut, parameters, ex => ex);
        //}

        //[TestMethod]
        //[ExpectContractFailure(MessagePattern = "File.Exists.Path.FullName.+invalid-configuration-file-name")]
        //public void ExportConfigurationWithInvalidPathThrowsContractException()
        //{
        //    var parameters = @"-Path invalid-configuration-file-name";
        //    PsCmdletAssert.Invoke(sut, parameters, ex => ex);
        //}

        //[TestMethod]
        //[ExpectContractFailure(MessagePattern = ModuleConfiguration.CONFIGURATION_FILE_NAME)]
        //public void ExportConfigurationWithEmptyPathResolvesDefaultConfigurationFileName()
        //{
        //    Mock.SetupStatic(typeof(File));
        //    Mock.Arrange(() => File.Exists(Arg.IsAny<string>()))
        //        .OnAllThreads()
        //        .Returns(false);

        //    var parameters = @";";
        //    PsCmdletAssert.Invoke(sut, parameters, ex => ex);
        //}

        //[TestMethod]
        //public void ExportConfigurationWithEmptyPathSucceeds()
        //{
        //    var parameters = @";";
        //    var results = PsCmdletAssert.Invoke(sut, parameters);
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(1, results.Count);
        //    var result = results[0].BaseObject;
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(ModuleContextSection));
        //}

        //[TestMethod]
        //public void ExportConfigurationWithEmptyPathAndSaveToVariableSucceeds()
        //{
        //    var parameters = @"-SaveToModuleVariable;";
        //    var results = PsCmdletAssert.Invoke(sut, parameters);
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(1, results.Count);
        //    var result = results[0].BaseObject;
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(ModuleContextSection));
        //}

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "moduleContextFromVariable")]
        public void ExportConfigurationWithoutVariableThrowsContractException()
        {
            var parameters = @"-Confirm:$false;";
            var results = PsCmdletAssert.Invoke(sut, parameters, ex => ex);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            var result = results[0].BaseObject;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ModuleContextSection));
        }
    }
}
