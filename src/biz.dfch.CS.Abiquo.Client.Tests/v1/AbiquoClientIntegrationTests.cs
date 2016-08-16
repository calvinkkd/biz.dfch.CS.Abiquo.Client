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
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Abiquo.Client.Authentication;
using biz.dfch.CS.Abiquo.Client.Factory;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1
{
    [TestClass]
    public class AbiquoClientIntegrationTests
    {
        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LoginWithValidAuthenticationInformationSucceeds()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation(IntegrationTestEnvironment.Username, IntegrationTestEnvironment.Password, IntegrationTestEnvironment.TenantId);

            // Act
            var loginResult = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUrl, basicAuthInfo);

            // Assert
            Assert.AreEqual(LoginResultEnum.Success, loginResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LoginWithInValidAuthenticationInformationResultsInNotAuthorized()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation("invalid-username", "invalid-password", IntegrationTestEnvironment.TenantId);

            // Act
            var loginResult = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUrl, basicAuthInfo);

            // Assert
            Assert.AreEqual(LoginResultEnum.NotAuthorized, loginResult);
        }
    }
}
