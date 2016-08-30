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
using biz.dfch.CS.Abiquo.Client.General;
﻿using biz.dfch.CS.Abiquo.Client.v1.Model;

namespace biz.dfch.CS.Abiquo.Client.Tests
{
    [TestClass]
    public class BaseAbiquoClientTest
    {
        private const string ABIQUO_API_BASE_URI = "https://abiquo/api/";
        private const string VIRTUALMACHINETEMPLATE_HREF = "http://abiquo/api/admin/enterprises/42/datacenterrepositories/42/virtualmachinetemplates/42";
        private const string USERNAME = "ArbitraryUsername";
        private const string PASSWORD = "ArbitraryPassword";
        private const int TENANT_ID = 1;
        private const int INVALID_ID = 0;

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

        #region ExecuteRequest

        [TestMethod]
        public void ExecuteRequestWithoutAdditionalHeadersAndBodyCallsRestCallExecutor()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var expectedRequestUri = UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES);
            
            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, authenticationInformation.GetAuthorizationHeaders(), null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.ExecuteRequest(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        public void ExecuteRequestWithAdditionalHeadersMergesHeadersAndCallsRestCallExecutorWithMergedHeaders()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var expectedRequestUri = UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES);

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.ExecuteRequest(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, headers, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        #endregion ExecuteRequest


        #region Generic Invoke

        [TestMethod]
        [ExpectContractFailure]
        public void GenericInvokeWithEmptyUriSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            abiquoClient.Invoke<Enterprise>(null, new Dictionary<string, string>());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GenericInvokeIfNotLoggedInThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.Invoke<Enterprises>(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null, null);

            // Assert
        }

        #endregion Generic Invoke


        #region Invoke

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithEmptyUriSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            abiquoClient.Invoke(HttpMethod.Get, " ", null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidUriSuffixThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            abiquoClient.Invoke(HttpMethod.Get, "http://example.com", null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeIfNotLoggedInThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null, default(BaseDto));

            // Assert
        }

        [TestMethod]
        public void InvokeWithFilterCallsRestCallExecutorWithRequestUriContainingFilterExpression()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();
            abiquoClient.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var filter = new Dictionary<string, object>()
            {
                {"currentPage", 1},
                {"limit", "25"}
            };

