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

namespace biz.dfch.CS.Abiquo.Client.Tests
{
    [TestClass]
    public class BaseAbiquoClientTest
    {
        private const string ABIQUO_API_BASE_URL = "https://abiquo/api/";
        private const string URL_SUFFIX = "/enterprises";
        private const string USERNAME = "ArbitraryUsername";
        private const string PASSWORD = "ArbitraryPassword";
        private const string TENANT_ID = "1";

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
        [ExpectContractFailure]
        public void ExecuteRequestWithHttpPutAndValidUrlSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.ExecuteRequest(HttpMethod.Put, AbiquoUrlSuffixes.LOGIN);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ExecuteRequestWithInvalidUrlSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.ExecuteRequest(HttpMethod.Get, " ");

            // Assert
        }

        [TestMethod]
        public void ExecuteRequestWithoutAdditionalHeadersAndBodyCallsRestCallExecutor()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URL, authenticationInformation);

            var expectedRequestUrl = UrlHelper.ConcatUrl(ABIQUO_API_BASE_URL, URL_SUFFIX);
            
            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUrl, authenticationInformation.GetAuthorizationHeaders(), null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.ExecuteRequest(HttpMethod.Get, URL_SUFFIX, null, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        public void ExecuteRequestWithAdditionalHeadersMergesHeadersAndCallsRestCallExecutorWithMergedHeaders()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URL, authenticationInformation);

            var expectedRequestUrl = UrlHelper.ConcatUrl(ABIQUO_API_BASE_URL, URL_SUFFIX);

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUrl, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.ExecuteRequest(HttpMethod.Get, URL_SUFFIX, headers, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithEmptyUrlSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URL, authenticationInformation);

            // Act
            abiquoClient.Invoke(HttpMethod.Get, " ", null, null, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidUrlSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URL, authenticationInformation);

            // Act
            abiquoClient.Invoke(HttpMethod.Get, "http://example.com", null, null, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeIfNotLoggedInThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.Invoke(HttpMethod.Get, URL_SUFFIX, null, null, null);

            // Assert
        }

        [TestMethod]
        public void InvokeWithFilterCallsRestCallExecutorWithRequestUrlContainingFilterExpression()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URL, authenticationInformation);

            var filter = new Dictionary<string, object>()
            {
                {"currentPage", 1},
                {"limit", "25"}
            };

            var expectedRequestUrl = string.Format("{0}?{1}", UrlHelper.ConcatUrl(ABIQUO_API_BASE_URL, URL_SUFFIX), "currentPage=1&limit=25");

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUrl, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.Invoke(HttpMethod.Get, URL_SUFFIX, filter, headers, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        private class DummyAbiquoClient : BaseAbiquoClient
        {
            public DummyAbiquoClient()
            {
                AbiquoApiVersion = "Arbitrary-Version";
            }

            public override bool Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation)
            {
                AbiquoApiBaseUrl = abiquoApiBaseUrl;
                AuthenticationInformation = authenticationInformation;

                IsLoggedIn = true;

                return true;
            }
        }

        private class InvalidAbiquoClient : BaseAbiquoClient
        {
            // AbiquoApiVersion intentionally not set in constructor for testing purposes

            public override bool Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation)
            {
                return true;
            }
        }
    }
}
