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

namespace biz.dfch.CS.Abiquo.Client.Tests.v1
{
    [TestClass]
    public class AbiquoClientTest
    {
        private const string ABIQUO_BASE_URL = "http://abiquo/api";
        private const string USERNAME = "ArbitraryUsername";
        private const string PASSWORD = "ArbitraryPassword";
        private const string TENANT_ID = "1";

        [TestMethod]
        [ExpectContractFailure]
        public void LoginWithNullAuthenticationInformationThrowsContractException()
        {
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            abiquoClient.Login(ABIQUO_BASE_URL, null);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void LoginWithNullAbiquoBaseUrlThrowsContractException()
        {
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);
            abiquoClient.Login(null, basicAuthInfo);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void LoginWithEmptyAbiquoBaseUrlThrowsContractException()
        {
            var abiquoClient = AbiquoClientFactory.GetByVersion("v1");
            var basicAuthInfo = new BasicAuthenticationInformation(USERNAME, PASSWORD, TENANT_ID);
            abiquoClient.Login(" ", basicAuthInfo);
        }

        [TestMethod]
        public void LoginWithValidAuthenticationInformationSucceeds()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void LoginWithInvalidAuthenticationInformationReturnsNotAuthorized()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
