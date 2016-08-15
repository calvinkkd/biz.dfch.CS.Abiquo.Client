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
﻿using biz.dfch.CS.Utilities.Testing;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using biz.dfch.CS.Web.Utilities.Rest;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1
{
    [TestClass]
    public class AbiquoClientTest
    {
        private const string ABIQUO_API_BASE_URL = "http://abiquo/api/";
        private const string USERNAME = "ArbitraryUsername";
        private const string PASSWORD = "ArbitraryPassword";
        private const string TENANT_ID = "1";

        [TestMethod]
        [ExpectContractFailure]
        public void LoginWithNullAuthenticationInformationThrowsContractException()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");

            // Act
            abiquoClient.Login(ABIQUO_API_BASE_URL, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void LoginWithNullAbiquoApiBaseUrlThrowsContractException()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);

            // Act
            abiquoClient.Login(null, basicAuthInfo);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void LoginWithEmptyAbiquoApiBaseUrlThrowsContractException()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);

            // Act
            abiquoClient.Login(" ", basicAuthInfo);

            // Assert
        }

        [TestMethod]
        public void LoginWithValidAuthenticationInformationReturnsSuccess()
        {
            // Arrange
            var expectedRequestUri = string.Format("{0}{1}", ABIQUO_API_BASE_URL.TrimEnd('/'), Constants.LOGIN_URL_SUFFIX);
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => new RestCallExecutor(true))
                .Returns(restCallExecutor)
                .OccursOnce();

            Mock.Arrange(() => restCallExecutor
                .Invoke(HttpMethod.Get, expectedRequestUri, basicAuthInfo.GetAuthorizationHeaders(), null))
                    .OccursOnce();

            // Act
            var loginResult = abiquoClient.Login(ABIQUO_API_BASE_URL, basicAuthInfo);
            
            // Assert
            Assert.AreEqual(LoginResultEnum.Success, loginResult);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        public void LoginWithInvalidAuthenticationInformationReturnsNotAuthorized()
        {
            // Arrange


            // Act


            // Assert

        }

        [TestMethod]
        public void LogoutResetsAuthenticationInformationApiBaseUrlAndSetsLoggedInToFalse()
        {
            // Arrange
            var expectedRequestUri = string.Format("{0}{1}", ABIQUO_API_BASE_URL.TrimEnd('/'), Constants.LOGIN_URL_SUFFIX);
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => new RestCallExecutor(true))
                .Returns(restCallExecutor)
                .OccursOnce();

            Mock.Arrange(() => restCallExecutor
                .Invoke(HttpMethod.Get, expectedRequestUri, basicAuthInfo.GetAuthorizationHeaders(), null))
                    .OccursOnce();

            // Act
            var loginResult = abiquoClient.Login(ABIQUO_API_BASE_URL, basicAuthInfo);

            Assert.AreEqual(LoginResultEnum.Success, loginResult);
            Assert.AreEqual(true, abiquoClient.IsLoggedIn);
            Assert.AreEqual(basicAuthInfo, abiquoClient.AuthenticationInformation);
            Assert.AreEqual(ABIQUO_API_BASE_URL, abiquoClient.AbiquoApiBaseUrl);

            abiquoClient.Logout();

            // Assert
            Assert.AreEqual(false, abiquoClient.IsLoggedIn);
            Assert.IsNull(abiquoClient.AuthenticationInformation);
            Assert.IsNull(abiquoClient.AbiquoApiBaseUrl);

            Mock.Assert(restCallExecutor);
        }
    }
}
