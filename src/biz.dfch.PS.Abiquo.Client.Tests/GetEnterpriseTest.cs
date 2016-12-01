﻿/**
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
using biz.dfch.CS.Abiquo.Client;
using biz.dfch.CS.Abiquo.Client.Factory;
using biz.dfch.CS.Abiquo.Client.v1.Model;
using biz.dfch.CS.Testing.Attributes;
using biz.dfch.CS.Testing.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Current = biz.dfch.CS.Abiquo.Client.v1;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace biz.dfch.PS.Abiquo.Client.Tests
{
    [TestClass]
    public class GetEnterpriseTest
    {
        public static BaseAbiquoClient Client;
        public static User User;

        public Enterprises Enterprises = new Enterprises()
        {
            Collection = new List<Enterprise>()
            {
                new Enterprise()
                {
                    Id = 42,
                    Name = "Edgar"
                },
                new Enterprise()
                {
                    Id = 1,
                    Name = "EnterpriseWithDuplicateName"
                },
                new Enterprise()
                {
                    Id = 2,
                    Name = "eNTERPRISEwITHdUPLICATEnAME"
                },
            }
        };

        private readonly Type sut = typeof(GetEnterprise);

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Mock.SetupStatic(typeof(ContractEventHandler));
            Mock.Arrange(
                    () =>
                        ContractEventHandler.ContractFailedEventHandler(Arg.IsAny<object>(),
                            Arg.IsAny<ContractFailedEventArgs>()))
                .DoNothing();

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

            var enterpriseLink =
                new Current.LinkBuilder()
                    .BuildHref(string.Format("https://abiquo.example.com/api/admin/enterprises/{0}",
                        EnterServer.TENANT_ID_DEFAULT_VALUE))
                    .BuildRel(Current.AbiquoRelations.ENTERPRISE)
                    .BuildTitle("Abiquo")
                    .GetLink();

            User.Links = new List<Link>()
            {
                enterpriseLink
            };

            // this must be inside ClassInitialize - otherwise the tests will only work one at a time
            Client = Mock.Create<Current.AbiquoClient>(Behavior.CallOriginal);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Mock.Arrange(() => Client.CurrentUserInformation)
                .IgnoreInstance()
                .Returns(User);

            Mock.SetupStatic(typeof(AbiquoClientFactory));
            Mock.Arrange(() => AbiquoClientFactory.GetByVersion())
                .Returns(Client);
            Mock.Arrange(() => AbiquoClientFactory.GetByVersion(Arg.IsAny<string>()))
                .Returns(Client);

            // strange - the mock inside the PSCmdlet only works when we invoke the mocked methods here first
            // this seems to be related to the Lazy<T> we use to initialise the Abiquo client via the factory
            var currentClient = ModuleConfiguration.Current.Client;
        }

        [TestMethod]
        [ExpectParameterBindingValidationException(MessagePattern = @"'Id'.+range")]
        public void InvokeWithInvalidIdParameterThrowsParameterBindingValidationException1()
        {
            var parameters = @"-Id 0";
            var results = PsCmdletAssert.Invoke(sut, parameters);
        }

        [TestMethod]
        [ExpectParameterBindingValidationException(MessagePattern = @"'Name'.+empty")]
        public void InvokeWithInvalidNameParameterThrowsParameterBindingValidationException1()
        {
            var parameters = @"-Name ''";
            var results = PsCmdletAssert.Invoke(sut, parameters);
        }

        [TestMethod]
        public void ParameterSetListHasExpectedOutputType()
        {
            PsCmdletAssert.HasOutputType(sut, typeof(Enterprise), GetEnterprise.ParameterSets.LIST);
        }

        [TestMethod]
        public void ParameterSetNameHasExpectedOutputType()
        {
            PsCmdletAssert.HasOutputType(sut, typeof(Enterprise), GetEnterprise.ParameterSets.NAME);
        }

        [TestMethod]
        public void ParameterSetIdHasExpectedOutputType()
        {
            PsCmdletAssert.HasOutputType(sut, typeof(Enterprise), GetEnterprise.ParameterSets.ID);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "ModuleConfiguration.Current.Client.IsLoggedIn")]
        public void InvokeNotLoggedInThrowsContractException()
        {
            Mock.Arrange(() => Client.IsLoggedIn)
                .IgnoreInstance()
                .Returns(false);

            var parameters = @"-ListAvailable";

            var results = PsCmdletAssert.Invoke(sut, parameters, ex => ex);
            
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void InvokeParameterSetListAvailableSucceeds()
        {
            Mock.Arrange(() => Client.IsLoggedIn)
                .IgnoreInstance()
                .Returns(true);

            Mock.Arrange(() => Client.GetEnterprises())
                .IgnoreInstance()
                .Returns(Enterprises)
                .MustBeCalled();

            var parameters = @"-ListAvailable";
            
            var results = PsCmdletAssert.Invoke(sut, parameters);
            
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count);

            Mock.Assert(() => Client.GetEnterprises());
        }

        [TestMethod]
        public void InvokeParameterSetIdSucceeds()
        {
            Mock.Arrange(() => Client.IsLoggedIn)
                .IgnoreInstance()
                .Returns(true);

            Mock.Arrange(() => Client.GetEnterprise(Arg.IsAny<int>()))
                .IgnoreInstance()
                .Returns(Enterprises.Collection.First(e => e.Id == 42))
                .MustBeCalled();

            // this Id does not exist
            var parameters = @"-Id 42";
            
            var results = PsCmdletAssert.Invoke(sut, parameters);
            
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            var result = results[0].BaseObject as Enterprise;
            Assert.IsNotNull(result, results[0].BaseObject.GetType().FullName);
            Assert.AreEqual(42, result.Id);
            Assert.AreEqual("Edgar", result.Name);

            Mock.Assert(() => Client.GetEnterprise(Arg.IsAny<int>()));
        }

        [TestMethod]
        public void InvokeParameterSetIdWriterErrorRecord()
        {
            Mock.Arrange(() => Client.IsLoggedIn)
                .IgnoreInstance()
                .Returns(true);

            Mock.Arrange(() => Client.GetEnterprise(Arg.IsAny<int>()))
                .IgnoreInstance()
                .Throws(new Exception("9990"))
                .MustBeCalled();

            // this Id does not exist
            var parameters = @"-Id 9999";
            
            var results = PsCmdletAssert.Invoke(sut, parameters, er => { Assert.IsTrue(er.Single().Exception.Message.Contains("9999")); });
            
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);

            Mock.Assert(() => Client.GetEnterprise(Arg.IsAny<int>()));
        }

        [TestMethod]
        public void InvokeParameterSetNameSucceeds()
        {
            Mock.Arrange(() => Client.IsLoggedIn)
                .IgnoreInstance()
                .Returns(true);

            Mock.Arrange(() => Client.GetEnterprises())
                .IgnoreInstance()
                .Returns(Enterprises)
                .MustBeCalled();

            // this Id does not exist
            var parameters = @"-Name Edgar";
            
            var results = PsCmdletAssert.Invoke(sut, parameters);
            
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            var result = results[0].BaseObject as Enterprise;
            Assert.IsNotNull(result, results[0].BaseObject.GetType().FullName);
            Assert.AreEqual(42, result.Id);
            Assert.AreEqual("Edgar", result.Name);

            Mock.Assert(() => Client.GetEnterprises());
        }

        [TestMethod]
        public void InvokeParameterSetNameSucceedsAndReturnsCollection()
        {
            Mock.Arrange(() => Client.IsLoggedIn)
                .IgnoreInstance()
                .Returns(true);

            Mock.Arrange(() => Client.GetEnterprises())
                .IgnoreInstance()
                .Returns(Enterprises)
                .MustBeCalled();

            // this Id does not exist
            var parameters = @"-Name EnterpriseWithDuplicateName";
            
            var results = PsCmdletAssert.Invoke(sut, parameters);
            
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            var result0 = results[0].BaseObject as Enterprise;
            Assert.IsNotNull(result0, results[0].BaseObject.GetType().FullName);
            Assert.AreEqual("EnterpriseWithDuplicateName".ToLower(), result0.Name.ToLower());

            var result1 = results[1].BaseObject as Enterprise;
            Assert.IsNotNull(result1, results[1].BaseObject.GetType().FullName);
            Assert.AreEqual("EnterpriseWithDuplicateName".ToLower(), result1.Name.ToLower());
            
            Assert.AreNotEqual(result0.Id, result1.Id);

            Mock.Assert(() => Client.GetEnterprises());
        }

    }
}
