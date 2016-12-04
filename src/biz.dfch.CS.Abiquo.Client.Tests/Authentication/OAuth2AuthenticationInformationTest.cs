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

using biz.dfch.CS.Abiquo.Client.Authentication;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Abiquo.Client.Tests.Authentication
{
    [TestClass]
    public class OAuth2AuthenticationInformationTest
    {
        private const string OAUTH2_TOKEN = "ARBITRARY_OAUTH2_TOKEN";
        private const int TENANT_ID = 1;

        [TestMethod]
        [ExpectContractFailure]
        public void CreateOAuth2AuthenticationInformationWithNullTokenThrowsContractException()
        {
            // Arrange

            // Act
            new OAuth2AuthenticationInformation(null, TENANT_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateOAuth2AuthenticationInformationWithEmptyTokenThrowsContractException()
        {
            // Arrange

            // Act
            new OAuth2AuthenticationInformation(" ", TENANT_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateOAuth2AuthenticationInformationWithInvalidTenantIdThrowsContractException()
        {
            // Arrange

            // Act
            new OAuth2AuthenticationInformation(OAUTH2_TOKEN, 0);

            // Assert
        }

        [TestMethod]
        public void GetAuthenticationHeadersReturnsOAuth2AuthenticationHeader()
        {
            // Arrange
            var oAuth2AuthInfo = new OAuth2AuthenticationInformation(OAUTH2_TOKEN, TENANT_ID);

            // Act
            var authHeaders = oAuth2AuthInfo.GetAuthorizationHeaders();

            // Assert
            Assert.IsNotNull(authHeaders);
            Assert.AreEqual(1, authHeaders.Keys.Count);

            Assert.AreEqual("Bearer ARBITRARY_OAUTH2_TOKEN", authHeaders[Client.Authentication.Constants.AUTHORIZATION_HEADER_KEY]);
        }

        [TestMethod]
        public void GetTenantIdReturnsTenantId()
        {
            // Arrange
            var oAuth2AuthInfo = new OAuth2AuthenticationInformation(OAUTH2_TOKEN, TENANT_ID);

            // Act
            var tenantId = oAuth2AuthInfo.GetTenantId();

            // Assert
            Assert.AreEqual(TENANT_ID, tenantId);
        }
    }
}
