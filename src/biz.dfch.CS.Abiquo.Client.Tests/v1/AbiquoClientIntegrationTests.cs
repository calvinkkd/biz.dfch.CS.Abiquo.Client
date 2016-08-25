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
﻿using System.Runtime.CompilerServices;
﻿using System.Text;
using System.Threading.Tasks;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Abiquo.Client.Authentication;
﻿using biz.dfch.CS.Abiquo.Client.Communication;
﻿using biz.dfch.CS.Abiquo.Client.Factory;
using biz.dfch.CS.Abiquo.Client.v1;
using biz.dfch.CS.Web.Utilities.Rest;
﻿using Newtonsoft.Json;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1
{
    [TestClass]
    public class AbiquoClientIntegrationTests
    {
        private const string ABIQUO_CLIENT_VERSION = "v1";

        private readonly IAuthenticationInformation basicAuthenticationInformation = new BasicAuthenticationInformation(IntegrationTestEnvironment.Username, IntegrationTestEnvironment.Password, IntegrationTestEnvironment.TenantId);

        #region Login

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LoginWithValidBasicAuthenticationInformationReturnsTrue()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);

            // Act
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            // Assert
            Assert.IsTrue(loginSucceeded);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LoginWithInValidBasicAuthenticationInformationReturnsFalse()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var basicAuthInfo = new BasicAuthenticationInformation("invalid-username", "invalid-password", IntegrationTestEnvironment.TenantId);

            // Act
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthInfo);

            // Assert
            Assert.IsFalse(loginSucceeded);
        }

        #endregion Login

        #region Enterprises

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetEnterprisesReturnsAbiquoEnterprises()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var headers = new Dictionary<string, string>()
            {
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            // Act
            var result = abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, headers);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CreateNewAbiquoEnterpriseAndDeleteTheNewCreatedEnterpriseSucceeds()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var headers = new Dictionary<string, string>()
            {
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISE }
                ,
                { Constants.CONTENT_TYPE_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISE }
            };

            var enterpriseName = Guid.NewGuid().ToString();

            var body = new Dictionary<string, object>()
            {
                { "cpuCountHardLimit", 2 }
                ,
                { "diskHardLimitInMb", 2 }
                ,
                { "isReservationRestricted", false }
                ,
                { "twoFactorAuthenticationMandatory", false }
                ,
                { "ramSoftLimitInMb", 1 }
                ,
                { "links", new string[0] }
                ,
                { "workflow", false }
                ,
                { "vlansHard", 0 }
                ,
                { "publicIpsHard", 0 }
                ,
                { "publicIpsSoft", 0 }
                ,
                { "ramHardLimitInMb", 2 }
                ,
                { "vlansSoft", 0 }
                ,
                { "cpuCountSoftLimit", 1 }
                ,
                { "diskSoftLimitInMb", 1 }
                ,
                { "name", enterpriseName }
            };

            var jsonBody = JsonConvert.SerializeObject(body);

            // Act
            var creationResult = abiquoClient.Invoke(HttpMethod.Post, AbiquoUriSuffixes.ENTERPRISES, null, headers, jsonBody);

            var resultingEnterprise = JsonConvert.DeserializeObject<dynamic>(creationResult);

            var requestUriSuffix = string.Format(AbiquoUriSuffixes.ENTERPRISE_BY_ID, resultingEnterprise.id.ToString());
            var deletionResult = abiquoClient.Invoke(HttpMethod.Delete, requestUriSuffix, null, headers);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(creationResult);
            Assert.IsNotNull(deletionResult);
        }

        #endregion Enterprises

        #region Users

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUsersReturnsAbiquoUsersWithRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var headers = new Dictionary<string, string>()
            {
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_USERSWITHROLES }
            };

            // Act
            var requestUriSuffix = string.Format(AbiquoUriSuffixes.USERSWITHROLES_BY_ENTERPRISE_ID, IntegrationTestEnvironment.TenantId);
            var result = abiquoClient.Invoke(HttpMethod.Get, requestUriSuffix, null, headers);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(result);
        }

        #endregion Users

        #region Roles

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetRolesSucceedsReturnsAbiquoRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var headers = new Dictionary<string, string>()
            {
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ROLES }
            };

            // Act
            var result = abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ROLES, null, headers);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(result);
        }

        #endregion Roles
    }
}
