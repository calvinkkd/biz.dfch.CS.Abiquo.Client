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
﻿using System.Net.Http;
﻿using System.Text;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Abiquo.Client.Authentication;
﻿using biz.dfch.CS.Abiquo.Client.Communication;
﻿using biz.dfch.CS.Abiquo.Client.Factory;
using biz.dfch.CS.Abiquo.Client.v1;
﻿using Newtonsoft.Json;
using biz.dfch.CS.Abiquo.Client.General;
﻿using biz.dfch.CS.Abiquo.Client.v1.Model;
﻿using HttpMethod = biz.dfch.CS.Web.Utilities.Rest.HttpMethod;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1
{
    [TestClass]
    public class AbiquoClientIntegrationTests
    {
        private readonly IAuthenticationInformation BasicAuthenticationInformation = new BasicAuthenticationInformation(IntegrationTestEnvironment.Username, IntegrationTestEnvironment.Password, IntegrationTestEnvironment.TenantId);
        private const string SAMPLE_VIRTUAL_MACHINE_PASSWORD = "SamplePw";
        private const string SAMPLE_VIRTUAL_MACHINE_NAME = "Abiquo Client TestVM";

        private VirtualMachineState virtualMachineOffState = new VirtualMachineState()
        {
            State = VirtualMachineStateEnum.OFF
        };

        private VirtualMachineState virtualMachineOnState = new VirtualMachineState()
        {
            State = VirtualMachineStateEnum.ON
        };

        #region Login

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LoginWithValidBasicAuthenticationInformationReturnsTrue()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);

            // Act
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Assert
            Assert.IsTrue(loginSucceeded);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void LoginWithInValidBasicAuthenticationInformationReturnsFalse()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
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
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

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
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var enterprises = abiquoClient.GetEnterprises();

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(enterprises);
            Assert.IsNotNull(enterprises.Collection);
            Assert.IsTrue(0 < enterprises.TotalSize);
            Assert.IsTrue(0 < enterprises.Links.Count);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetCurrentEnterpriseReturnsCurrentAbiquoEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var currentEnterprise = abiquoClient.GetCurrentEnterprise();

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(currentEnterprise);
            Assert.AreEqual(BasicAuthenticationInformation.GetTenantId(), currentEnterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetEnterpriseReturnsExpectedAbiquoEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var enterprise = abiquoClient.GetEnterprise(BasicAuthenticationInformation.GetTenantId());

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(enterprise);
            Assert.AreEqual(BasicAuthenticationInformation.GetTenantId(), enterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        [ExpectedException(typeof(HttpRequestException))]
        public void GetInexistentEnterpriseThrowsException()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            abiquoClient.GetEnterprise(5000);

            // Assert
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void InvokeNewAbiquoEnterpriseAndDeleteTheNewCreatedEnterpriseSucceeds()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var headers = new Dictionary<string, string>()
            {
                { AbiquoHeaderKeys.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISE }
                ,
                { AbiquoHeaderKeys.CONTENT_TYPE_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISE }
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
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

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
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var usersWithRoles = abiquoClient.GetUsersWithRolesOfCurrentEnterprise();

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(usersWithRoles);
            Assert.IsNotNull(usersWithRoles.Collection);
            Assert.IsTrue(0 < usersWithRoles.TotalSize);
            Assert.IsTrue(0 < usersWithRoles.Links.Count);

            var userWithRole = usersWithRoles.Collection.FirstOrDefault();
            Assert.IsNotNull(userWithRole);
            Assert.AreEqual(IntegrationTestEnvironment.TenantId, userWithRole.Enterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUsersWithRolesReturnsAbiquoUsersWithRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var usersWithRoles = abiquoClient.GetUsersWithRoles(IntegrationTestEnvironment.TenantId);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsNotNull(usersWithRoles);
            Assert.IsNotNull(usersWithRoles.Collection);
            Assert.IsTrue(0 < usersWithRoles.TotalSize);
            Assert.IsTrue(0 < usersWithRoles.Links.Count);

            var user = usersWithRoles.Collection.FirstOrDefault();
            Assert.IsNotNull(user);
            Assert.AreEqual(IntegrationTestEnvironment.TenantId, user.Enterprise.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUserOfCurrentEnterpriseReturnsExpectedAbiquoUserOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var usersWithRoles = abiquoClient.GetUsersWithRoles(IntegrationTestEnvironment.TenantId);
            var expectedUserWithRoles = usersWithRoles.Collection.FirstOrDefault();
            Contract.Assert(null != expectedUserWithRoles);

            // Act
            var userWithRoles = abiquoClient.GetUserOfCurrentEnterprise(expectedUserWithRoles.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(userWithRoles);
            Assert.AreEqual(expectedUserWithRoles.Id, userWithRoles.Id);
            Assert.AreEqual(expectedUserWithRoles.Active, userWithRoles.Active);
            Assert.AreEqual(expectedUserWithRoles.AuthType, userWithRoles.AuthType);
            Assert.AreEqual(expectedUserWithRoles.AvailableVirtualDatacenters, userWithRoles.AvailableVirtualDatacenters);
            Assert.AreEqual(expectedUserWithRoles.Description, userWithRoles.Description);
            Assert.AreEqual(expectedUserWithRoles.Email, userWithRoles.Email);
            Assert.AreEqual(expectedUserWithRoles.Locale, userWithRoles.Locale);
            Assert.AreEqual(expectedUserWithRoles.FirstLogin, userWithRoles.FirstLogin);
            Assert.AreEqual(expectedUserWithRoles.Nick, userWithRoles.Nick);
            Assert.AreEqual(expectedUserWithRoles.Password, userWithRoles.Password);
            Assert.AreEqual(expectedUserWithRoles.Surname, userWithRoles.Surname);
            Assert.AreEqual(expectedUserWithRoles.Locked, userWithRoles.Locked);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetUserReturnsExpectedAbiquoUser()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var usersWithRoles = abiquoClient.GetUsersWithRoles(IntegrationTestEnvironment.TenantId);
            var expectedUserWithRoles = usersWithRoles.Collection.FirstOrDefault();
            Contract.Assert(null != expectedUserWithRoles);

            // Act
            var userWithRoles = abiquoClient.GetUser(IntegrationTestEnvironment.TenantId, expectedUserWithRoles.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(userWithRoles);
            Assert.AreEqual(expectedUserWithRoles.Id, userWithRoles.Id);
            Assert.AreEqual(expectedUserWithRoles.Active, userWithRoles.Active);
            Assert.AreEqual(expectedUserWithRoles.AuthType, userWithRoles.AuthType);
            Assert.AreEqual(expectedUserWithRoles.AvailableVirtualDatacenters, userWithRoles.AvailableVirtualDatacenters);
            Assert.AreEqual(expectedUserWithRoles.Description, userWithRoles.Description);
            Assert.AreEqual(expectedUserWithRoles.Email, userWithRoles.Email);
            Assert.AreEqual(expectedUserWithRoles.Locale, userWithRoles.Locale);
            Assert.AreEqual(expectedUserWithRoles.FirstLogin, userWithRoles.FirstLogin);
            Assert.AreEqual(expectedUserWithRoles.Nick, userWithRoles.Nick);
            Assert.AreEqual(expectedUserWithRoles.Password, userWithRoles.Password);
            Assert.AreEqual(expectedUserWithRoles.Surname, userWithRoles.Surname);
            Assert.AreEqual(expectedUserWithRoles.Locked, userWithRoles.Locked);
        }

        #endregion Users


        #region Roles

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void InvokeGetRolesSucceedsReturnsAbiquoRoles()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

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
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var roles = abiquoClient.GetRoles();

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(roles);
            Assert.IsNotNull(roles.Collection);
            Assert.IsTrue(0 < roles.TotalSize);
            Assert.IsTrue(0 < roles.Links.Count);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetRoleReturnsExpectedAbiquoRole()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var roles = abiquoClient.GetRoles();
            var expectedRole = roles.Collection.FirstOrDefault();
            Contract.Assert(null != expectedRole);

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


        #region DataCentersLimits

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetDataCentersLimitsOfCurrentEnterpriseReturnsAbiquoLimitsOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var dataCentersLimits = abiquoClient.GetDataCentersLimitsOfCurrentEnterprise();

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(dataCentersLimits);
            Assert.IsNotNull(dataCentersLimits.Collection);
            Assert.IsTrue(0 < dataCentersLimits.Collection.Count);
            Assert.IsNotNull(dataCentersLimits.Links);

            var dataCenterLimits = dataCentersLimits.Collection.First();
            Assert.IsTrue(dataCenterLimits.IsValid());
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetDataCenterLimitsOfCurrentEnterpriseReturnsExpectedAbiquoDataCenterLimitsOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCentersLimits = abiquoClient.GetDataCentersLimitsOfCurrentEnterprise();
            Contract.Assert(null != dataCentersLimits);
            var expectedDataCenterLimits = dataCentersLimits.Collection.First();

            // Act
            var dataCenterLimits = abiquoClient.GetDataCenterLimitsOfCurrentEnterprise(expectedDataCenterLimits.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(dataCenterLimits);
            Assert.IsTrue(dataCenterLimits.IsValid());
            Assert.AreEqual(expectedDataCenterLimits.Id, dataCenterLimits.Id);
            Assert.AreEqual(expectedDataCenterLimits.CpuCountHardLimit, dataCenterLimits.CpuCountHardLimit);
            Assert.AreEqual(expectedDataCenterLimits.CpuCountSoftLimit, dataCenterLimits.CpuCountSoftLimit);
            Assert.AreEqual(expectedDataCenterLimits.DiskHardLimitInMb, dataCenterLimits.DiskHardLimitInMb);
            Assert.AreEqual(expectedDataCenterLimits.DiskSoftLimitInMb, dataCenterLimits.DiskSoftLimitInMb);
            Assert.AreEqual(expectedDataCenterLimits.PublicIpsHard, dataCenterLimits.PublicIpsHard);
            Assert.AreEqual(expectedDataCenterLimits.PublicIpsSoft, dataCenterLimits.PublicIpsSoft);
            Assert.AreEqual(expectedDataCenterLimits.RamHardLimitInMb, dataCenterLimits.RamHardLimitInMb);
            Assert.AreEqual(expectedDataCenterLimits.RamSoftLimitInMb, dataCenterLimits.RamSoftLimitInMb);
            Assert.AreEqual(expectedDataCenterLimits.RepositoryHardInMb, dataCenterLimits.RepositoryHardInMb);
            Assert.AreEqual(expectedDataCenterLimits.RepositorySoftInMb, dataCenterLimits.RepositorySoftInMb);
            Assert.AreEqual(expectedDataCenterLimits.StorageHardInMb, dataCenterLimits.StorageHardInMb);
            Assert.AreEqual(expectedDataCenterLimits.StorageSoftInMb, dataCenterLimits.StorageSoftInMb);
            Assert.AreEqual(expectedDataCenterLimits.VlansHard, dataCenterLimits.VlansHard);
            Assert.AreEqual(expectedDataCenterLimits.VlansSoft, dataCenterLimits.VlansSoft);
        }

        #endregion DataCentersLimits


        #region VirtualMachines

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetAllVirtualMachinesReturnsAllAbiquoVirtualMachinesOfUser()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var virtualMachines = abiquoClient.GetAllVirtualMachines();

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualMachines);
            Assert.IsNotNull(virtualMachines.Collection);
            Assert.IsTrue(0 < virtualMachines.TotalSize);
            Assert.IsTrue(0 < virtualMachines.Links.Count);

            var virtualMachine = virtualMachines.Collection.FirstOrDefault();
            Assert.IsNotNull(virtualMachine);
            Assert.IsTrue(0 < virtualMachine.Id);
            Assert.IsNotNull(virtualMachine.Name);
            Assert.IsTrue(0 < virtualMachine.Cpu);
            Assert.IsTrue(0 < virtualMachine.Ram);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualMachinesReturnsAbiquoVirtualMachines()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            // Act
            var virtualMachines = abiquoClient.GetVirtualMachines(virtualDataCenter.Id, virtualAppliance.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualMachines);
            Assert.IsNotNull(virtualMachines.Collection);
            Assert.IsTrue(0 < virtualMachines.TotalSize);
            Assert.IsTrue(0 < virtualMachines.Links.Count);

            var virtualMachine = virtualMachines.Collection.FirstOrDefault();
            Assert.IsNotNull(virtualMachine);
            Assert.IsTrue(0 < virtualMachine.Id);
            Assert.IsNotNull(virtualMachine.Name);
            Assert.IsTrue(0 < virtualMachine.Cpu);
            Assert.IsTrue(0 < virtualMachine.Ram);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualMachineReturnsExpectedAbiquoVirtualMachine()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);
            
            var virtualMachines = abiquoClient.GetVirtualMachines(virtualDataCenter.Id, virtualAppliance.Id);
            var expectedVirtualMachine = virtualMachines.Collection.FirstOrDefault();
            Contract.Assert(null != expectedVirtualMachine);

            // Act
            var virtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                expectedVirtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualMachine);
            Assert.IsTrue(0 < virtualMachine.Id);
            Assert.AreEqual(expectedVirtualMachine.Id, virtualMachine.Id);
            Assert.AreEqual(expectedVirtualMachine.Name, virtualMachine.Name);
            Assert.AreEqual(expectedVirtualMachine.CoresPerSocket, virtualMachine.CoresPerSocket);
            Assert.AreEqual(expectedVirtualMachine.Cpu, virtualMachine.Cpu);
            Assert.AreEqual(expectedVirtualMachine.Ram, virtualMachine.Ram);
            Assert.AreEqual(expectedVirtualMachine.Description, virtualMachine.Description);
            Assert.AreEqual(expectedVirtualMachine.HighDisponibility, virtualMachine.HighDisponibility);
            Assert.AreEqual(expectedVirtualMachine.IdState, virtualMachine.IdState);
            Assert.AreEqual(expectedVirtualMachine.IdType, virtualMachine.IdType);
            Assert.AreEqual(expectedVirtualMachine.Label, virtualMachine.Label);
            Assert.AreEqual(expectedVirtualMachine.Monitored, virtualMachine.Monitored);
            Assert.AreEqual(expectedVirtualMachine.Protected, virtualMachine.Protected);
            Assert.AreEqual(expectedVirtualMachine.State, virtualMachine.State);
            Assert.AreEqual(expectedVirtualMachine.Uuid, virtualMachine.Uuid);
            Assert.AreEqual(expectedVirtualMachine.VdrpIp, virtualMachine.VdrpIp);
            Assert.AreEqual(expectedVirtualMachine.VdrpEnabled, virtualMachine.VdrpEnabled);
            Assert.AreEqual(expectedVirtualMachine.VdrpPort, virtualMachine.VdrpPort);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CreateVirtualMachineWithoutCustomConfigurationCreatesAbiquoVirtualMachineBasedOnTemplate()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            // Act
            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualMachine);
            Assert.IsTrue(0 < virtualMachine.Id);
            Assert.AreNotEqual(virtualMachineTemplate.Name, virtualMachine.Name);
            Assert.AreEqual(virtualMachineTemplate.CpuRequired, virtualMachine.Cpu);
            Assert.AreEqual(virtualMachineTemplate.RamRequired, virtualMachine.Ram);
            Assert.IsNotNull(virtualMachine.Password);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }
        
        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void CreateVirtualMachineWithCustomConfigurationCreatesAbiquoVirtualMachineBasedOnTemplateAndCustomConfiguration()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachineToBeCreated = new VirtualMachine()
            {
                Cpu = 2
                ,
                Ram = 1024
                ,
                Password = SAMPLE_VIRTUAL_MACHINE_PASSWORD
                ,
                Name = SAMPLE_VIRTUAL_MACHINE_NAME
            };

            // Act
            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id, virtualMachineToBeCreated);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualMachine);
            Assert.IsTrue(0 < virtualMachine.Id);
            Assert.AreEqual(virtualMachineToBeCreated.Cpu, virtualMachine.Cpu);
            Assert.AreEqual(virtualMachineToBeCreated.Ram, virtualMachine.Ram);
            Assert.IsNotNull(virtualMachine.Password);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void DeployVirtualMachineWithForceAndWaitForCompletionDeploysAbiquoVirtualMachineAndReturnsSuccessfullyCompletedDeployTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            // Act
            var deployTask = abiquoClient.DeployVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true, true);
            
            // Assert
            Assert.IsTrue(loginSucceeded);
            
            Assert.IsFalse(string.IsNullOrWhiteSpace(deployTask.TaskId));
            Assert.IsTrue(0 < deployTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, deployTask.State);
            Assert.AreEqual(TaskTypeEnum.DEPLOY, deployTask.Type);
            
            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void DeployVirtualMachineWithWaitForCompletionDeploysAbiquoVirtualMachineAndReturnsSuccessfullyCompletedDeployTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            // Act
            var deployTask = abiquoClient.DeployVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), false, true);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(deployTask.TaskId));
            Assert.IsTrue(0 < deployTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, deployTask.State);
            Assert.AreEqual(TaskTypeEnum.DEPLOY, deployTask.Type);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void DeployVirtualMachineDeploysAbiquoVirtualMachineAndReturnsStartedDeployTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            // Act
            var deployTask = abiquoClient.DeployVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), false);

            var completedTask = abiquoClient.WaitForTaskCompletion(deployTask,
                abiquoClient.TaskPollingWaitTimeMilliseconds, abiquoClient.TaskPollingTimeoutMilliseconds);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(deployTask.TaskId));
            Assert.IsTrue(0 < deployTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.STARTED, deployTask.State);
            Assert.AreEqual(TaskTypeEnum.DEPLOY, deployTask.Type);

            Assert.IsFalse(string.IsNullOrWhiteSpace(completedTask.TaskId));
            Assert.AreEqual(deployTask.TaskId, completedTask.TaskId);
            Assert.IsTrue(0 < completedTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, completedTask.State);
            Assert.AreEqual(TaskTypeEnum.DEPLOY, completedTask.Type);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void UpdateVirtualMachineWithForceAndWaitForTaskCompletionUpdatesAbiquoVirtualMachineAndReturnsSuccessfullyCompletedReconfigureTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            var updatedCpuValue = virtualMachine.Cpu*2;
            var updatedRamValue = virtualMachine.Ram*2;
            var updatedVdrpEnabled = !virtualMachine.VdrpEnabled;
            virtualMachine.Cpu = updatedCpuValue;
            virtualMachine.Ram = updatedRamValue;
            virtualMachine.VdrpEnabled = updatedVdrpEnabled;

            // Act
            var updateTask = abiquoClient.UpdateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachine, true, true);

            var updatedVirtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(updateTask.TaskId));
            Assert.IsTrue(0 < updateTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, updateTask.State);
            Assert.AreEqual(TaskTypeEnum.RECONFIGURE, updateTask.Type);

            Assert.IsNotNull(updatedVirtualMachine);
            Assert.IsTrue(0 < updatedVirtualMachine.Id);
            Assert.AreEqual(updatedCpuValue, updatedVirtualMachine.Cpu);
            Assert.AreEqual(updatedRamValue, updatedVirtualMachine.Ram);
            Assert.AreEqual(updatedVdrpEnabled, updatedVirtualMachine.VdrpEnabled);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void UpdateVirtualMachineWithWaitForCompletionUpdatesAbiquoVirtualMachineAndReturnsSuccessfullyCompletedUpdateTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            var updatedCpuValue = virtualMachine.Cpu * 2;
            var updatedRamValue = virtualMachine.Ram * 2;
            var updatedVdrpEnabled = !virtualMachine.VdrpEnabled;
            virtualMachine.Cpu = updatedCpuValue;
            virtualMachine.Ram = updatedRamValue;
            virtualMachine.VdrpEnabled = updatedVdrpEnabled;

            // Act
            var updateTask = abiquoClient.UpdateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachine, false, true);

            var updatedVirtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(updateTask.TaskId));
            Assert.IsTrue(0 < updateTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, updateTask.State);
            Assert.AreEqual(TaskTypeEnum.RECONFIGURE, updateTask.Type);

            Assert.IsNotNull(updatedVirtualMachine);
            Assert.IsTrue(0 < updatedVirtualMachine.Id);
            Assert.AreEqual(updatedCpuValue, updatedVirtualMachine.Cpu);
            Assert.AreEqual(updatedRamValue, updatedVirtualMachine.Ram);
            Assert.AreEqual(updatedVdrpEnabled, updatedVirtualMachine.VdrpEnabled);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void UpdateVirtualMachineWithWaitForCompletionForDeployedVmUpdatesAbiquoVirtualMachineAndReturnsSuccessfullyCompletedUpdateTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            var deployTask = abiquoClient.DeployVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), false, true);

            virtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            var variableKey = "arbitraryKey";
            var variableValue = "ArbitraryValue";
            var updatedVariables = new Dictionary<string, string>()
            {
                {variableKey, variableValue}
            };
            var updatedDescription = "Arbitrary Description";
            virtualMachine.Variables = updatedVariables;
            virtualMachine.Description = updatedDescription;

            // Act
            var updateTask = abiquoClient.UpdateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachine, false, true);

            var updatedVirtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(deployTask.TaskId));
            Assert.IsTrue(0 < deployTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, deployTask.State);
            Assert.AreEqual(TaskTypeEnum.DEPLOY, deployTask.Type);

            Assert.IsFalse(string.IsNullOrWhiteSpace(updateTask.TaskId));
            Assert.IsTrue(0 < updateTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, updateTask.State);
            Assert.AreEqual(TaskTypeEnum.RECONFIGURE, updateTask.Type);

            Assert.IsNotNull(updatedVirtualMachine);
            Assert.IsTrue(0 < updatedVirtualMachine.Id);
            Assert.AreEqual(updatedDescription, updatedVirtualMachine.Description);
            Assert.IsTrue(updatedVirtualMachine.Variables.ContainsKey(variableKey));
            Assert.AreEqual(variableValue, updatedVirtualMachine.Variables[variableKey]);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void ChangeVirtualMachineStateWithWaitForCompletionChangesStateOfAbiquoVirtualMachineAndReturnsSuccessfullyCompletedTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            abiquoClient.DeployVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), false, true);

            // Act
            var changeStateTask = abiquoClient.ChangeStateOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachineOffState, true);

            var switchedOffVirtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id, virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(changeStateTask.TaskId));
            Assert.IsTrue(0 < changeStateTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, changeStateTask.State);
            Assert.AreEqual(TaskTypeEnum.POWER_OFF, changeStateTask.Type);

            Assert.AreEqual(VirtualMachineStateEnum.OFF, switchedOffVirtualMachine.State);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void ChangeVirtualMachineStateChangesStateOfAbiquoVirtualMachineAndReturnsSuccessfullyCompletedTask()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            abiquoClient.DeployVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), false, true);

            // Act
            var changeStateTask = abiquoClient.ChangeStateOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachineOffState);

            var completedTask = abiquoClient.WaitForTaskCompletion(changeStateTask,
                abiquoClient.TaskPollingWaitTimeMilliseconds, abiquoClient.TaskPollingTimeoutMilliseconds);

            var switchedOffVirtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id, virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(changeStateTask.TaskId));
            Assert.IsTrue(0 < changeStateTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, changeStateTask.State);
            Assert.AreEqual(TaskTypeEnum.POWER_OFF, changeStateTask.Type);

            Assert.IsFalse(string.IsNullOrWhiteSpace(completedTask.TaskId));
            Assert.AreEqual(changeStateTask.TaskId, completedTask.TaskId);
            Assert.IsTrue(0 < completedTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, completedTask.State);
            Assert.AreEqual(TaskTypeEnum.POWER_OFF, completedTask.Type);

            Assert.AreEqual(VirtualMachineStateEnum.OFF, switchedOffVirtualMachine.State);

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void DeleteVirtualMachineDeletesAbiquoVirtualMachine()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachineTemplate);

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);
            
            // Act
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);

            // Assert
            Assert.IsTrue(loginSucceeded);
            Assert.IsTrue(deletionResult);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetNetworkConfigurationsForVmReturnsAbiquoNetworkConfigurationsForVm()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var virtualMachines = abiquoClient.GetVirtualMachines(virtualDataCenter.Id, virtualAppliance.Id);
            var virtualMachine = virtualMachines.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachine);

            // Act
            var networkConfigurations = abiquoClient.GetNetworkConfigurationsForVm(virtualDataCenter.Id,
                virtualAppliance.Id, virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(networkConfigurations);
            Assert.IsNotNull(networkConfigurations.Collection);
            Assert.IsTrue(0 < networkConfigurations.Collection.Count);
            Assert.IsNotNull(networkConfigurations.Links);

            var networkConfiguration = networkConfigurations.Collection.First();
            Assert.IsTrue(0 < networkConfiguration.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetNetworkConfigurationForVmReturnsExpectedAbiquoNetworkConfiguration()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var virtualMachines = abiquoClient.GetVirtualMachines(virtualDataCenter.Id, virtualAppliance.Id);
            var virtualMachine = virtualMachines.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachine);

            var networkConfigurations = abiquoClient.GetNetworkConfigurationsForVm(virtualDataCenter.Id,
                virtualAppliance.Id, virtualMachine.Id.GetValueOrDefault());

            var expectedNetworkConfiguration = networkConfigurations.Collection.First();

            // Act
            var networkConfiguration = abiquoClient.GetNetworkConfigurationForVm(virtualDataCenter.Id,
                virtualAppliance.Id, virtualMachine.Id.GetValueOrDefault(),
                expectedNetworkConfiguration.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsTrue(0 < networkConfiguration.Id);
            Assert.AreEqual(expectedNetworkConfiguration.Id, networkConfiguration.Id);
            Assert.AreEqual(expectedNetworkConfiguration.Gateway, networkConfiguration.Gateway);
            Assert.AreEqual(expectedNetworkConfiguration.PrimaryDNS, networkConfiguration.PrimaryDNS);
            Assert.AreEqual(expectedNetworkConfiguration.SecondaryDNS, networkConfiguration.SecondaryDNS);
            Assert.AreEqual(expectedNetworkConfiguration.SuffixDNS, networkConfiguration.SuffixDNS);
            Assert.AreEqual(expectedNetworkConfiguration.Used, networkConfiguration.Used);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetNicsOfVirtualMachineReturnsAbiquoNicsOfSpecifiedVirtualMachine()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var virtualMachines = abiquoClient.GetVirtualMachines(virtualDataCenter.Id, virtualAppliance.Id);
            var virtualMachine = virtualMachines.Collection.LastOrDefault();
            Contract.Assert(null != virtualMachine);

            // Act
            var nics = abiquoClient.GetNicsOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(nics);
            Assert.IsNotNull(nics.Collection);
            Assert.IsTrue(0 < nics.Collection.Count);

            var nic = nics.Collection.First();
            Assert.IsNotNull(nic);
            Assert.IsTrue(0 < nic.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetAllTasksOfVirtualMachineReturnsAbiquoTasksOfVirtualMachine()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var virtualMachines = abiquoClient.GetVirtualMachines(virtualDataCenter.Id, virtualAppliance.Id);
            var expectedVirtualMachine = virtualMachines.Collection.LastOrDefault();
            Contract.Assert(null != expectedVirtualMachine);

            var virtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                expectedVirtualMachine.Id.GetValueOrDefault());
            
            // Act
            var tasksOfVirtualMachine = abiquoClient.GetAllTasksOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(tasksOfVirtualMachine);
            Assert.IsNotNull(tasksOfVirtualMachine.Collection);
            Assert.IsTrue(0 < tasksOfVirtualMachine.Collection.Count);
            Assert.IsNotNull(tasksOfVirtualMachine.Links);
            Assert.IsTrue(0 < tasksOfVirtualMachine.Links.Count);

            var taskOfVirtualMachine = tasksOfVirtualMachine.Collection.FirstOrDefault();
            Assert.IsNotNull(taskOfVirtualMachine);
            Assert.IsFalse(string.IsNullOrWhiteSpace(taskOfVirtualMachine.OwnerId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(taskOfVirtualMachine.TaskId));
            Assert.IsTrue(0 < taskOfVirtualMachine.Timestamp);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetTaskOfVirtualMachineReturnsAbiquoTasksOfVirtualMachine()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != virtualAppliance);

            var virtualMachines = abiquoClient.GetVirtualMachines(virtualDataCenter.Id, virtualAppliance.Id);
            var existingVirtualMachine = virtualMachines.Collection.LastOrDefault();
            Contract.Assert(null != existingVirtualMachine);

            var virtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                existingVirtualMachine.Id.GetValueOrDefault());

            var tasksOfVirtualMachine = abiquoClient.GetAllTasksOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            var expectedTaskOfVirtualMachine = tasksOfVirtualMachine.Collection.FirstOrDefault();
            Contract.Assert(null != expectedTaskOfVirtualMachine);
            
            // Act
            var taskOfVirtualMachine = abiquoClient.GetTaskOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), expectedTaskOfVirtualMachine.TaskId);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(taskOfVirtualMachine);
            Assert.AreEqual(expectedTaskOfVirtualMachine.OwnerId, taskOfVirtualMachine.OwnerId);
            Assert.AreEqual(expectedTaskOfVirtualMachine.State, taskOfVirtualMachine.State);
            Assert.AreEqual(expectedTaskOfVirtualMachine.TaskId, taskOfVirtualMachine.TaskId);
            Assert.AreEqual(expectedTaskOfVirtualMachine.Timestamp, taskOfVirtualMachine.Timestamp);
            Assert.AreEqual(expectedTaskOfVirtualMachine.Type, taskOfVirtualMachine.Type);
            Assert.AreEqual(expectedTaskOfVirtualMachine.Jobs.Collection.Count, taskOfVirtualMachine.Jobs.Collection.Count);
            Assert.AreEqual(expectedTaskOfVirtualMachine.UserId, taskOfVirtualMachine.UserId);
        }

        #endregion VirtualMachines


        #region VirtualMachineTemplates

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualMachineTemplatesReturnsAbiquoVirtualMachineTemplates()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            // Act
            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualMachineTemplates);
            Assert.IsNotNull(virtualMachineTemplates.Collection);
            Assert.IsTrue(0 < virtualMachineTemplates.TotalSize);
            Assert.IsTrue(0 < virtualMachineTemplates.Links.Count);

            var virtualMachineTemplate = virtualMachineTemplates.Collection.FirstOrDefault();
            Assert.IsNotNull(virtualMachineTemplate);
            Assert.IsTrue(0 < virtualMachineTemplate.Id);
            Assert.IsNotNull(virtualMachineTemplate.Name);
            Assert.IsTrue(0 < virtualMachineTemplate.CpuRequired);
            Assert.IsTrue(0 < virtualMachineTemplate.RamRequired);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualMachineTemplateReturnsExpectedAbiquoVirtualMachineTemplate()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != dataCenterRepository);

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var existingVirtualMachine = virtualMachineTemplates.Collection.FirstOrDefault();
            Contract.Assert(null != existingVirtualMachine);

            // Act
            var virtualMachineTemplate = abiquoClient.GetVirtualMachineTemplate(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId, existingVirtualMachine.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualMachineTemplate);
            Assert.IsTrue(0 < virtualMachineTemplate.Id);
            Assert.IsNotNull(virtualMachineTemplate.Name);
            Assert.IsTrue(0 < virtualMachineTemplate.CpuRequired);
            Assert.IsTrue(0 < virtualMachineTemplate.RamRequired);
        }

        #endregion VirtualMachineTemplates


        #region VirtualDataCenters

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualDataCentersReturnsAbiquoVirtualDataCenters()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualDataCenters);
            Assert.IsNotNull(virtualDataCenters.Collection);
            Assert.IsTrue(0 < virtualDataCenters.TotalSize);
            Assert.IsTrue(0 < virtualDataCenters.Links.Count);

            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Assert.IsNotNull(virtualDataCenter);
            Assert.IsTrue(0 < virtualDataCenter.Id);
            Assert.IsNotNull(virtualDataCenter.Name);
            Assert.AreEqual(0, virtualDataCenter.CpuCountHardLimit);
            Assert.AreEqual(0, virtualDataCenter.CpuCountSoftLimit);
            Assert.AreEqual(0, virtualDataCenter.PublicIpsSoft);
            Assert.AreEqual(0, virtualDataCenter.PublicIpsHard);
            Assert.AreEqual(0, virtualDataCenter.DiskSoftLimitInMb);
            Assert.AreEqual(0, virtualDataCenter.DiskHardLimitInMb);
            Assert.AreEqual(0, virtualDataCenter.RamHardLimitInMb);
            Assert.AreEqual(0, virtualDataCenter.RamSoftLimitInMb);
            Assert.IsNotNull(virtualDataCenter.Vlan);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualDataCenterReturnsExpectedAbiquoVirtualDataCenter()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var expectedVirtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != expectedVirtualDataCenter);

            // Act
            var virtualDataCenter = abiquoClient.GetVirtualDataCenter(expectedVirtualDataCenter.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualDataCenter);
            Assert.IsTrue(0 < virtualDataCenter.Id);
            Assert.IsNotNull(virtualDataCenter.Name);
            Assert.AreEqual(0, virtualDataCenter.CpuCountHardLimit);
            Assert.AreEqual(0, virtualDataCenter.CpuCountSoftLimit);
            Assert.AreEqual(0, virtualDataCenter.PublicIpsSoft);
            Assert.AreEqual(0, virtualDataCenter.PublicIpsHard);
            Assert.AreEqual(0, virtualDataCenter.DiskSoftLimitInMb);
            Assert.AreEqual(0, virtualDataCenter.DiskHardLimitInMb);
            Assert.AreEqual(0, virtualDataCenter.RamHardLimitInMb);
            Assert.AreEqual(0, virtualDataCenter.RamSoftLimitInMb);
            
            Assert.IsNotNull(virtualDataCenter.Vlan);
            Assert.IsTrue(0 < virtualDataCenter.Vlan.Id);
            Assert.IsNotNull(virtualDataCenter.Vlan.Name);
            Assert.IsNotNull(virtualDataCenter.Vlan.DhcpOptions);
            Assert.IsNotNull(virtualDataCenter.Vlan.DhcpOptions.Collection);
        }

        #endregion VirtualDataCenters


        #region VirtualAppliances

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualAppliancesReturnsAbiquoVirtualAppliances()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenterToBeLoaded = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenterToBeLoaded);
            var virtualDataCenter = abiquoClient.GetVirtualDataCenter(virtualDataCenterToBeLoaded.Id);

            // Act
            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualAppliances);
            Assert.IsNotNull(virtualAppliances.Collection);
            Assert.IsTrue(0 < virtualAppliances.TotalSize);
            Assert.IsTrue(0 < virtualAppliances.Links.Count);

            var virtualAppliance = virtualAppliances.Collection.FirstOrDefault();
            Assert.IsNotNull(virtualAppliance);
            Assert.IsTrue(0 < virtualAppliance.Id);
            Assert.IsNotNull(virtualAppliance.Name);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetVirtualApplianceReturnsExpectedAbiquoVirtualAppliance()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenterToBeLoaded = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenterToBeLoaded);

            var virtualDataCenter = abiquoClient.GetVirtualDataCenter(virtualDataCenterToBeLoaded.Id);
            
            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var expectedVirtualApplicance = virtualAppliances.Collection.FirstOrDefault();
            Contract.Assert(null != expectedVirtualApplicance);

            // Act
            var virtualAppliance = abiquoClient.GetVirtualAppliance(virtualDataCenter.Id, expectedVirtualApplicance.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(virtualAppliance);
            Assert.AreEqual(expectedVirtualApplicance.Id, virtualAppliance.Id);
            Assert.AreEqual(expectedVirtualApplicance.Error, virtualAppliance.Error);
            Assert.AreEqual(expectedVirtualApplicance.HighDisponibility, virtualAppliance.HighDisponibility);
            Assert.AreEqual(expectedVirtualApplicance.NodeConnections, virtualAppliance.NodeConnections);
            Assert.AreEqual(expectedVirtualApplicance.State, virtualAppliance.State);
            Assert.AreEqual(expectedVirtualApplicance.SubState, virtualAppliance.SubState);
            Assert.AreEqual(expectedVirtualApplicance.PublicApp, virtualAppliance.PublicApp);
        }

        #endregion VirtualAppliances


        #region DataCenterRepositories

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetDataCenterRepositoriesReturnsAbiquoDataCenterRepositories()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var dataCenterRepositories = abiquoClient.GetDataCenterRepositories(IntegrationTestEnvironment.TenantId);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(dataCenterRepositories);
            Assert.IsNotNull(dataCenterRepositories.Collection);
            Assert.IsNotNull(dataCenterRepositories.Links);
            //Assert.IsTrue(0 < dataCenterRepositories.TotalSize);
            //Assert.IsTrue(0 < dataCenterRepositories.Links.Count);

            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Assert.IsNotNull(dataCenterRepository);
            Assert.IsNotNull(dataCenterRepository.Links);
            Assert.IsTrue(0 < dataCenterRepository.Links.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.RepositoryLocation));
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetDataCenterRepositoriesOfCurrentEnterpriseReturnsAbiquoDataCenterRepositoriesOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            // Act
            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(dataCenterRepositories);
            Assert.IsNotNull(dataCenterRepositories.Collection);
            Assert.IsNotNull(dataCenterRepositories.Links);
            //Assert.IsTrue(0 < dataCenterRepositories.TotalSize);
            //Assert.IsTrue(0 < dataCenterRepositories.Links.Count);

            var dataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Assert.IsNotNull(dataCenterRepository);
            Assert.IsNotNull(dataCenterRepository.Links);
            Assert.IsTrue(0 < dataCenterRepository.Links.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.RepositoryLocation));
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetDataCenterRepositoryOfCurrentEnterpriseReturnsExpectedAbiquoDataCenterRepositoryOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var expectedDataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != expectedDataCenterRepository);

            var editLink = expectedDataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            // Act
            var dataCenterRepository = abiquoClient.GetDataCenterRepositoryOfCurrentEnterprise(dataCenterRepositoryId);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(dataCenterRepository);
            Assert.IsNotNull(dataCenterRepository.Links);
            Assert.IsTrue(0 < dataCenterRepository.Links.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.RepositoryLocation));

            Assert.AreEqual(expectedDataCenterRepository.Error, dataCenterRepository.Error);
            Assert.AreEqual(expectedDataCenterRepository.Name, dataCenterRepository.Name);
            Assert.AreEqual(expectedDataCenterRepository.RepositoryCapacityMb, dataCenterRepository.RepositoryCapacityMb);
            Assert.AreEqual(expectedDataCenterRepository.RepositoryLocation, dataCenterRepository.RepositoryLocation);
            Assert.AreEqual(expectedDataCenterRepository.RepositoryRemainingMb, dataCenterRepository.RepositoryRemainingMb);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetDataCenterRepositoryReturnsExpectedAbiquoDataCenterRepository()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var expectedDataCenterRepository = dataCenterRepositories.Collection.FirstOrDefault();
            Contract.Assert(null != expectedDataCenterRepository);

            var editLink = expectedDataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            // Act
            var dataCenterRepository = abiquoClient.GetDataCenterRepository(IntegrationTestEnvironment.TenantId, dataCenterRepositoryId);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(dataCenterRepository);
            Assert.IsNotNull(dataCenterRepository.Links);
            Assert.IsTrue(0 < dataCenterRepository.Links.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(dataCenterRepository.RepositoryLocation));

            Assert.AreEqual(expectedDataCenterRepository.Error, dataCenterRepository.Error);
            Assert.AreEqual(expectedDataCenterRepository.Name, dataCenterRepository.Name);
            Assert.AreEqual(expectedDataCenterRepository.RepositoryCapacityMb, dataCenterRepository.RepositoryCapacityMb);
            Assert.AreEqual(expectedDataCenterRepository.RepositoryLocation, dataCenterRepository.RepositoryLocation);
            Assert.AreEqual(expectedDataCenterRepository.RepositoryRemainingMb, dataCenterRepository.RepositoryRemainingMb);
        }

        #endregion DataCenterRepositories


        #region Tasks

        // WaitForTaskCompletion gets implicitly tested by some of the other integration tests

        #endregion Tasks


        #region Networks

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetPrivateNetworksReturnsAbiquoPrivateNetworks()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            // Act
            var privateNetworks = abiquoClient.GetPrivateNetworks(virtualDataCenter.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(privateNetworks);
            Assert.IsNotNull(privateNetworks.Collection);
            Assert.IsTrue(0 < privateNetworks.Collection.Count);
            Assert.IsNotNull(privateNetworks.Links);

            var privateNetwork = privateNetworks.Collection.First();
            Assert.IsTrue(privateNetwork.IsValid());
            Assert.IsTrue(0 < privateNetwork.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetPrivateNetworkReturnsExpectedAbiquoPrivateNetwork()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var privateNetworks = abiquoClient.GetPrivateNetworks(virtualDataCenter.Id);
            var expectedPrivateNetwork = privateNetworks.Collection.First();
            
            // Act
            var privateNetwork = abiquoClient.GetPrivateNetwork(virtualDataCenter.Id, expectedPrivateNetwork.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsTrue(privateNetwork.IsValid());
            Assert.IsTrue(0 < privateNetwork.Id);
            Assert.AreEqual(expectedPrivateNetwork.Id, privateNetwork.Id);
            Assert.AreEqual(expectedPrivateNetwork.Address, privateNetwork.Address);
            Assert.AreEqual(expectedPrivateNetwork.DefaultNetwork, privateNetwork.DefaultNetwork);
            Assert.AreEqual(expectedPrivateNetwork.Gateway, privateNetwork.Gateway);
            Assert.AreEqual(expectedPrivateNetwork.Ipv6, privateNetwork.Ipv6);
            Assert.AreEqual(expectedPrivateNetwork.Mask, privateNetwork.Mask);
            Assert.AreEqual(expectedPrivateNetwork.Name, privateNetwork.Name);
            Assert.AreEqual(expectedPrivateNetwork.PrimaryDns, privateNetwork.PrimaryDns);
            Assert.AreEqual(expectedPrivateNetwork.SecondaryDns, privateNetwork.SecondaryDns);
            Assert.AreEqual(expectedPrivateNetwork.Strict, privateNetwork.Strict);
            Assert.AreEqual(expectedPrivateNetwork.Tag, privateNetwork.Tag);
            Assert.AreEqual(expectedPrivateNetwork.Type, privateNetwork.Type);
            Assert.AreEqual(expectedPrivateNetwork.Unmanaged, privateNetwork.Unmanaged);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetIpsOfPrivateNetworkWihtFreeReturnsFreeIPsOfPrivateNetwork()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var privateNetworks = abiquoClient.GetPrivateNetworks(virtualDataCenter.Id);
            var privateNetwork = privateNetworks.Collection.First();

            // Act
            var ips = abiquoClient.GetIpsOfPrivateNetwork(virtualDataCenter.Id, privateNetwork.Id, true);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(ips);
            Assert.IsNotNull(ips.Collection);
            Assert.IsTrue(0 < ips.Collection.Count);
            Assert.IsNotNull(ips.Links);
            Assert.IsTrue(0 < ips.Links.Count);

            var ip = ips.Collection.First();
            Assert.IsNotNull(ip);
            Assert.IsTrue(0 < ip.Id);
            Assert.IsFalse(string.IsNullOrWhiteSpace(ip.Ip));
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetExternalNetworksOfCurrentEnterpriseReturnsAbiquoExternalNetworksOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCentersLimits = abiquoClient.GetDataCentersLimitsOfCurrentEnterprise();
            var dataCenterLimits = dataCentersLimits.Collection.First();

            // Act
            var externalNetworks = abiquoClient.GetExternalNetworksOfCurrentEnterprise(dataCenterLimits.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(externalNetworks);
            Assert.IsNotNull(externalNetworks.Links);
            Assert.IsTrue(0 < externalNetworks.Links.Count);
            Assert.IsNotNull(externalNetworks.Collection);
            Assert.IsTrue(0 < externalNetworks.Collection.Count);

            var externalNetwork = externalNetworks.Collection.First();
            Assert.IsTrue(externalNetwork.IsValid());
            Assert.IsTrue(0 < externalNetwork.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetExternalNetworkOfCurrentEnterpriseReturnsExpectedAbiquoExternalNetworkOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCentersLimits = abiquoClient.GetDataCentersLimitsOfCurrentEnterprise();
            var dataCenterLimits = dataCentersLimits.Collection.First();

            var externalNetworks = abiquoClient.GetExternalNetworksOfCurrentEnterprise(dataCenterLimits.Id);
            var expectedExternalNetwork = externalNetworks.Collection.First();
            
            // Act
            var externalNetwork = abiquoClient.GetExternalNetworkOfCurrentEnterprise(dataCenterLimits.Id,
                expectedExternalNetwork.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsTrue(externalNetwork.IsValid());
            Assert.IsTrue(0 < externalNetwork.Id);
            Assert.AreEqual(expectedExternalNetwork.Id, externalNetwork.Id);
            Assert.AreEqual(expectedExternalNetwork.Address, externalNetwork.Address);
            Assert.AreEqual(expectedExternalNetwork.DefaultNetwork, externalNetwork.DefaultNetwork);
            Assert.AreEqual(expectedExternalNetwork.Gateway, externalNetwork.Gateway);
            Assert.AreEqual(expectedExternalNetwork.Ipv6, externalNetwork.Ipv6);
            Assert.AreEqual(expectedExternalNetwork.Mask, externalNetwork.Mask);
            Assert.AreEqual(expectedExternalNetwork.Name, externalNetwork.Name);
            Assert.AreEqual(expectedExternalNetwork.PrimaryDns, externalNetwork.PrimaryDns);
            Assert.AreEqual(expectedExternalNetwork.SecondaryDns, externalNetwork.SecondaryDns);
            Assert.AreEqual(expectedExternalNetwork.Strict, externalNetwork.Strict);
            Assert.AreEqual(expectedExternalNetwork.Tag, externalNetwork.Tag);
            Assert.AreEqual(expectedExternalNetwork.Type, externalNetwork.Type);
            Assert.AreEqual(expectedExternalNetwork.Unmanaged, externalNetwork.Unmanaged);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetIpsOfExternalNetworkOfCurrentEnterpriseWithFreeReturnsFreeIPsOfExternalNetworkOfCurrentEnterprise()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var dataCentersLimits = abiquoClient.GetDataCentersLimitsOfCurrentEnterprise();
            var dataCenterLimits = dataCentersLimits.Collection.First();

            var externalNetworks = abiquoClient.GetExternalNetworksOfCurrentEnterprise(dataCenterLimits.Id);
            var externalNetwork = externalNetworks.Collection.First();

            // Act
            var ips = abiquoClient.GetIpsOfExternalNetworkOfCurrentEnterprise(dataCenterLimits.Id, externalNetwork.Id, true);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(ips);
            Assert.IsNotNull(ips.Collection);
            Assert.IsTrue(0 < ips.Collection.Count);
            Assert.IsNotNull(ips.Links);
            Assert.IsTrue(0 < ips.Links.Count);

            var ip = ips.Collection.First();
            Assert.IsNotNull(ip);
            Assert.IsTrue(0 < ip.Id);
            Assert.IsFalse(string.IsNullOrWhiteSpace(ip.Ip));
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetPublicNetworksReturnsAbiquoPublicNetworks()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            // Act
            var publicNetworks = abiquoClient.GetPublicNetworks(virtualDataCenter.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(publicNetworks);
            Assert.IsNotNull(publicNetworks.Collection);
            Assert.IsTrue(0 < publicNetworks.Collection.Count);
            Assert.IsNotNull(publicNetworks.Links);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetPublicNetworkReturnsExpectedAbiquoPublicNetwork()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var publicNetworks = abiquoClient.GetPublicNetworks(virtualDataCenter.Id);
            var expectedPublicNetwork = publicNetworks.Collection.First();

            // Act
            var publicNetwork = abiquoClient.GetPublicNetwork(virtualDataCenter.Id, expectedPublicNetwork.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(publicNetwork);
            Assert.IsTrue(publicNetwork.IsValid());
            Assert.IsTrue(0 < publicNetwork.Id);
            Assert.AreEqual(expectedPublicNetwork.Id, publicNetwork.Id);
            Assert.AreEqual(expectedPublicNetwork.Address, publicNetwork.Address);
            Assert.AreEqual(expectedPublicNetwork.DefaultNetwork, publicNetwork.DefaultNetwork);
            Assert.AreEqual(expectedPublicNetwork.Gateway, publicNetwork.Gateway);
            Assert.AreEqual(expectedPublicNetwork.Ipv6, publicNetwork.Ipv6);
            Assert.AreEqual(expectedPublicNetwork.Mask, publicNetwork.Mask);
            Assert.AreEqual(expectedPublicNetwork.Name, publicNetwork.Name);
            Assert.AreEqual(expectedPublicNetwork.PrimaryDns, publicNetwork.PrimaryDns);
            Assert.AreEqual(expectedPublicNetwork.SecondaryDns, publicNetwork.SecondaryDns);
            Assert.AreEqual(expectedPublicNetwork.Strict, publicNetwork.Strict);
            Assert.AreEqual(expectedPublicNetwork.Tag, publicNetwork.Tag);
            Assert.AreEqual(expectedPublicNetwork.Type, publicNetwork.Type);
            Assert.AreEqual(expectedPublicNetwork.Unmanaged, publicNetwork.Unmanaged);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void GetPublicIpsToPurchaseOfPublicNetworkReturnsAbiquoPublicIpsToPurchase()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var publicNetworks = abiquoClient.GetPublicNetworks(virtualDataCenter.Id);
            var publicNetwork = publicNetworks.Collection.First();

            // Act
            var publicIpsToPurchase = abiquoClient.GetPublicIpsToPurchaseOfPublicNetwork(virtualDataCenter.Id,
                publicNetwork.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(publicIpsToPurchase);
            Assert.IsTrue(publicIpsToPurchase.IsValid());
            Assert.IsNotNull(publicIpsToPurchase.Collection);
            Assert.IsTrue(0 < publicIpsToPurchase.Collection.Count);

            var publicIp = publicIpsToPurchase.Collection.First();
            Assert.IsNotNull(publicIp);
            Assert.IsTrue(publicIp.IsValid());
            Assert.IsTrue(0 < publicIp.Id);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void PurchaseAndReleasePublicIpSucceeds()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.FirstOrDefault();
            Contract.Assert(null != virtualDataCenter);

            var publicNetworks = abiquoClient.GetPublicNetworks(virtualDataCenter.Id);
            var publicNetwork = publicNetworks.Collection.First();

            var publicIpsToPurchase = abiquoClient.GetPublicIpsToPurchaseOfPublicNetwork(virtualDataCenter.Id,
                publicNetwork.Id);
            var publicIpToBePurchased = publicIpsToPurchase.Collection.First();

            // Act
            var purchasedPublicIp = abiquoClient.PurchasePublicIp(virtualDataCenter.Id, publicIpToBePurchased.Id);

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsNotNull(purchasedPublicIp);
            Assert.IsTrue(purchasedPublicIp.IsValid());
            Assert.IsTrue(0 < purchasedPublicIp.Id);

            Assert.AreEqual(publicIpToBePurchased.Available, purchasedPublicIp.Available);
            Assert.AreEqual(publicIpToBePurchased.Id, purchasedPublicIp.Id);
            Assert.AreEqual(publicIpToBePurchased.Ip, purchasedPublicIp.Ip);
            Assert.AreNotEqual(publicIpToBePurchased.Mac, purchasedPublicIp.Mac);
            Assert.AreNotEqual(publicIpToBePurchased.Name, purchasedPublicIp.Name);
            Assert.AreEqual(publicIpToBePurchased.NetworkName, purchasedPublicIp.NetworkName);
            Assert.AreEqual(publicIpToBePurchased.Quarantine, purchasedPublicIp.Quarantine);

            // Cleanup
            var releasedPublicIp = abiquoClient.ReleasePublicIp(virtualDataCenter.Id, publicIpToBePurchased.Id);

            Assert.AreEqual(publicIpToBePurchased.Available, releasedPublicIp.Available);
            Assert.AreEqual(publicIpToBePurchased.Id, releasedPublicIp.Id);
            Assert.AreEqual(publicIpToBePurchased.Ip, releasedPublicIp.Ip);
            Assert.AreEqual(publicIpToBePurchased.Mac, releasedPublicIp.Mac);
            Assert.AreEqual(publicIpToBePurchased.Name, releasedPublicIp.Name);
            Assert.AreEqual(publicIpToBePurchased.NetworkName, releasedPublicIp.NetworkName);
            Assert.AreEqual(publicIpToBePurchased.Quarantine, releasedPublicIp.Quarantine);
        }

        #endregion Networks

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void AttachNetworkToNotDeployedVirtualMachineSucceeds()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.First();

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.First();

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.First();

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.Last();

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            // Get available public networks
            var publicNetworks = abiquoClient.GetPublicNetworks(virtualDataCenter.Id);
            var publicNetwork = publicNetworks.Collection.First();

            // Get IP of public network
            var publicIpsToPurchase = abiquoClient.GetPublicIpsToPurchaseOfPublicNetwork(virtualDataCenter.Id,
                publicNetwork.Id);
            var publicIpToBePurchased = publicIpsToPurchase.Collection.First();

            // Purchase IP
            var purchasedPublicIp = abiquoClient.PurchasePublicIp(virtualDataCenter.Id, publicIpToBePurchased.Id);
            Contract.Assert(null != purchasedPublicIp);

            var nics = abiquoClient.GetNicsOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            var ipLinkRel = string.Format("nic{0}", nics.Collection.Count);
            var ipLinkHref = purchasedPublicIp.GetLinkByRel(AbiquoRelations.SELF).Href;
            var ipLink = new LinkBuilder()
                .BuildHref(ipLinkHref)
                .BuildRel(ipLinkRel)
                .BuildTitle("BluenetIp")
                .BuildType(AbiquoMediaDataTypes.VND_ABIQUO_PUBLICIP)
                .GetLink();

            // Act
            virtualMachine.Links.Add(ipLink);

            var updateTask = abiquoClient.UpdateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachine, false, true);

            var vmWithAttachedNetwork = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(updateTask.TaskId));
            Assert.IsTrue(0 < updateTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, updateTask.State);
            Assert.AreEqual(TaskTypeEnum.RECONFIGURE, updateTask.Type);

            Assert.IsNotNull(vmWithAttachedNetwork);
            Assert.IsTrue(0 < vmWithAttachedNetwork.Id);
            Assert.IsNotNull(vmWithAttachedNetwork.Links.FirstOrDefault(l => l.Href == ipLinkHref));

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);

            var releasedPublicIp = abiquoClient.ReleasePublicIp(virtualDataCenter.Id, publicIpToBePurchased.Id);

            Assert.AreEqual(publicIpToBePurchased.Available, releasedPublicIp.Available);
            Assert.AreEqual(publicIpToBePurchased.Id, releasedPublicIp.Id);
            Assert.AreEqual(publicIpToBePurchased.Ip, releasedPublicIp.Ip);
            Assert.AreEqual(publicIpToBePurchased.Mac, releasedPublicIp.Mac);
            Assert.AreEqual(publicIpToBePurchased.Name, releasedPublicIp.Name);
            Assert.AreEqual(publicIpToBePurchased.NetworkName, releasedPublicIp.NetworkName);
            Assert.AreEqual(publicIpToBePurchased.Quarantine, releasedPublicIp.Quarantine);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void AttachNetworkToDeployedVirtualMachineSucceeds()
        {
            // Arrange
            var abiquoClient = AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);
            var loginSucceeded = abiquoClient.Login(IntegrationTestEnvironment.AbiquoApiBaseUri, BasicAuthenticationInformation);

            var virtualDataCenters = abiquoClient.GetVirtualDataCenters();
            var virtualDataCenter = virtualDataCenters.Collection.First();

            var virtualAppliances = abiquoClient.GetVirtualAppliances(virtualDataCenter.Id);
            var virtualAppliance = virtualAppliances.Collection.First();

            var dataCenterRepositories = abiquoClient.GetDataCenterRepositoriesOfCurrentEnterprise();
            var dataCenterRepository = dataCenterRepositories.Collection.First();

            var editLink = dataCenterRepository.GetLinkByRel("edit");
            var dataCenterRepositoryId = UriHelper.ExtractIdAsInt(editLink.Href);

            var virtualMachineTemplates = abiquoClient.GetVirtualMachineTemplates(IntegrationTestEnvironment.TenantId,
                dataCenterRepositoryId);
            var virtualMachineTemplate = virtualMachineTemplates.Collection.Last();

            var virtualMachine = abiquoClient.CreateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                IntegrationTestEnvironment.TenantId, dataCenterRepositoryId, virtualMachineTemplate.Id);

            var deployTask = abiquoClient.DeployVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), false, true);

            virtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Get available public networks
            var publicNetworks = abiquoClient.GetPublicNetworks(virtualDataCenter.Id);
            var publicNetwork = publicNetworks.Collection.First();

            // Get IP of public network
            var publicIpsToPurchase = abiquoClient.GetPublicIpsToPurchaseOfPublicNetwork(virtualDataCenter.Id,
                publicNetwork.Id);
            var publicIpToBePurchased = publicIpsToPurchase.Collection.First();

            // Purchase IP
            var purchasedPublicIp = abiquoClient.PurchasePublicIp(virtualDataCenter.Id, publicIpToBePurchased.Id);
            Contract.Assert(null != purchasedPublicIp);

            var nics = abiquoClient.GetNicsOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            var ipLinkRel = string.Format("nic{0}", nics.Collection.Count);
            var ipLinkHref = purchasedPublicIp.GetLinkByRel(AbiquoRelations.SELF).Href;
            var ipLink = new LinkBuilder()
                .BuildHref(ipLinkHref)
                .BuildRel(ipLinkRel)
                .BuildTitle("BluenetIp")
                .BuildType(AbiquoMediaDataTypes.VND_ABIQUO_PUBLICIP)
                .GetLink();

            // Act
            // Power off VM
            var changeStateTask = abiquoClient.ChangeStateOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachineOffState, true);
            Contract.Assert(TaskStateEnum.FINISHED_SUCCESSFULLY == changeStateTask.State);
            virtualMachine = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id, virtualMachine.Id.GetValueOrDefault());

            virtualMachine.Links.Add(ipLink);

            var updateTask = abiquoClient.UpdateVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachine, false, true);

            // Power on VM
            changeStateTask = abiquoClient.ChangeStateOfVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), virtualMachineOnState, true);
            Contract.Assert(TaskStateEnum.FINISHED_SUCCESSFULLY == changeStateTask.State);

            var vmWithAttachedNetwork = abiquoClient.GetVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault());

            // Assert
            Assert.IsTrue(loginSucceeded);

            Assert.IsFalse(string.IsNullOrWhiteSpace(deployTask.TaskId));
            Assert.IsTrue(0 < deployTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, deployTask.State);
            Assert.AreEqual(TaskTypeEnum.DEPLOY, deployTask.Type);

            Assert.IsFalse(string.IsNullOrWhiteSpace(updateTask.TaskId));
            Assert.IsTrue(0 < updateTask.Timestamp);
            Assert.AreEqual(TaskStateEnum.FINISHED_SUCCESSFULLY, updateTask.State);
            Assert.AreEqual(TaskTypeEnum.RECONFIGURE, updateTask.Type);

            Assert.IsNotNull(vmWithAttachedNetwork);
            Assert.IsTrue(0 < vmWithAttachedNetwork.Id);
            Assert.IsNotNull(vmWithAttachedNetwork.Links.FirstOrDefault(l => l.Href == ipLinkHref));

            // Cleanup
            var deletionResult = abiquoClient.DeleteVirtualMachine(virtualDataCenter.Id, virtualAppliance.Id,
                virtualMachine.Id.GetValueOrDefault(), true);
            Assert.IsTrue(deletionResult);

            var releasedPublicIp = abiquoClient.ReleasePublicIp(virtualDataCenter.Id, publicIpToBePurchased.Id);

            Assert.AreEqual(publicIpToBePurchased.Available, releasedPublicIp.Available);
            Assert.AreEqual(publicIpToBePurchased.Id, releasedPublicIp.Id);
            Assert.AreEqual(publicIpToBePurchased.Ip, releasedPublicIp.Ip);
            Assert.AreEqual(publicIpToBePurchased.Mac, releasedPublicIp.Mac);
            Assert.AreEqual(publicIpToBePurchased.Name, releasedPublicIp.Name);
            Assert.AreEqual(publicIpToBePurchased.NetworkName, releasedPublicIp.NetworkName);
            Assert.AreEqual(publicIpToBePurchased.Quarantine, releasedPublicIp.Quarantine);
        }
    }
}
