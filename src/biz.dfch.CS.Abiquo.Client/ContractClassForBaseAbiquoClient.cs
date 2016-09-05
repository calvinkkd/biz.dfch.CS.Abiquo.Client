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
﻿using biz.dfch.CS.Abiquo.Client.v1.Model;
﻿using Task = biz.dfch.CS.Abiquo.Client.v1.Model.Task;

namespace biz.dfch.CS.Abiquo.Client
{
    [ContractClassFor(typeof(BaseAbiquoClient))]
    abstract class ContractClassForBaseAbiquoClient : BaseAbiquoClient
    {
        public override bool Login(string abiquoApiBaseUri, IAuthenticationInformation authenticationInformation)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(abiquoApiBaseUri));
            Contract.Requires(null != authenticationInformation);
            Contract.Ensures(Contract.Result<bool>() == !string.IsNullOrWhiteSpace(this.AbiquoApiBaseUri));
            Contract.Ensures(Contract.Result<bool>() == (null != this.AuthenticationInformation));

            return default(bool);
        }


        #region Enterprises

        public override Enterprises GetEnterprises()
        {
            Contract.Ensures(null != Contract.Result<Enterprises>());

            return default(Enterprises);
        }

        public override Enterprise GetCurrentEnterprise()
        {
            return default(Enterprise);
        }

        public override Enterprise GetEnterprise(int id)
        {
            Contract.Requires(0 < id);

            return default(Enterprise);
        }

        #endregion Enterprises


        #region Users

        public override UsersWithRoles GetUsersWithRolesOfCurrentEnterprise()
        {
            Contract.Ensures(null != Contract.Result<UsersWithRoles>());

            return default(UsersWithRoles);
        }

        public override UsersWithRoles GetUsersWithRoles(int enterpriseId)
        {
            Contract.Requires(0 < enterpriseId);
            Contract.Ensures(null != Contract.Result<UsersWithRoles>());

            return default(UsersWithRoles);
        }

        public override User GetUserOfCurrentEnterprise(int id)
        {
            Contract.Requires(0 < id);

            return default(User);
        }

        public override User GetUser(int enterpriseId, int id)
        {
            Contract.Requires(0 < enterpriseId);
            Contract.Requires(0 < id);

            return default(User);
        }

        #endregion Users


        #region Roles

        public override Roles GetRoles()
        {
            Contract.Ensures(null != Contract.Result<Roles>());

            return default(Roles);
        }

        public override Role GetRole(int id)
        {
            Contract.Requires(0 < id);

            return default(Role);
        }

        #endregion Users


        #region VirtualMachines

        public override VirtualMachines GetAllVirtualMachines()
        {
            Contract.Ensures(null != Contract.Result<VirtualMachines>());

            return default(VirtualMachines);
        }

        public override VirtualMachines GetVirtualMachines(int virtualDataCenterId, int virtualApplianceId)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Ensures(null != Contract.Result<VirtualMachines>());

            return default(VirtualMachines);
        }

        public override VirtualMachine GetVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int id)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < id);

            return default(VirtualMachine);
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId,
            int virtualMachineTemplateId)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < enterpriseId);
            Contract.Requires(0 < dataCenterRepositoryId);
            Contract.Requires(0 < virtualMachineTemplateId);

            return default(VirtualMachine);
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(!string.IsNullOrWhiteSpace(virtualMachineTemplateHref));
            Contract.Requires(Uri.IsWellFormedUriString(virtualMachineTemplateHref, UriKind.Absolute));

            return default(VirtualMachine);
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId,
            int virtualMachineTemplateId, VirtualMachineBase virtualMachine)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < enterpriseId);
            Contract.Requires(0 < dataCenterRepositoryId);
            Contract.Requires(0 < virtualMachineTemplateId);
            Contract.Requires(null != virtualMachine);
            Contract.Requires(virtualMachine.IsValid());

            return default(VirtualMachine);
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref,
            VirtualMachineBase virtualMachine)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(!string.IsNullOrWhiteSpace(virtualMachineTemplateHref));
            Contract.Requires(Uri.IsWellFormedUriString(virtualMachineTemplateHref, UriKind.Absolute));
            Contract.Requires(null != virtualMachine);
            Contract.Requires(virtualMachine.IsValid());

            return default(VirtualMachine);
        }

        public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);

            return default(Task);
        }

        public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force, bool waitForCompletion)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);

            return default(Task);
        }

        public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachine virtualMachine)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);
            Contract.Requires(null != virtualMachine);
            Contract.Requires(virtualMachine.IsValid());

            return default(Task);
        }

        public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachine virtualMachine, bool waitForCompletion)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);
            Contract.Requires(null != virtualMachine);
            Contract.Requires(virtualMachine.IsValid());

            return default(Task);
        }

        public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachineState state)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);

            return default(Task);
        }

        public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachineState state, bool waitForCompletion)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);

            return default(Task);
        }

        public override bool DeleteVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force = false)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);

            return default(bool);
        }

        public override Tasks GetAllTasksOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);
            Contract.Ensures(null != Contract.Result<Tasks>());

            return default(Tasks);
        }

        public override Task GetTaskOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, string taskId)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < virtualApplianceId);
            Contract.Requires(0 < virtualMachineId);
            Contract.Requires(!string.IsNullOrWhiteSpace(taskId));

            return default(Task);
        }

        #endregion VirtualMachines


        #region VirtualMachineTemplates

        public override VirtualMachineTemplates GetVirtualMachineTemplates(int enterpriseId, int dataCenterRepositoryId)
        {
            Contract.Requires(0 < enterpriseId);
            Contract.Requires(0 < dataCenterRepositoryId);
            Contract.Ensures(null != Contract.Result<VirtualMachineTemplates>());

            return default(VirtualMachineTemplates);
        }

        public override VirtualMachineTemplate GetVirtualMachineTemplate(int enterpriseId, int dataCenterRepositoryId, int id)
        {
            Contract.Requires(0 < enterpriseId);
            Contract.Requires(0 < dataCenterRepositoryId);
            Contract.Requires(0 < id);

            return default(VirtualMachineTemplate);
        }

        #endregion VirtualMachineTemplates


        #region VirtualDataCenters

        public override VirtualDataCenters GetVirtualDataCenters()
        {
            Contract.Ensures(null != Contract.Result<VirtualDataCenters>());

            return default(VirtualDataCenters);
        }

        public override VirtualDataCenter GetVirtualDataCenter(int id)
        {
            Contract.Requires(0 < id);

            return default(VirtualDataCenter);
        }

        #endregion VirtualDataCenters


        #region VirtualAppliances

        public override VirtualAppliances GetVirtualAppliances(int virtualDataCenterId)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Ensures(null != Contract.Result<VirtualAppliances>());

            return default(VirtualAppliances);
        }

        public override VirtualAppliance GetVirtualAppliance(int virtualDataCenterId, int id)
        {
            Contract.Requires(0 < virtualDataCenterId);
            Contract.Requires(0 < id);

            return default(VirtualAppliance);
        }

        #endregion VirtualAppliances


        #region DataCenterRepositories

        public override DataCenterRepositories GetDataCenterRepositoriesOfCurrentEnterprise()
        {
            Contract.Ensures(null != Contract.Result<DataCenterRepositories>());

            return default(DataCenterRepositories);
        }

        public override DataCenterRepositories GetDataCenterRepositories(int enterpriseId)
        {
            Contract.Requires(0 < enterpriseId);
            Contract.Ensures(null != Contract.Result<DataCenterRepositories>());

            return default(DataCenterRepositories);
        }

        public override DataCenterRepository GetDataCenterRepositoryOfCurrentEnterprise(int id)
        {
            Contract.Requires(0 < id);

            return default(DataCenterRepository);
        }

        public override DataCenterRepository GetDataCenterRepository(int enterpriseId, int id)
        {
            Contract.Requires(0 < enterpriseId);
            Contract.Requires(0 < id);
            
            return default(DataCenterRepository);
        }

        #endregion DataCenterRepositories


        #region Tasks

        public override Task WaitForTaskCompletion(biz.dfch.CS.Abiquo.Client.v1.Model.Task task, int basePollingWaitTimeMilliseconds, int timeoutMilliseconds)
        {
            Contract.Requires(null != task);
            Contract.Requires(task.IsValid());
            Contract.Requires(0 < basePollingWaitTimeMilliseconds);
            Contract.Requires(0 < timeoutMilliseconds);

            return default(Task);
        }

        #endregion Tasks
    }
}
