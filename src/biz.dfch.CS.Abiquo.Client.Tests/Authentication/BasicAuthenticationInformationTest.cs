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

namespace biz.dfch.CS.Abiquo.Client.Tests.Authentication
{
    [TestClass]
    public class BasicAuthenticationInformationTest
    {
        private const string USERNAME = "ArbitraryUsername";
        private const string PASSWORD = "ArbitraryPassword";
        private const string TENANT_ID = "1";

        [TestMethod]
        [ExpectContractFailure]
        public void CreateBasicAuthenticationInformationWithNullUsernameThrowsContractException()
        {
            // Arrange/Act
            new BasicAuthenticationInformation(null, PASSWORD, TENANT_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateBasicAuthenticationInformationWithEmptyUsernameThrowsContractException()
        {
            // Arrange/Act
            new BasicAuthenticationInformation(" ", PASSWORD, TENANT_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateBasicAuthenticationInformationWithNullPasswordThrowsContractException()
        {
            // Arrange/Act
            new BasicAuthenticationInformation(USERNAME, null, TENANT_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateBasicAuthenticationInformationWithEmptyPasswordThrowsContractException()
        {
            // Arrange/Act
            new BasicAuthenticationInformation(USERNAME, " ", TENANT_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateBasicAuthenticationInformationWithNullTenantIdThrowsContractException()
        {
            // Arrange/Act
            new BasicAuthenticationInformation(USERNAME, PASSWORD, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateBasicAuthenticationInformationWithEmptyTenantIdThrowsContractException()
        {
            // Arrange/Act
            new BasicAuthenticationInformation(USERNAME, PASSWORD, " ");

            // Assert
        }

        [TestMethod]
        public void GetAuthenticationHeadersReturnsBasicAuthenticationHeader()
        {
            // Arrange
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);

            // Act
            var authHeaders = basicAuthInfo.GetAuthorizationHeaders();

            // Assert
            Assert.IsNotNull(authHeaders);
            Assert.AreEqual(1, authHeaders.Keys.Count);

            Assert.AreEqual("Basic QXJiaXRyYXJ5VXNlcm5hbWU6QXJiaXRyYXJ5UGFzc3dvcmQ=", authHeaders[Constants.AUTHORIZATION_HEADER_KEY]);
        }

        [TestMethod]
        public void GetTenantIdReturnsTenantId()
        {
            // Arrange
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);
            
            // Act
            var tenantId = basicAuthInfo.GetTenantId();

            // Assert
            Assert.IsNotNull(tenantId);
            Assert.AreEqual(TENANT_ID, tenantId);
        }
    }
}