            var expectedRequestUri = string.Format("{0}?{1}", UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES), "currentPage=1&limit=25");

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { Constants.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = abiquoClient.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, filter, headers);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        #endregion Invoke


        #region Enterprises

        [TestMethod]
        [ExpectContractFailure]
        public void GetEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetEnterprise(INVALID_ID);

            // Assert
        }

        #endregion Enterprises


        #region Users

        [TestMethod]
        [ExpectContractFailure]
        public void GetUsersWithRolesWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetUsersWithRoles(INVALID_ID);

            // Assert
        }        
        
        [TestMethod]
        [ExpectContractFailure]
        public void GetUserOfCurrentEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetUserOfCurrentEnterprise(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetUserWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetUser(INVALID_ID, 15);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetUserWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetUser(42, INVALID_ID);

            // Assert
        }

        #endregion Users


        #region Roles

        [TestMethod]
        [ExpectContractFailure]
        public void GetRoleWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetRole(INVALID_ID);

            // Assert
        }

        #endregion Roles


        #region VirtualMachines

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachinesWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachines(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachinesWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachines(42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachine(INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachine(42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachine(42, 42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(0, 42, VIRTUALMACHINETEMPLATE_HREF);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 0, VIRTUALMACHINETEMPLATE_HREF);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithNullVirtualMachineTemplateHrefThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithEmptyVirtualMachineTemplateHrefThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, " ");

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineTemplateHrefThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, "Arbitrary");

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualDataCenterId2ThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(INVALID_ID, 42, 42, 42, 42, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualApplianceId2ThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, INVALID_ID, 42, 42, 42, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, INVALID_ID, 42, 42, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidDataCenterRepositoryIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, 42, INVALID_ID, 42, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineTemplateIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, 42, 42, INVALID_ID, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithNullVirtualMachineThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, 42, 42, 42, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualDataCenterId3ThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(INVALID_ID, 42, VIRTUALMACHINETEMPLATE_HREF, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualApplianceId3ThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, INVALID_ID, VIRTUALMACHINETEMPLATE_HREF, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithNullVirtualMachineTemplateHref2ThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, null, new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithEmptyVirtualMachineTemplateHref2ThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, " ", new VirtualMachine());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, VIRTUALMACHINETEMPLATE_HREF, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineTemplateHref2ThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.CreateVirtualMachine(42, 42, "Arbitrary", null);

            // Assert
        }

        #endregion VirtualMachines


        #region VirtualMachineTemplates

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplatesWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachineTemplates(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplatesWithInvalidDataCenterRepositoryIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachineTemplates(42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplateWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachineTemplate(INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplateWithInvalidDataCenterRepositoryIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachineTemplate(42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplateWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualMachineTemplate(42, 42, INVALID_ID);

            // Assert
        }

        #endregion VirtualMachineTemplates


        #region VirtualDataCenters

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualDataCenterWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualDataCenter(INVALID_ID);

            // Assert
        }

        #endregion VirtualDataCenters


        #region VirtualAppliances

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualAppliancesWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualAppliances(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualApplianceWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualAppliance(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualApplianceWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetVirtualAppliance(42, INVALID_ID);

            // Assert
        }

        #endregion VirtualAppliances


        #region DataCenterRepositories

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoriesWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetDataCenterRepositories(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoryOfCurrentEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetDataCenterRepositoryOfCurrentEnterprise(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoryWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetDataCenterRepository(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoryWithInvalidIdThrowsContractException()
        {
            // Arrange
            var abiquoClient = new DummyAbiquoClient();

            // Act
            abiquoClient.GetDataCenterRepository(42, INVALID_ID);

            // Assert
        }

        #endregion DataCenterRepositories


        private class DummyAbiquoClient : BaseAbiquoClient
        {
            public DummyAbiquoClient()
            {
                AbiquoApiVersion = "Arbitrary-Version";
            }

            public override bool Login(string abiquoApiBaseUri, IAuthenticationInformation authenticationInformation)
            {
                AbiquoApiBaseUri = abiquoApiBaseUri;
                AuthenticationInformation = authenticationInformation;

                IsLoggedIn = true;

                return true;
            }

            public override Enterprises GetEnterprises()
            {
                return new Enterprises();
            }

            public override Enterprise GetCurrentEnterprise()
            {
                return new Enterprise();
            }

            public override Enterprise GetEnterprise(int id)
            {
                return new Enterprise();
            }

            public override UsersWithRoles GetUsersWithRolesOfCurrentEnterprise()
            {
                return new UsersWithRoles();
            }

            public override UsersWithRoles GetUsersWithRoles(int enterpriseId)
            {
                return new UsersWithRoles();
            }

            public override User GetUserOfCurrentEnterprise(int id)
            {
                return new User();
            }

            public override User GetUser(int enterpriseId, int id)
            {
                return new User();
            }

            public override Roles GetRoles()
            {
                return new Roles();
            }

            public override Role GetRole(int id)
            {
                return new Role();
            }

            public override VirtualMachines GetAllVirtualMachines()
            {
                return new VirtualMachines();
            }

            public override VirtualMachines GetVirtualMachines(int virtualDataCenterId, int virtualApplianceId)
            {
                return new VirtualMachines();
            }

            public override VirtualMachine GetVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int id)
            {
                return new VirtualMachine();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId,
                int virtualMachineTemplateId)
            {
                return new VirtualMachine();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref)
            {
                return new VirtualMachine();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId,
                int virtualMachineTemplateId, VirtualMachine virtualMachine)
            {
                return new VirtualMachine();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref,
                VirtualMachine virtualMachine)
            {
                return new VirtualMachine();
            }

            public override VirtualMachineTemplates GetVirtualMachineTemplates(int enterpriseId, int dataCenterRepositoryId)
            {
                return new VirtualMachineTemplates();
            }

            public override VirtualMachineTemplate GetVirtualMachineTemplate(int enterpriseId, int dataCenterRepositoryId, int id)
            {
                return new VirtualMachineTemplate();
            }

            public override VirtualDataCenters GetVirtualDataCenters()
            {
                return new VirtualDataCenters();
            }

            public override VirtualDataCenter GetVirtualDataCenter(int id)
            {
                return new VirtualDataCenter();
            }

            public override VirtualAppliances GetVirtualAppliances(int virtualDataCenterId)
            {
                return new VirtualAppliances();
            }

            public override VirtualAppliance GetVirtualAppliance(int virtualDataCenterId, int id)
            {
                return new VirtualAppliance();
            }

            public override DataCenterRepositories GetDataCenterRepositoriesOfCurrentEnterprise()
            {
                return new DataCenterRepositories();
            }

            public override DataCenterRepositories GetDataCenterRepositories(int enterpriseId)
            {
                return new DataCenterRepositories();
            }

            public override DataCenterRepository GetDataCenterRepositoryOfCurrentEnterprise(int id)
            {
                return new DataCenterRepository();
            }

            public override DataCenterRepository GetDataCenterRepository(int enterpriseId, int id)
            {
                return new DataCenterRepository();
            }
        }

        private class InvalidAbiquoClient : BaseAbiquoClient
        {
            // AbiquoApiVersion intentionally not set in constructor for testing purposes

            public override bool Login(string abiquoApiBaseUri, IAuthenticationInformation authenticationInformation)
            {
                return true;
            }

            public override Enterprises GetEnterprises()
            {
                throw new NotImplementedException();
            }

            public override Enterprise GetCurrentEnterprise()
            {
                throw new NotImplementedException();
            }

            public override Enterprise GetEnterprise(int id)
            {
                throw new NotImplementedException();
            }

            public override UsersWithRoles GetUsersWithRolesOfCurrentEnterprise()
            {
                throw new NotImplementedException();
            }

            public override UsersWithRoles GetUsersWithRoles(int enterpriseId)
            {
                throw new NotImplementedException();
            }

            public override User GetUserOfCurrentEnterprise(int id)
            {
                throw new NotImplementedException();
            }

            public override User GetUser(int enterpriseId, int id)
            {
                throw new NotImplementedException();
            }

            public override Roles GetRoles()
            {
                throw new NotImplementedException();
            }

            public override Role GetRole(int id)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachines GetAllVirtualMachines()
            {
                throw new NotImplementedException();
            }

            public override VirtualMachines GetVirtualMachines(int virtualDataCenterId, int virtualApplianceId)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachine GetVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int id)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId,
                int dataCenterRepositoryId, int virtualMachineTemplateId)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId,
                int dataCenterRepositoryId, int virtualMachineTemplateId, VirtualMachine virtualMachine)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref,
                VirtualMachine virtualMachine)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachineTemplates GetVirtualMachineTemplates(int enterpriseId, int dataCenterRepositoryId)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachineTemplate GetVirtualMachineTemplate(int enterpriseId, int dataCenterRepositoryId, int id)
            {
                throw new NotImplementedException();
            }

            public override VirtualDataCenters GetVirtualDataCenters()
            {
                throw new NotImplementedException();
            }

            public override VirtualDataCenter GetVirtualDataCenter(int id)
            {
                throw new NotImplementedException();
            }

            public override VirtualAppliances GetVirtualAppliances(int virtualDataCenterId)
            {
                throw new NotImplementedException();
            }

            public override VirtualAppliance GetVirtualAppliance(int virtualDataCenterId, int id)
            {
                throw new NotImplementedException();
            }

            public override DataCenterRepositories GetDataCenterRepositoriesOfCurrentEnterprise()
            {
                throw new NotImplementedException();
            }

            public override DataCenterRepositories GetDataCenterRepositories(int enterpriseId)
            {
                throw new NotImplementedException();
            }

            public override DataCenterRepository GetDataCenterRepositoryOfCurrentEnterprise(int id)
            {
                throw new NotImplementedException();
            }

            public override DataCenterRepository GetDataCenterRepository(int enterpriseId, int id)
            {
                throw new NotImplementedException();
            }
        }
    }
}
