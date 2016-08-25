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
using biz.dfch.CS.Abiquo.Client.General;

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
        public void InvokeGetEnterprisesReturnsAbiquoEnterprises()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES).GetHeaders();

            // Act
            var result = abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, headers);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetEnterprisesReturnsAbiquoEnterprises()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            // Act
            var enterprises = abiquoClient.GetEnterprises();

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(enterprises);
            Assert.IsNotNull(enterprises.Collection);
            Assert.IsTrue(enterprises.TotalSize > 0);
            Assert.IsTrue(enterprises.Links.Count > 0);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetCurrentEnterpriseReturnsAbiquoEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            // Act
            var currentEnterprise = abiquoClient.GetCurrentEnterprise();

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(currentEnterprise);
            Assert.AreEqual(basicAuthenticationInformation.GetTenantId(), currentEnterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetEnterpriseReturnsAbiquoEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            // Act
            var enterprise = abiquoClient.GetEnterprise(basicAuthenticationInformation.GetTenantId());

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(enterprise);
            Assert.AreEqual(basicAuthenticationInformation.GetTenantId(), enterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void InvokeNewAbiquoEnterpriseAndDeleteTheNewCreatedEnterpriseSucceeds()
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
        public void InvokeGetUsersReturnsAbiquoUsersWithRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_USERSWITHROLES).GetHeaders();

            // Act
            var requestUriSuffix = string.Format(AbiquoUriSuffixes.USERSWITHROLES_BY_ENTERPRISE_ID, IntegrationTestEnvironment.TenantId);
            var result = abiquoClient.Invoke(HttpMethod.Get, requestUriSuffix, null, headers);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUsersWithRolesOfCurrentEnterpriseReturnsAbiquoUsersWithRolesOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            // Act
            var usersWithRoles = abiquoClient.GetUsersWithRolesOfCurrentEnterprise();

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(usersWithRoles);
            Assert.IsNotNull(usersWithRoles.Collection);
            Assert.IsTrue(usersWithRoles.TotalSize > 0);
            Assert.IsTrue(usersWithRoles.Links.Count > 0);

            var userWithRole = usersWithRoles.Collection.FirstOrDefault();
            Assert.IsNotNull(userWithRole);
            Assert.AreEqual(IntegrationTestEnvironment.TenantId, userWithRole.Enterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUsersWithRolesReturnsAbiquoUsersWithRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            // Act
            var usersWithRoles = abiquoClient.GetUsersWithRoles(IntegrationTestEnvironment.TenantId);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(usersWithRoles);
            Assert.IsNotNull(usersWithRoles.Collection);
            Assert.IsTrue(usersWithRoles.TotalSize > 0);
            Assert.IsTrue(usersWithRoles.Links.Count > 0);

            var user = usersWithRoles.Collection.FirstOrDefault();
            Assert.IsNotNull(user);
            Assert.AreEqual(IntegrationTestEnvironment.TenantId, user.Enterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUserOfCurrentEnterpriseReturnsAbiquoUserOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var usersWithRoles = abiquoClient.GetUsersWithRoles(IntegrationTestEnvironment.TenantId);
            var expectedUser = usersWithRoles.Collection.FirstOrDefault();

            // Act
            var user = abiquoClient.GetUserOfCurrentEnterprise(expectedUser.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(user);
            Assert.AreEqual(expectedUser.Id, user.Id);
            Assert.AreEqual(expectedUser.Active, user.Active);
            Assert.AreEqual(expectedUser.AuthType, user.AuthType);
            Assert.AreEqual(expectedUser.Description, user.Description);
            Assert.AreEqual(expectedUser.Email, user.Email);
            Assert.AreEqual(expectedUser.FirstLogin, user.FirstLogin);
            Assert.AreEqual(expectedUser.Locale, user.Locale);
            Assert.AreEqual(expectedUser.Locked, user.Locked);
            Assert.AreEqual(expectedUser.Nick, user.Nick);
            Assert.AreEqual(expectedUser.Surname, user.Surname);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUserReturnsAbiquoUser()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var usersWithRoles = abiquoClient.GetUsersWithRoles(IntegrationTestEnvironment.TenantId);
            var expectedUser = usersWithRoles.Collection.FirstOrDefault();

            // Act
            var user = abiquoClient.GetUser(IntegrationTestEnvironment.TenantId, expectedUser.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(user);
            Assert.AreEqual(expectedUser.Id, user.Id);
            Assert.AreEqual(expectedUser.Active, user.Active);
            Assert.AreEqual(expectedUser.AuthType, user.AuthType);
            Assert.AreEqual(expectedUser.Description, user.Description);
            Assert.AreEqual(expectedUser.Email, user.Email);
            Assert.AreEqual(expectedUser.FirstLogin, user.FirstLogin);
            Assert.AreEqual(expectedUser.Locale, user.Locale);
            Assert.AreEqual(expectedUser.Locked, user.Locked);
            Assert.AreEqual(expectedUser.Nick, user.Nick);
            Assert.AreEqual(expectedUser.Surname, user.Surname);
        }

        #endregion Users


        #region Roles

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void InvokeGetRolesSucceedsReturnsAbiquoRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_ROLES).GetHeaders();

            // Act
            var result = abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ROLES, null, headers);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetRolesReturnsAbiquoRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            // Act
            var roles = abiquoClient.GetRoles();

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(roles);
            Assert.IsNotNull(roles.Collection);
            Assert.IsTrue(roles.TotalSize > 0);
            Assert.IsTrue(roles.Links.Count > 0);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetRoleReturnsAbiquoRole()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(ABIQUO_CLIENT_VERSION);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, basicAuthenticationInformation);

            var roles = abiquoClient.GetRoles();
            var expectedRole = roles.Collection.FirstOrDefault();

            // Act
            var role = abiquoClient.GetRole(expectedRole.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(role);
            Assert.AreEqual(expectedRole.Id, role.Id);
            Assert.AreEqual(expectedRole.Blocked, role.Blocked);
            Assert.AreEqual(expectedRole.IdEnterprise, role.IdEnterprise);
            Assert.AreEqual(expectedRole.Ldap, role.Ldap);
        }

        #endregion Roles
    }
}
