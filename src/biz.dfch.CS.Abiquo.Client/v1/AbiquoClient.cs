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
 
﻿using biz.dfch.CS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
﻿using System.Net.Http;
﻿using System.Text;
using System.Threading.Tasks;
﻿using biz.dfch.CS.Abiquo.Client.Authentication;
﻿using biz.dfch.CS.Abiquo.Client.Communication;
﻿using biz.dfch.CS.Abiquo.Client.General;
﻿using biz.dfch.CS.Abiquo.Client.v1.Model;
﻿using HttpMethod = biz.dfch.CS.Web.Utilities.Rest.HttpMethod;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
﻿using Task = biz.dfch.CS.Abiquo.Client.v1.Model.Task;

namespace biz.dfch.CS.Abiquo.Client.v1
{
    public class AbiquoClient : BaseAbiquoClient
    {
        public const string ABIQUO_API_VERSION = "3.8";

        internal AbiquoClient()
        {
            AbiquoApiVersion = ABIQUO_API_VERSION;
        }

        public override bool Login(string abiquoApiBaseUri, IAuthenticationInformation authenticationInformation)
        {
            Debug.WriteLine(string.Format("START Login (AbiquoApiBaseUri: '{0}'; TenantId: '{1}') ...", abiquoApiBaseUri, authenticationInformation.GetTenantId()));

            Logout();
            AuthenticationInformation = authenticationInformation;
            AbiquoApiBaseUri = abiquoApiBaseUri;

            try
            {
                ExecuteRequest(AbiquoUriSuffixes.LOGIN);

                IsLoggedIn = true;
                Trace.WriteLine("END Login SUCCEEDED");
                return true;
            }
            catch (HttpRequestException ex)
            {
                Logout();
                Trace.WriteLine(string.Format("END Login FAILED ('{0}')", ex.Message));
                return false;
            }
        }

        #region Enterprises

        public override Enterprises GetEnterprises()
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISES).GetHeaders();

