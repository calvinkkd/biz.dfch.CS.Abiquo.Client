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
 
﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using biz.dfch.CS.Abiquo.Client.Authentication;
﻿using biz.dfch.CS.Abiquo.Client.Communication;
﻿using biz.dfch.CS.Utilities.Testing;
﻿using biz.dfch.CS.Web.Utilities.Rest;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using biz.dfch.CS.Abiquo.Client.v1;
using biz.dfch.CS.Abiquo.Client.General;
﻿using biz.dfch.CS.Abiquo.Client.v1.Model;

namespace biz.dfch.CS.Abiquo.Client.Tests
{
    [TestClass]
    public class BaseAbiquoClientTest
    {
        private const string ABIQUO_API_BASE_URI = "https://abiquo/api/";
        private const string USERNAME = "ArbitraryUsername";
        private const string PASSWORD = "ArbitraryPassword";
        private const long TENANT_ID = 1;
        private const long INVALID_ID = 0;

        private readonly IAuthenticationInformation authenticationInformation = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);
        private static readonly string BEARER_TOKEN = "Bearer TESTTOKEN";

        [TestMethod]
        [ExpectContractFailure]
        public void InvalidBaseAbqiuoClientThatDoesNotSetVersionPropertyThrowsContractExceptionOnInstantiation()
        {
            // Arrange

            // Act
            new InvalidAbiquoClient();

            // Assert
        }

        [TestMethod]
        public void ExecuteRequestWithoutAdditionalHeadersAndBodyCallsRestCallExecutor()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var expectedRequestUri = UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES);
            
            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, authenticationInformation.GetAuthorizationHeaders(), null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.ExecuteRequest(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        public void ExecuteRequestWithAdditionalHeadersMergesHeadersAndCallsRestCallExecutorWithMergedHeaders()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var expectedRequestUri = UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES);

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.ExecuteRequest(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, headers, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GenericInvokeWithEmptyUriSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            abiquoClient.Invoke<Enterprise>(null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GenericInvokeIfNotLoggedInThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.Invoke<Enterprises>(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null, null);

            // Assert
        }
        
        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithEmptyUriSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            abiquoClient.Invoke(HttpMethod.Get, " ", null, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidUriSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            abiquoClient.Invoke(HttpMethod.Get, "http://example.com", null, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeIfNotLoggedInThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null);

            // Assert
        }

        [TestMethod]
        public void InvokeWithFilterCallsRestCallExecutorWithRequestUriContainingFilterExpression()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var filter = new Dictionary<string, object>()
            {
                {"currentPage", 1},
                {"limit", "25"}
            };

            var expectedRequestUri = string.Format("{0}?{1}", UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES), "currentPage=1&limit=25");

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, filter, headers);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetEnterprise(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetUserWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetUser(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetUserWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetUser(INVALID_ID, 15);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetRoleWith0IdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetRole(INVALID_ID);

            // Assert
        }

        private class DummyAbiquoClient : BaseAbiquoClient
        {
            public DummyAbiquoClient()
            {
                AbiquoApiVersion = "Arbitrary-Version";
            }

            public override bool Login(string abiquoApiBaseUri, IAuthenticationInformation authenticationInformation)
            {
                AbiquoApiBaseUri = abiquoApiBaseUri;
                AuthenticationInformation = authenticationInformation;

                IsLoggedIn = true;

                return true;
            }

            public override Client.v1.Model.Enterprises GetEnterprises()
            {
                return new Enterprises();
            }

            public override Client.v1.Model.Enterprise GetEnterprise(long id)
            {
                return new Enterprise();
            }

            public override Client.v1.Model.UsersWithRoles GetUsersWithRoles()
            {
                return new UsersWithRoles();
            }

            public override Client.v1.Model.User GetUser(long id)
            {
                return new User();
            }

            public override User GetUser(long enterpriseId, long id)
            {
                return new User();
            }

            public override Client.v1.Model.Roles GetRoles()
            {
                return new Roles();
            }

            public override Client.v1.Model.Role GetRole(long id)
            {
                return new Role();
            }
        }

        private class InvalidAbiquoClient : BaseAbiquoClient
        {
            // AbiquoApiVersion intentionally not set in constructor for testing purposes

            public override bool Login(string abiquoApiBaseUri, IAuthenticationInformation authenticationInformation)
            {
                return true;
            }

            public override Client.v1.Model.Enterprises GetEnterprises()
            {
                throw new NotImplementedException();
            }

            public override Client.v1.Model.Enterprise GetEnterprise(long id)
            {
                throw new NotImplementedException();
            }

            public override Client.v1.Model.UsersWithRoles GetUsersWithRoles()
            {
                throw new NotImplementedException();
            }

            public override Client.v1.Model.User GetUser(long id)
            {
                throw new NotImplementedException();
            }

            public override User GetUser(long enterpriseId, long id)
            {
                throw new NotImplementedException();
            }

            public override Client.v1.Model.Roles GetRoles()
            {
                throw new NotImplementedException();
            }

            public override Client.v1.Model.Role GetRole(long id)
            {
                throw new NotImplementedException();
            }
        }
    }
}
