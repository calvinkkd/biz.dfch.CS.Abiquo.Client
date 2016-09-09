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
﻿using Task = biz.dfch.CS.Abiquo.Client.v1.Model.Task;

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
        private const string BEARER_TOKEN = "Bearer TESTTOKEN";

        private BaseAbiquoClient sut = new DummyAbiquoClient();

        private readonly VirtualMachine validVirtualMachine = new VirtualMachine()
        {
            Cpu = 2
            ,
            Ram = 512
            ,
            Name = "Arbitrary"
        };

        private readonly Task validTask = new Task()
        {
            OwnerId = "ArbitraryOwnerId"
            ,
            State = TaskStateEnum.FINISHED_SUCCESSFULLY
            ,
            TaskId = Guid.NewGuid().ToString()
            ,
            Timestamp = 1
            ,
            Type = TaskTypeEnum.DEPLOY
        };

        private readonly VirtualMachineState virtualMachineState = new VirtualMachineState()
        {
            State = VirtualMachineStateEnum.ON
        };

        [TestMethod]
        [ExpectContractFailure]
        public void InvalidAbqiuoClientThatDoesNotSetVersionPropertyThrowsContractExceptionOnInstantiation()
        {
            // Arrange

            // Act
            new InvalidAbiquoClient(null, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvalidAbqiuoClientThatDoesNotSetValidTaskPollingWaitTimeMillisecondsPropertyThrowsContractExceptionOnInstantiation()
        {
            // Arrange

            // Act
            new InvalidAbiquoClient("v1", 0, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvalidAbqiuoClientThatDoesNotSetValidTaskPollingTimeoutMillisecondsPropertyThrowsContractExceptionOnInstantiation()
        {
            // Arrange

            // Act
            new InvalidAbiquoClient("v1", 42, 0);

            // Assert
        }

        #region ExecuteRequest

        [TestMethod]
        public void ExecuteRequestWithoutAdditionalHeadersAndBodyCallsRestCallExecutor()
        {
            // Arrange
            sut.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var expectedRequestUri = UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES);
            
            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, authenticationInformation.GetAuthorizationHeaders(), null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = sut.ExecuteRequest(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null);

            // Assert
            Assert.AreEqual("Arbitrary-Result", result);

            Mock.Assert(restCallExecutor);
        }

        [TestMethod]
        public void ExecuteRequestWithAdditionalHeadersMergesHeadersAndCallsRestCallExecutorWithMergedHeaders()
        {
            // Arrange
            sut.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var expectedRequestUri = UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES);

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { AbiquoHeaderKeys.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = sut.ExecuteRequest(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, headers, null);

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
            sut.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            sut.Invoke<Enterprise>(null, new Dictionary<string, string>());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GenericInvokeIfNotLoggedInThrowsContractException()
        {
            // Arrange

            // Act
            sut.Invoke<Enterprises>(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null, default(string));

            // Assert
        }

        #endregion Generic Invoke


        #region Invoke

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithEmptyUriSuffixThrowsContractException()
        {
            // Arrange
            sut.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            sut.Invoke(HttpMethod.Get, " ", null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidUriSuffixThrowsContractException()
        {
            // Arrange
            sut.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            // Act
            sut.Invoke(HttpMethod.Get, "http://example.com", null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeIfNotLoggedInThrowsContractException()
        {
            // Arrange

            // Act
            sut.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, null, null, default(BaseDto));

            // Assert
        }

        [TestMethod]
        public void InvokeWithFilterCallsRestCallExecutorWithRequestUriContainingFilterExpression()
        {
            // Arrange
            sut.Login(ABIQUO_API_BASE_URI, authenticationInformation);

            var filter = new FilterBuilder()
                .BuildFilterPart("currentPage", 1)
                .BuildFilterPart("limit", 25)
                .GetFilter();

            var expectedRequestUri = string.Format("{0}?{1}", UriHelper.ConcatUri(ABIQUO_API_BASE_URI, AbiquoUriSuffixes.ENTERPRISES), "currentPage=1&limit=25");

            var headers = new Dictionary<string, string>()
            {
                { Constants.AUTHORIZATION_HEADER_KEY, BEARER_TOKEN }
                ,
                { AbiquoHeaderKeys.ACCEPT_HEADER_KEY, AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES }
            };

            var restCallExecutor = Mock.Create<RestCallExecutor>();
            Mock.Arrange(() => restCallExecutor.Invoke(HttpMethod.Get, expectedRequestUri, headers, null))
                .IgnoreInstance()
                .Returns("Arbitrary-Result")
                .OccursOnce();

            // Act
            var result = sut.Invoke(HttpMethod.Get, AbiquoUriSuffixes.ENTERPRISES, filter, headers);

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

            // Act
            sut.GetEnterprise(INVALID_ID);

            // Assert
        }

        #endregion Enterprises


        #region Users

        [TestMethod]
        [ExpectContractFailure]
        public void GetUsersWithRolesWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetUsersWithRoles(INVALID_ID);

            // Assert
        }        
        
        [TestMethod]
        [ExpectContractFailure]
        public void GetUserOfCurrentEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetUserOfCurrentEnterprise(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetUserWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetUser(INVALID_ID, 15);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetUserWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetUser(42, INVALID_ID);

            // Assert
        }

        #endregion Users


        #region Roles

        [TestMethod]
        [ExpectContractFailure]
        public void GetRoleWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetRole(INVALID_ID);

            // Assert
        }

        #endregion Roles


        #region DataCentersLimits

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCentersLimitsWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCentersLimits(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterLimitsOfCurrentEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCenterLimitsOfCurrentEnterprise(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterLimitsWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCenterLimits(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterLimitsWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCenterLimits(42, INVALID_ID);

            // Assert
        }

        #endregion DataCentersLimits


        #region VirtualMachines

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachinesWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachines(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachinesWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachines(42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachine(INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachine(42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachine(42, 42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(0, 42, VIRTUALMACHINETEMPLATE_HREF);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 0, VIRTUALMACHINETEMPLATE_HREF);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithNullVirtualMachineTemplateHrefThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithEmptyVirtualMachineTemplateHrefThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, " ");
            
            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineTemplateHrefThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, "Arbitrary");

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualDataCenterId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(INVALID_ID, 42, 42, 42, 42, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualApplianceId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, INVALID_ID, 42, 42, 42, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, INVALID_ID, 42, 42, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidDataCenterRepositoryIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, 42, INVALID_ID, 42, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineTemplateIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, 42, 42, INVALID_ID, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithNullVirtualMachineThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, 42, 42, 42, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualDataCenterId3ThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(INVALID_ID, 42, VIRTUALMACHINETEMPLATE_HREF, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualApplianceId3ThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, INVALID_ID, VIRTUALMACHINETEMPLATE_HREF, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithNullVirtualMachineTemplateHref2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, null, validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithEmptyVirtualMachineTemplateHref2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, " ", validVirtualMachine);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, VIRTUALMACHINETEMPLATE_HREF, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateVirtualMachineWithInvalidVirtualMachineTemplateHref2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.CreateVirtualMachine(42, 42, "Arbitrary", null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeployVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeployVirtualMachine(INVALID_ID, 42, 42, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeployVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeployVirtualMachine(42, INVALID_ID, 42, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeployVirtualMachineWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeployVirtualMachine(42, 42, INVALID_ID, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeployVirtualMachineWithInvalidVirtualDataCenterId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeployVirtualMachine(INVALID_ID, 42, 42, false, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeployVirtualMachineWithInvalidVirtualApplianceId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeployVirtualMachine(42, INVALID_ID, 42, false, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeployVirtualMachineWithInvalidVirtualMachineId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeployVirtualMachine(42, 42, INVALID_ID, false, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void UpdateVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.UpdateVirtualMachine(INVALID_ID, 42, 42, validVirtualMachine, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void UpdateVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.UpdateVirtualMachine(42, INVALID_ID, 42, validVirtualMachine, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void UpdateVirtualMachineWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.UpdateVirtualMachine(42, 42, INVALID_ID, validVirtualMachine, false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void UpdateVirtualMachineWithInvalidVirtualMachineThrowsContractException()
        {
            // Arrange

            // Act
            sut.UpdateVirtualMachine(42, 42, 42, new VirtualMachine(), false);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void UpdateVirtualMachineWithInvalidVirtualDataCenterId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.UpdateVirtualMachine(INVALID_ID, 42, 42, validVirtualMachine, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void UpdateVirtualMachineWithInvalidVirtualApplianceId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.UpdateVirtualMachine(42, INVALID_ID, 42, validVirtualMachine, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void UpdateVirtualMachineWithInvalidVirtualMachineId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.UpdateVirtualMachine(42, 42, INVALID_ID, validVirtualMachine, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ChangeStateOfVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.ChangeStateOfVirtualMachine(INVALID_ID, 42, 42, virtualMachineState);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ChangeStateOfVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.ChangeStateOfVirtualMachine(42, INVALID_ID, 42, virtualMachineState);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ChangeStateOfVirtualMachineWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.ChangeStateOfVirtualMachine(42, 42, INVALID_ID, virtualMachineState);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ChangeStateOfVirtualMachineWithInvalidVirtualDataCenterId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.ChangeStateOfVirtualMachine(INVALID_ID, 42, 42, virtualMachineState, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ChangeStateOfVirtualMachineWithInvalidVirtualApplianceId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.ChangeStateOfVirtualMachine(42, INVALID_ID, 42, virtualMachineState, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ChangeStateOfVirtualMachineWithInvalidVirtualMachineId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.ChangeStateOfVirtualMachine(42, 42, INVALID_ID, virtualMachineState, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ChangeStateOfVirtualMachineWithInvalidVirtualMachineState2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.ChangeStateOfVirtualMachine(42, 42, INVALID_ID, new VirtualMachineState(), true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeleteVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeleteVirtualMachine(0, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeleteVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeleteVirtualMachine(42, 0, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeleteVirtualMachineWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeleteVirtualMachine(42, 42, 0);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeleteVirtualMachineWithInvalidVirtualDataCenterId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeleteVirtualMachine(0, 42, 42, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeleteVirtualMachineWithInvalidVirtualApplianceId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeleteVirtualMachine(42, 0, 42, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeleteVirtualMachineWithInvalidVirtualMachineId2ThrowsContractException()
        {
            // Arrange

            // Act
            sut.DeleteVirtualMachine(42, 42, 0, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetNetworkConfigurationsForVmWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetNetworkConfigurationsForVm(INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetNetworkConfigurationsForVmWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetNetworkConfigurationsForVm(42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetNetworkConfigurationsForVmWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetNetworkConfigurationsForVm(42, 42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetNetworkConfigurationForVmWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetNetworkConfigurationForVm(INVALID_ID, 42, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetNetworkConfigurationForVmWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetNetworkConfigurationForVm(42, INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetNetworkConfigurationForVmWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetNetworkConfigurationForVm(42, 42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetNetworkConfigurationForVmWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetNetworkConfigurationForVm(42, 42, 42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetAllTasksOfVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetAllTasksOfVirtualMachine(INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetAllTasksOfVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetAllTasksOfVirtualMachine(42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetAllTasksOfVirtualMachineWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetAllTasksOfVirtualMachine(42, 42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetTaskOfVirtualMachineWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetTaskOfVirtualMachine(INVALID_ID, 42, 42, Guid.NewGuid().ToString());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetTaskOfVirtualMachineWithInvalidVirtualApplianceIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetTaskOfVirtualMachine(42, INVALID_ID, 42, Guid.NewGuid().ToString());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetTaskOfVirtualMachineWithInvalidVirtualMachineIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetTaskOfVirtualMachine(42, 42, INVALID_ID, Guid.NewGuid().ToString());

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetTaskOfVirtualMachineWithNullTaskIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetTaskOfVirtualMachine(42, 42, 42, "");

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetTaskOfVirtualMachineWithEmptyTaskIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetTaskOfVirtualMachine(42, 42, 42, "");

            // Assert
        }

        #endregion VirtualMachines


        #region VirtualMachineTemplates

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplatesWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachineTemplates(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplatesWithInvalidDataCenterRepositoryIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachineTemplates(42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplateWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachineTemplate(INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplateWithInvalidDataCenterRepositoryIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachineTemplate(42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualMachineTemplateWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualMachineTemplate(42, 42, INVALID_ID);

            // Assert
        }

        #endregion VirtualMachineTemplates


        #region VirtualDataCenters

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualDataCenterWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualDataCenter(INVALID_ID);

            // Assert
        }

        #endregion VirtualDataCenters


        #region VirtualAppliances

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualAppliancesWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualAppliances(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualApplianceWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualAppliance(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetVirtualApplianceWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetVirtualAppliance(42, INVALID_ID);

            // Assert
        }

        #endregion VirtualAppliances


        #region DataCenterRepositories

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoriesWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCenterRepositories(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoryOfCurrentEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCenterRepositoryOfCurrentEnterprise(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoryWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCenterRepository(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetDataCenterRepositoryWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetDataCenterRepository(42, INVALID_ID);

            // Assert
        }

        #endregion DataCenterRepositories


        #region Tasks

        [TestMethod]
        [ExpectContractFailure]
        public void WaitForTaskCompletionWithNullTaskThrowsContractException()
        {
            // Arrange

            // Act
            sut.WaitForTaskCompletion(null, 1, 1);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void WaitForTaskCompletionWithInvalidTaskThrowsContractException()
        {
            // Arrange

            // Act
            sut.WaitForTaskCompletion(new Task(), 1, 1);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void WaitForTaskCompletionWithInvalidBasePollingWaitTimeThrowsContractException()
        {
            // Arrange

            // Act
            sut.WaitForTaskCompletion(validTask, INVALID_ID, 1);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void WaitForTaskCompletionWithInvalidTimeoutThrowsContractException()
        {
            // Arrange

            // Act
            sut.WaitForTaskCompletion(validTask, 1, INVALID_ID);

            // Assert
        }

        #endregion Tasks


        #region Networks

        [TestMethod]
        [ExpectContractFailure]
        public void GetPrivateNetworksWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetPrivateNetworks(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetPrivateNetworkWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetPrivateNetwork(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetPrivateNetworkWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetPrivateNetwork(42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetIpsOfPrivateNetworkWithInvalidVirtualDataCenterIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetIpsOfPrivateNetwork(INVALID_ID, 42, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetIpsOfPrivateNetworkWithInvalidPrivateNetworkIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetIpsOfPrivateNetwork(42, INVALID_ID, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworksOfCurrentEnterpriseWithInvalidDataCenterLimitsIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetworksOfCurrentEnterprise(INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworksWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetworks(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworksWithInvalidDataCenterLimitsIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetworks(42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworkOfCurrentEnterpriseWithInvalidDataCenterLimitsIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetworkOfCurrentEnterprise(INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworkOfCurrentEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetworkOfCurrentEnterprise(42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworkWithInvalidLEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetwork(INVALID_ID, 42, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworkWithInvalidDataCenterLimitsIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetwork(42, INVALID_ID, 42);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetExternalNetworkWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetExternalNetwork(42, 42, INVALID_ID);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetIpsOfExternalNetworkOfCurrentEnterpriseWithInvalidDataCenterLimitsIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetIpsOfExternalNetworkOfCurrentEnterprise(INVALID_ID, 42, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetIpsOfExternalNetworkOfCurrentEnterpriseWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetIpsOfExternalNetworkOfCurrentEnterprise(42, INVALID_ID, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetIpsOfExternalNetworkWithInvalidEnterpriseIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetIpsOfExternalNetwork(INVALID_ID, 42, 42, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetIpsOfExternalNetworkWithInvalidDataCenterLimitsIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetIpsOfExternalNetwork(42, INVALID_ID, 42, true);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetIpsOfExternalNetworkWithInvalidIdThrowsContractException()
        {
            // Arrange

            // Act
            sut.GetIpsOfExternalNetwork(42, 42, INVALID_ID, true);

            // Assert
        }

        #endregion Newtorks


        private class DummyAbiquoClient : BaseAbiquoClient
        {
            public DummyAbiquoClient()
            {
                AbiquoApiVersion = "Arbitrary-Version";
                TaskPollingWaitTimeMilliseconds = 10 * 1000;
                TaskPollingTimeoutMilliseconds = 10 * 1000;
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

            public override DataCentersLimits GetDataCentersLimitsOfCurrentEnterprise()
            {
                return new DataCentersLimits();
            }

            public override DataCentersLimits GetDataCentersLimits(int enterpriseId)
            {
                return new DataCentersLimits();
            }

            public override DataCenterLimits GetDataCenterLimitsOfCurrentEnterprise(int id)
            {
                return new DataCenterLimits();
            }

            public override DataCenterLimits GetDataCenterLimits(int enterpriseId, int id)
            {
                return new DataCenterLimits();
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
                int virtualMachineTemplateId, VirtualMachineBase virtualMachine)
            {
                return new VirtualMachine();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref,
                VirtualMachineBase virtualMachine)
            {
                return new VirtualMachine();
            }

            public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force)
            {
                return new Task();
            }

            public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force, bool waitForCompletion)
            {
                return new Task();
            }

            public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachine virtualMachine, bool force)
            {
                return new Task();
            }

            public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachine virtualMachine, bool force, bool waitForCompletion)
            {
                return new Task();
            }

            public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachineState state)
            {
                return new Task();
            }

            public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachineState state, bool waitForCompletion)
            {
                return new Task();
            }

            public override VmNetworkConfigurations GetNetworkConfigurationsForVm(int virtualDataCenterId, int virtualApplianceId, 
                int virtualMachineId)
            {
                return new VmNetworkConfigurations();
            }

            public override VmNetworkConfiguration GetNetworkConfigurationForVm(int virtualDataCenterId, int virtualApplianceId,
                int virtualMachineId, int id)
            {
                return new VmNetworkConfiguration();
            }

            public override Nics GetNicsOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
            {
                return new Nics();
            }

            public override bool DeleteVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
            {
                return true;
            }

            public override bool DeleteVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force)
            {
                return true;
            }

            public override Tasks GetAllTasksOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
            {
                return new Tasks();
            }

            public override Task GetTaskOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, string taskId)
            {
                return new Task();
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

            public override Task WaitForTaskCompletion(Task task, int taskPollingWaitTimeMilliseconds, int taskPollingTimeoutMilliseconds)
            {
                return new Task();
            }

            public override VlanNetworks GetPrivateNetworks(int virtualDataCenterId)
            {
                return new VlanNetworks();
            }

            public override VlanNetwork GetPrivateNetwork(int virtualDataCenterId, int id)
            {
                return new VlanNetwork();
            }

            public override PrivateIps GetIpsOfPrivateNetwork(int virtualDataCenterId, int privateNetworkId, bool free)
            {
                return new PrivateIps();
            }

            public override VlanNetworks GetExternalNetworksOfCurrentEnterprise(int dataCenterLimitsId)
            {
                return new VlanNetworks();
            }

            public override VlanNetworks GetExternalNetworks(int enterpriseId, int dataCenterLimitsId)
            {
                return new VlanNetworks();
            }

            public override VlanNetwork GetExternalNetworkOfCurrentEnterprise(int dataCenterLimitsId, int id)
            {
                return new VlanNetwork();
            }

            public override VlanNetwork GetExternalNetwork(int enterpriseId, int dataCenterLimitsId, int id)
            {
                return new VlanNetwork();
            }

            public override ExternalIps GetIpsOfExternalNetworkOfCurrentEnterprise(int dataCenterLimitsId, int externalNetworkId, bool free)
            {
                return new ExternalIps();
            }

            public override ExternalIps GetIpsOfExternalNetwork(int enterpriseId, int dataCenterLimitsId, int externalNetworkId, bool free)
            {
                return new ExternalIps();
            }
        }

        private class InvalidAbiquoClient : BaseAbiquoClient
        {
            public InvalidAbiquoClient(string abiquoApiVersion, int taskPollingWaitTimeMilliseconds, int taskPollingTimeoutMilliseconds)
            {
                AbiquoApiVersion = abiquoApiVersion;
                TaskPollingWaitTimeMilliseconds = taskPollingWaitTimeMilliseconds;
                TaskPollingTimeoutMilliseconds = taskPollingTimeoutMilliseconds;
            }

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

            public override DataCentersLimits GetDataCentersLimitsOfCurrentEnterprise()
            {
                throw new NotImplementedException();
            }

            public override DataCentersLimits GetDataCentersLimits(int enterpriseId)
            {
                throw new NotImplementedException();
            }

            public override DataCenterLimits GetDataCenterLimitsOfCurrentEnterprise(int id)
            {
                throw new NotImplementedException();
            }

            public override DataCenterLimits GetDataCenterLimits(int enterpriseId, int id)
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
                int dataCenterRepositoryId, int virtualMachineTemplateId, VirtualMachineBase virtualMachine)
            {
                throw new NotImplementedException();
            }

            public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref,
                VirtualMachineBase virtualMachine)
            {
                throw new NotImplementedException();
            }

            public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force)
            {
                throw new NotImplementedException();
            }

            public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force, bool waitForCompletion)
            {
                throw new NotImplementedException();
            }

            public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachine virtualMachine, bool force)
            {
                throw new NotImplementedException();
            }

            public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachine virtualMachine, bool force, bool waitForCompletion)
            {
                throw new NotImplementedException();
            }

            public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachineState state)
            {
                throw new NotImplementedException();
            }

            public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
                VirtualMachineState state, bool waitForCompletion)
            {
                throw new NotImplementedException();
            }

            public override bool DeleteVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
            {
                throw new NotImplementedException();
            }

            public override bool DeleteVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force)
            {
                throw new NotImplementedException();
            }

            public override VmNetworkConfigurations GetNetworkConfigurationsForVm(int virtualDataCenterId, int virtualApplianceId,
                int virtualMachineId)
            {
                throw new NotImplementedException();
            }

            public override VmNetworkConfiguration GetNetworkConfigurationForVm(int virtualDataCenterId, int virtualApplianceId,
                int virtualMachineId, int id)
            {
                throw new NotImplementedException();
            }

            public override Nics GetNicsOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
            {
                throw new NotImplementedException();
            }

            public override Tasks GetAllTasksOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
            {
                throw new NotImplementedException();
            }

            public override Task GetTaskOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, string taskId)
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

            public override Task WaitForTaskCompletion(Task task, int taskPollingWaitTimeMilliseconds, int taskPollingTimeoutMilliseconds)
            {
                throw new NotImplementedException();
            }

            public override VlanNetworks GetPrivateNetworks(int virtualDataCenterId)
            {
                throw new NotImplementedException();
            }

            public override VlanNetwork GetPrivateNetwork(int virtualDataCenterId, int id)
            {
                throw new NotImplementedException();
            }

            public override PrivateIps GetIpsOfPrivateNetwork(int virtualDataCenterId, int privateNetworkId, bool free)
            {
                throw new NotImplementedException();
            }

            public override VlanNetworks GetExternalNetworksOfCurrentEnterprise(int dataCenterLimitsId)
            {
                throw new NotImplementedException();
            }

            public override VlanNetworks GetExternalNetworks(int enterpriseId, int dataCenterLimitsId)
            {
                throw new NotImplementedException();
            }

            public override VlanNetwork GetExternalNetworkOfCurrentEnterprise(int dataCenterLimitsId, int id)
            {
                throw new NotImplementedException();
            }

            public override VlanNetwork GetExternalNetwork(int enterpriseId, int dataCenterLimitsId, int id)
            {
                throw new NotImplementedException();
            }

            public override ExternalIps GetIpsOfExternalNetworkOfCurrentEnterprise(int dataCenterLimitsId, int externalNetworkId, bool free)
            {
                throw new NotImplementedException();
            }

            public override ExternalIps GetIpsOfExternalNetwork(int enterpriseId, int dataCenterLimitsId, int externalNetworkId, bool free)
            {
                throw new NotImplementedException();
            }
        }
    }
}