            return Invoke<Enterprises>(AbiquoUriSuffixes.ENTERPRISES, headers);
        }

        public override Enterprise GetCurrentEnterprise()
        {
            return GetEnterprise(AuthenticationInformation.GetTenantId());
        }

        public override Enterprise GetEnterprise(int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISE).GetHeaders();
            var uriSuffix = string.Format(AbiquoUriSuffixes.ENTERPRISE_BY_ID, id);

            return Invoke<Enterprise>(uriSuffix, headers);
        }

        #endregion Enterprises


        #region Users

        public override UsersWithRoles GetUsersWithRolesOfCurrentEnterprise()
        {
            return GetUsersWithRoles(AuthenticationInformation.GetTenantId());
        }

        public override UsersWithRoles GetUsersWithRoles(int enterpriseId)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_USERSWITHROLES).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.USERSWITHROLES_BY_ENTERPRISE_ID, enterpriseId);
            return Invoke<UsersWithRoles>(uriSuffix, headers);
        }

        public override User GetUserOfCurrentEnterprise(int id)
        {
            return GetUser(AuthenticationInformation.GetTenantId(), id);
        }

        public override User GetUser(int enterpriseId, int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_USER).GetHeaders();
            var uriSuffix = string.Format(AbiquoUriSuffixes.USER_BY_ENTERPRISE_ID_AND_USER_ID, enterpriseId, id);
            
            return Invoke<User>(uriSuffix, headers);
        }

        #endregion Users


        #region Roles

        public override Roles GetRoles()
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_ROLES).GetHeaders();

            return Invoke<Roles>(AbiquoUriSuffixes.ROLES, headers);
        }

        public override Role GetRole(int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_ROLE).GetHeaders();
            
            var uriSuffix = string.Format(AbiquoUriSuffixes.ROLE_BY_ID, id);
            return Invoke<Role>(uriSuffix, headers);
        }

        #endregion Roles


        #region VirtualMachines

        public override VirtualMachines GetAllVirtualMachines()
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINES).GetHeaders();

            return Invoke<VirtualMachines>(AbiquoUriSuffixes.VIRTUALMACHINES, headers);
        }

        public override VirtualMachines GetVirtualMachines(int virtualDataCenterId, int virtualApplianceId)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINES).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALMACHINES_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID, virtualDataCenterId, virtualApplianceId);
            return Invoke<VirtualMachines>(uriSuffix, headers);
        }

        public override VirtualMachine GetVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINE).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALMACHINE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID_AND_VIRTUALMACHINE_ID, virtualDataCenterId, virtualApplianceId, id);
            return Invoke<VirtualMachine>(uriSuffix, headers);
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId,
            int virtualMachineTemplateId)
        {
            return CreateVirtualMachine(virtualDataCenterId, virtualApplianceId, enterpriseId, dataCenterRepositoryId, virtualMachineTemplateId, new VirtualMachineBase());
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref)
        {
            return CreateVirtualMachine(virtualDataCenterId, virtualApplianceId, virtualMachineTemplateHref, new VirtualMachineBase());
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId,
            int virtualMachineTemplateId, VirtualMachineBase virtualMachine)
        {
            var virtualMachineTemplateHrefSuffix = string.Format(AbiquoUriSuffixes.VIRTUALMACHINETEMPLATE_BY_ENTERPISE_ID_AND_DATACENTERREPOSITORY_ID_AND_VIRTUALMACHINETEMPLATE_ID,
                enterpriseId, dataCenterRepositoryId, virtualMachineTemplateId);
            var virtualMachineTemplateHref = UriHelper.ConcatUri(AbiquoApiBaseUri, virtualMachineTemplateHrefSuffix);

            return CreateVirtualMachine(virtualDataCenterId, virtualApplianceId, virtualMachineTemplateHref, virtualMachine);
        }

        public override VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref,
            VirtualMachineBase virtualMachine)
        {
            var virtualMachineLink = new LinkBuilder()
                .BuildRel(AbiquoRelations.VIRTUALMACHINETEMPLATE)
                .BuildHref(virtualMachineTemplateHref)
                .GetLink();

            virtualMachine.Links = new List<Link>() { virtualMachineLink };
            
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINE)
                .BuildContentType(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINE).GetHeaders();

            var uriSuffix =
                string.Format(AbiquoUriSuffixes.VIRTUALMACHINES_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID,
                    virtualDataCenterId, virtualApplianceId);

            return Invoke<VirtualMachine>(HttpMethod.Post, uriSuffix, null, headers, virtualMachine.SerializeObject());
        }

        public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force)
        {
            return DeployVirtualMachine(virtualDataCenterId, virtualApplianceId, virtualMachineId, false, false);
        }

        public override Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force, bool waitForCompletion)
        {
            var filter = new Dictionary<string, object>();
            if (force)
            {
                filter.Add("force", "true");
            }

            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_ACCEPTEDREQUEST).GetHeaders();

            var uriSuffix =
                string.Format(AbiquoUriSuffixes.DEPLOY_VIRTUALMACHINE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID_AND_VIRTUALMACHINE_ID,
                    virtualDataCenterId, virtualApplianceId, virtualMachineId);

            var deploymentResult = Invoke<AcceptedRequest>(HttpMethod.Post, uriSuffix, filter, headers);
            Contract.Assert(null != deploymentResult);

            var link = deploymentResult.GetLinkByRel(AbiquoRelations.STATUS);
            var taskId = UriHelper.ExtractLastSegmentAsString(link.Href);

            var task = GetTaskOfVirtualMachine(virtualDataCenterId, virtualApplianceId, virtualMachineId, taskId);

            if (waitForCompletion)
            {
                
            }

            return task;
        }

        public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachine virtualMachine)
        {
            return UpdateVirtualMachine(virtualDataCenterId, virtualApplianceId, virtualMachineId, virtualMachine, false);
        }

        public override Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachine virtualMachine, bool waitForCompletion)
        {
            throw new NotImplementedException();
        }

        public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachineState state)
        {
            return ChangeStateOfVirtualMachine(virtualDataCenterId, virtualApplianceId, virtualMachineId, state, false);
        }

        public override Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId,
            VirtualMachineState state, bool waitForCompletion)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force = false)
        {
            var filter = new Dictionary<string, object>();
            if (force)
            {
                filter.Add("force", "true");
            }

            var uriSuffix = 
                string.Format(AbiquoUriSuffixes.VIRTUALMACHINE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID_AND_VIRTUALMACHINE_ID, 
                virtualDataCenterId, virtualApplianceId, virtualMachineId);

            Invoke(HttpMethod.Delete, uriSuffix, filter, null);

            return true;
        }

        public override Tasks GetAllTasksOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_TASKS).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALMACHINETASKS_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPPLIANCE_ID_AND_VIRTUALMACHINE_ID, virtualDataCenterId, virtualApplianceId, virtualMachineId);
            return Invoke<Tasks>(uriSuffix, headers);
        }

        public override Task GetTaskOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, string taskId)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_TASK).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALMACHINETASK_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPPLIANCE_ID_AND_VIRTUALMACHINE_ID_AND_TASK_ID, virtualDataCenterId, virtualApplianceId, virtualMachineId, taskId);
            return Invoke<Task>(uriSuffix, headers);
        }

        #endregion VirtualMachines


        #region VirtualMachineTemplates

        public override VirtualMachineTemplates GetVirtualMachineTemplates(int enterpriseId, int dataCenterRepositoryId)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINETEMPLATES).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALMACHINETEMPLATES_BY_ENTERPISE_ID_AND_DATACENTERREPOSITORY_ID, enterpriseId, dataCenterRepositoryId);
            return Invoke<VirtualMachineTemplates>(uriSuffix, headers);
        }

        public override VirtualMachineTemplate GetVirtualMachineTemplate(int enterpriseId, int dataCenterRepositoryId, int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINETEMPLATE).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALMACHINETEMPLATE_BY_ENTERPISE_ID_AND_DATACENTERREPOSITORY_ID_AND_VIRTUALMACHINETEMPLATE_ID, enterpriseId, dataCenterRepositoryId, id);
            return Invoke<VirtualMachineTemplate>(uriSuffix, headers);
        }

        #endregion VirtualMachineTemplates


        #region VirtualDataCenters

        public override VirtualDataCenters GetVirtualDataCenters()
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALDATACENTERS).GetHeaders();

            return Invoke<VirtualDataCenters>(AbiquoUriSuffixes.VIRTUALDATACENTERS, headers);
        }

        public override VirtualDataCenter GetVirtualDataCenter(int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALDATACENTER).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALDATACENTER_BY_ID, id);
            return Invoke<VirtualDataCenter>(uriSuffix, headers);
        }

        #endregion VirtualDataCenters


        #region VirtualAppliances

        public override VirtualAppliances GetVirtualAppliances(int virtualDataCenterId)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALAPPLIANCES).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALAPPLIANCES_BY_VIRTUALDATACENTER_ID, virtualDataCenterId);
            return Invoke<VirtualAppliances>(uriSuffix, headers);
        }

        public override VirtualAppliance GetVirtualAppliance(int virtualDataCenterId, int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALAPPLIANCE).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.VIRTUALAPPLIANCE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID, virtualDataCenterId, id);
            return Invoke<VirtualAppliance>(uriSuffix, headers);
        }

        #endregion VirtualAppliances


        #region DataCenterRepositories

        public override DataCenterRepositories GetDataCenterRepositoriesOfCurrentEnterprise()
        {
            return GetDataCenterRepositories(AuthenticationInformation.GetTenantId());
        }

        public override DataCenterRepositories GetDataCenterRepositories(int enterpriseId)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_DATACENTERREPOSITORIES).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.DATACENTERREPOSITORIES_BY_ENTERPRISE_ID, enterpriseId);
            return Invoke<DataCenterRepositories>(uriSuffix, headers);
        }

        public override DataCenterRepository GetDataCenterRepositoryOfCurrentEnterprise(int id)
        {
            return GetDataCenterRepository(AuthenticationInformation.GetTenantId(), id);
        }

        public override DataCenterRepository GetDataCenterRepository(int enterpriseId, int id)
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_DATACENTERREPOSITORY).GetHeaders();

            var uriSuffix = string.Format(AbiquoUriSuffixes.DATACENTERREPOSITORIES_BY_ENTERPRISE_ID_AND_DATACENTERREPOSITORY_ID, enterpriseId, id);
            return Invoke<DataCenterRepository>(uriSuffix, headers);
        }

        #endregion DataCenterRepositories


        #region Tasks

        public override Task WaitForTaskCompletion(Task task, int taskPollingWaitTimeMilliseconds, int taskTimeoutMilliseconds)
        {
            throw new NotImplementedException();

            /*
            
                        #region Contract
            Contract.Requires(this.IsLoggedIn, "Not logged in, call method login first");
            Contract.Requires(!string.IsNullOrEmpty(id), "No job id defined");
            Contract.Requires(totalAttempts > 0, "TotalAttempts must be greater than 0");
            Contract.Requires(baseWaitingMilliseconds > 0, "BaseWaitingMilliseconds must be greater than 0");
            Contract.Requires(basePollingWaitingMilliseconds > 0, "BasePollingWaitingMilliseconds must be greater than 0");
            Contract.Requires(timeOut > 0, "TimeOut must be greater than 0");
            #endregion Contract

            Trace.WriteLine(string.Format("v2.CimiClient.WaitForJobCompletion({0}, {1}, {2}, {3}, {4})", id, totalAttempts, baseWaitingMilliseconds, basePollingWaitingMilliseconds, timeOut));

            DateTime timeLimit = DateTime.Now.AddMilliseconds(timeOut);
            int currentPollingWaitingTime = basePollingWaitingMilliseconds;
            while (DateTime.Now < timeLimit)
            {
                Model.PostModel.Job job = this.GetJob(id, totalAttempts, baseWaitingMilliseconds);
                switch (job.State)
                {
                    case Job.StateEnum.FAILED:
                    case Job.StateEnum.STOPPED:
                    case Job.StateEnum.SUCCESS:
                        return job.State;
                }
                Thread.Sleep(currentPollingWaitingTime);
                currentPollingWaitingTime = System.Convert.ToInt32(Math.Floor(currentPollingWaitingTime * 1.5));
            }
            Model.PostModel.Job jobAtEndOfTimeout = this.GetJob(id, totalAttempts, baseWaitingMilliseconds);
            switch (jobAtEndOfTimeout.State)
            {
                case Job.StateEnum.FAILED:
                case Job.StateEnum.STOPPED:
                case Job.StateEnum.SUCCESS:
                    return jobAtEndOfTimeout.State;
            }
            
            throw new TimeoutException(string.Format("Timeout exceeded while waiting for Job with ID '{0}'", id));

            */
        }

        #endregion Tasks
    }
}
