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

namespace biz.dfch.CS.Abiquo.Client.Tests
{
    [TestClass]
    public class BaseAbiquoClientTest
    {
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
            abiquoClient.ExecuteRequest(HttpMethod.Put, AbiquoUrlSuffix.LOGIN);

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
        [ExpectContractFailure]
        public void ExecuteRequestWithHttpPostAndValidUrlSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.ExecuteRequest(HttpMethod.Post, AbiquoUrlSuffix.LOGIN);

            // Assert
        }



        private class DummyAbiquoClient : BaseAbiquoClient
        {
            public DummyAbiquoClient()
            {
                AbiquoApiVersion = "Arbitrary-Version";
            }

            public override bool Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation)
            {
                IsLoggedIn = true;
                AbiquoApiBaseUrl = "https://arbitrary/base/url";

                return true;
            }
        }

        private class InvalidAbiquoClient : BaseAbiquoClient
        {
            public override bool Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation)
            {
                return true;
            }
        }
    }
}
