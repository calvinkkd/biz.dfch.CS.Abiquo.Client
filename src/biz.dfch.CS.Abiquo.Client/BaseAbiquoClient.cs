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
 
﻿using biz.dfch.CS.Abiquo.Client.Authentication;
using biz.dfch.CS.Utilities.General;
using biz.dfch.CS.Utilities.Logging;
using biz.dfch.CS.Web.Utilities.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Abiquo.Client.General;
﻿using biz.dfch.CS.Abiquo.Client.v1.Model;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("biz.dfch.CS.Abiquo.Client.Tests")]
namespace biz.dfch.CS.Abiquo.Client
{
    [ContractClass(typeof(ContractClassForBaseAbiquoClient))]
    public abstract class BaseAbiquoClient
    {
        /// <summary>
        /// The Abiquo Api version the client is implemented for.
        /// Has to be set in the constructor of the derived class
        /// </summary>
        public string AbiquoApiVersion { get; protected set; }

        /// <summary>
        /// Indicates that the call to the /login endpoint succeeded
        /// with the provided authentication information.
        /// </summary>
        public bool IsLoggedIn { get; protected set; }

        /// <summary>
        /// Base URI of the Abiquo API
        /// </summary>
        public string AbiquoApiBaseUri { get; protected set; }

        /// <summary>
        /// Holds the authentication information, that get injected by the Login method 
        /// </summary>
        public IAuthenticationInformation AuthenticationInformation { get; protected set; }


        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(AbiquoApiVersion));
        }


        public abstract bool Login(string abiquoApiBaseUri, IAuthenticationInformation authenticationInformation);

        public void Logout()
        {
            Debug.WriteLine(string.Format("START {0}", Method.fn()));

            IsLoggedIn = false;
            AbiquoApiBaseUri = null;
            AuthenticationInformation = null;

            Trace.WriteLine(string.Format("END {0}", Method.fn()));
        }

        #region ExecuteRequest

        internal string ExecuteRequest(string uriSuffix)
        {
            return ExecuteRequest(HttpMethod.Get, uriSuffix, null, null);
        }

        internal string ExecuteRequest(HttpMethod httpMethod, string uriSuffix, IDictionary<string, string> headers, string body)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(uriSuffix));
            Contract.Requires(!string.IsNullOrWhiteSpace(AbiquoApiBaseUri));
            Contract.Requires(null != AuthenticationInformation);

            var requestUri = UriHelper.ConcatUri(AbiquoApiBaseUri, uriSuffix);
            Debug.WriteLine(string.Format("START Executing request '{0} {1} - {2} - {3}' ...", httpMethod, requestUri, headers, body));

            var requestHeaders = new Dictionary<string, string>(AuthenticationInformation.GetAuthorizationHeaders());
            if (null != headers)
            {
                headers.ToList().ForEach(header => requestHeaders[header.Key] = header.Value);
            }

            var restCallExecutor = new RestCallExecutor();
            var result = restCallExecutor.Invoke(httpMethod, requestUri, requestHeaders, body);

            Trace.WriteLine(string.Format("END Executing request '{0} {1}' SUCCEEDED", httpMethod, requestUri));
            return result;
        }

        #endregion ExecuteRequest


        #region Generic Invoke

        public T Invoke<T>(string uriSuffix, IDictionary<string, string> headers) where T : BaseDto
        {
            return Invoke<T>(HttpMethod.Get, uriSuffix, null, headers, default(string));
        }

        public T Invoke<T>(HttpMethod httpMethod, string uriSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers)
            where T : BaseDto
        {
            return Invoke<T>(HttpMethod.Get, uriSuffix, filter, headers, default(string));
        }

        public T Invoke<T>(HttpMethod httpMethod, string uriSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers, string body) 
            where T : BaseDto
        {
            var stringResponse = Invoke(httpMethod, uriSuffix, filter, headers, body);
            return BaseDto.DeserializeObject<T>(stringResponse);
        }

        #endregion Generic Invoke


        #region Invoke

        public string Invoke(string uriSuffix)
        {
            return Invoke(HttpMethod.Get, uriSuffix, null, null, default(string));
        }

        public string Invoke(string uriSuffix, IDictionary<string, string> headers)
        {
            return Invoke(HttpMethod.Get, uriSuffix, null, headers, default(string));
        }

        public string Invoke(string uriSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers)
        {
            return Invoke(HttpMethod.Get, uriSuffix, filter, headers, default(string));
        }

        public string Invoke(HttpMethod httpMethod, string uriSuffix, IDictionary<string, string> headers)
        {
            Contract.Requires(httpMethod != HttpMethod.Put);

            return Invoke(httpMethod, uriSuffix, null, headers, default(string));
        }

        public string Invoke(HttpMethod httpMethod, string uriSuffix, IDictionary<string, string> headers, BaseDto body)
        {
            Contract.Requires(null != body);

            return Invoke(httpMethod, uriSuffix, null, headers, body.SerializeObject());
        }

        public string Invoke(HttpMethod httpMethod, string uriSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers)
        {
            return Invoke(httpMethod, uriSuffix, filter, headers, default(string));
        }

        public string Invoke(HttpMethod httpMethod, string uriSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers, BaseDto body)
        {
            Contract.Requires(null != body);

            return Invoke(httpMethod, uriSuffix, filter, headers, body.SerializeObject());
        }

        public string Invoke(HttpMethod httpMethod, string uriSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers, string body)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(uriSuffix));
            Contract.Requires(Uri.IsWellFormedUriString(uriSuffix, UriKind.Relative), "Invalid relative URI");
            Contract.Requires(IsLoggedIn, "Not logged in, call method login first");

            Debug.WriteLine(string.Format("START calling invoke method({0}, {1}, {2} - {3} - {4}) ...", httpMethod, uriSuffix, filter, headers, body));

            if (null != filter)
            {
                var filterString = UriHelper.CreateFilterString(filter);
                uriSuffix = string.Format("{0}?{1}", uriSuffix, filterString);
            }

            var response = ExecuteRequest(httpMethod, uriSuffix, headers, body);

            Trace.WriteLine(string.Format("END calling invoke method({0}, {1} - {2} - {3}) SUCCEEDED", httpMethod, uriSuffix, headers, body));

            return response;
        }

        #endregion Invoke


        #region Enterprises

        /// <summary>
        /// Retrieve the list of enterprises/tenants
        /// </summary>
        /// <returns>Collection of Enterprises/tenants</returns>
        public abstract Enterprises GetEnterprises();
        
        /// <summary>
        /// Retrieve the enterprise/tenant specified in the authentication information
        /// </summary>
        /// <returns>Enterprise of the specified in the authentication information</returns>
        public abstract Enterprise GetCurrentEnterprise();

        /// <summary>
        /// Retrieve enterprise/tenant by id
        /// </summary>
        /// <param name="id">Id of the enterprise/tenant</param>
        /// <returns>Enterprise</returns>
        public abstract Enterprise GetEnterprise(int id);
        
        #endregion Enterprises


        #region Users

        /// <summary>
        /// Retrieve users with roles of the enterprise/tenant specified in the authentication information
        /// </summary>
        /// <returns>Collection of Users with roles</returns>
        public abstract UsersWithRoles GetUsersWithRolesOfCurrentEnterprise();

        /// <summary>
        /// Retrieve users with roles of a specific enterprise/tenant
        /// </summary>
        /// <param name="enterpriseId">Id of the enterprise/tenant</param>
        /// <returns>Collection of Users with roles</returns>
        public abstract UsersWithRoles GetUsersWithRoles(int enterpriseId);

        /// <summary>
        /// Retrieve a specific user by id of the enterprise/tenant specified in the authentication information
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns>User</returns>
        public abstract User GetUserOfCurrentEnterprise(int id);

        /// <summary>
        /// Retrieve a specific user by id of a specific enterprise/tenant
        /// </summary>
        /// <param name="enterpriseId">Id of the enterprise/tenant</param>
        /// <param name="id">Id of the user</param>
        /// <returns>User</returns>
        public abstract User GetUser(int enterpriseId, int id);

        #endregion Users


        #region Roles

        /// <summary>
        /// Retrieve all roles
        /// </summary>
        /// <returns>Colleciton of Roles</returns>
        public abstract Roles GetRoles();

        /// <summary>
        /// Retrieve a specific role by id
        /// </summary>
        /// <param name="id">Id of the role</param>
        /// <returns>Role</returns>
        public abstract Role GetRole(int id);

        #endregion Roles


        #region VirtualMachines

        /// <summary>
        /// Retrieve all virtual machines across all virtual datacenters the current user has access to
        /// </summary>
        /// <returns>Collection of VirtualMachines</returns>
        public abstract VirtualMachines GetAllVirtualMachines();

        /// <summary>
        /// Retrieve all virtual machines of a specific virtual appliance of a specific virtual datacenter
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance</param>
        /// <returns>Collection of VirtualMachines</returns>
        public abstract VirtualMachines GetVirtualMachines(int virtualDataCenterId, int virtualApplianceId);

        /// <summary>
        /// Retrieve a virtual machine by id of a specific virtual appliance of a specific virtual datacenter
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance</param>
        /// <param name="id">Id of the virtual machine</param>
        /// <returns>VirtualMachine</returns>
        public abstract VirtualMachine GetVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int id);

        /// <summary>
        /// Create a virtual machine based on a virtual machine template
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance to create the virtual machine in</param>
        /// <param name="enterpriseId">Id of the enterprise/tenant the template belongs to</param>
        /// <param name="dataCenterRepositoryId">Id of the datacenter repository the template belongs to</param>
        /// <param name="virtualMachineTemplateId">Id of the virtual machine template</param>
        /// <returns>VirtualMachine</returns>
        public abstract VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId, int virtualMachineTemplateId);

        /// <summary>
        /// Create a virtual machine based on a virtual machine template
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance to create the virtual machine in</param>
        /// <param name="virtualMachineTemplateHref">Href of the virtual machine template the template belongs to</param>
        /// <returns>VirtualMachine</returns>
        public abstract VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref);

        /// <summary>
        /// Create a virtual machine based on a virtual machine template and custom configuration
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance to create the virtual machine in</param>
        /// <param name="enterpriseId">Id of the enterprise/tenant the template belongs to</param>
        /// <param name="dataCenterRepositoryId">Id of the datacenter repository the template belongs to</param>
        /// <param name="virtualMachineTemplateId">Id of the virtual machine template</param>
        /// /// <param name="virtualMachine">Virtual machine configuration</param>
        /// <returns>VirtualMachine</returns>
        public abstract VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int enterpriseId, int dataCenterRepositoryId, int virtualMachineTemplateId, VirtualMachineBase virtualMachine);

        /// <summary>
        /// Create a virtual machine based on a virtual machine template and custom configuration
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance to create the virtual machine in</param>
        /// <param name="virtualMachineTemplateHref">Href of the virtual machine template the template belongs to</param>
        /// <param name="virtualMachine">Virtual machine configuration</param>
        /// <returns>VirtualMachine</returns>
        public abstract VirtualMachine CreateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, string virtualMachineTemplateHref, VirtualMachineBase virtualMachine);

        /// <summary>
        /// Initiates deplyoment of a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine gets deployed in</param>
        /// <param name="virtualMachineId">Id of the virtual machine</param>
        /// <param name="force">If true, soft limits of virtual datacenters could be surpassed</param>
        /// <returns>Task containing information about the status of the deployment</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force);

        /// <summary>
        /// Deplyoment of a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine gets deployed in</param>
        /// <param name="virtualMachineId">Id of the virtual machine</param>
        /// <param name="waitForCompletion">Set to true for waiting until task got completed</param>
        /// <param name="force">If true, soft limits of virtual datacenters could be surpassed</param>
        /// <returns>Task containing information about the status of the deployment</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task DeployVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force, bool waitForCompletion);
        
        /// <summary>
        /// Initiates update of a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine belongs to</param>
        /// <param name="virtualMachineId">Id of the virtual machine</param>
        /// <param name="virtualMachine">Virtual machine configuration</param>
        /// <returns>Task containing information about the status of the update</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, VirtualMachine virtualMachine);

        /// <summary>
        /// Update a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine belongs to</param>
        /// <param name="virtualMachineId">Id of the virtual machine</param>
        /// <param name="virtualMachine">Virtual machine configuration</param>
        /// <param name="waitForCompletion">Set to true for waiting until task got completed</param>
        /// <returns>Task containing information about the status of the update</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task UpdateVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, VirtualMachine virtualMachine, bool waitForCompletion);

        /// <summary>
        /// Initiates state change of a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine belongs to</param>
        /// <param name="virtualMachineId">Id of the virtual machine</param>
        /// <param name="state">Target state</param>
        /// <returns>Task containing information about the status of the state change</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, VirtualMachineState state);
        
        /// <summary>
        /// Changes state of a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine belongs to</param>
        /// <param name="virtualMachineId">Id of the virtual machine</param>
        /// <param name="state">Target state</param>
        /// <param name="waitForCompletion">Set to true for waiting until task got completed</param>
        /// <returns>Task containing information about the status of the state change</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task ChangeStateOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, VirtualMachineState state, bool waitForCompletion);

        /// <summary>
        /// Delete a virtual machine by Id
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine belongs to</param>
        /// <param name="virtualMachineId">Id of the virtual machine</param>
        /// <param name="force">Indicates if deletion has to be forced</param>
        /// <returns>True, if the virtual machine was deleted successfully</returns>
        public abstract bool DeleteVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, bool force = false);

        /// <summary>
        /// Retrieve tasks of a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine belongs to</param>
        /// <param name="virtualMachineId">Id of the virtual mahcine</param>
        /// <returns>Collection of Tasks</returns>
        public abstract Tasks GetAllTasksOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId);

        /// <summary>
        /// Retrieve a task by Id of a specific virtual machine
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter the virtual appliance belongs to</param>
        /// <param name="virtualApplianceId">Id of the virtual appliance the virtual machine belongs to</param>
        /// <param name="virtualMachineId">Id of the virtual mahcine</param>
        /// <param name="taskId">Id of the task</param>
        /// <returns>Task</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task GetTaskOfVirtualMachine(int virtualDataCenterId, int virtualApplianceId, int virtualMachineId, string taskId);

        #endregion VirtualMachines


        #region VirtualMachineTemplates

        /// <summary>
        /// Get all virtual machine templaes of a specific specific datacenter repository of a specific enterprise/tenant
        /// </summary>
        /// <param name="enterpriseId">Id of the entperise/tenant</param>
        /// <param name="dataCenterRepositoryId">Id of the datacenter repository</param>
        /// <returns>Collection of VirtualMachineTemplates</returns>
        public abstract VirtualMachineTemplates GetVirtualMachineTemplates(int enterpriseId, int dataCenterRepositoryId);
        
        /// <summary>
        /// Get a virtual machine template by id of a specific specific datacenter repository of a specific enterprise/tenant
        /// </summary>
        /// <param name="enterpriseId">Id of the enterprise/tenant</param>
        /// <param name="dataCenterRepositoryId">Id of the datacenter repository</param>
        /// <param name="id">Id of the virtual machine template</param>
        /// <returns>VirtualMachineTemplate</returns>
        public abstract VirtualMachineTemplate GetVirtualMachineTemplate(int enterpriseId, int dataCenterRepositoryId, int id);

        #endregion VirtualMachineTemplates


        #region VirtualDataCenters

        /// <summary>
        /// Retrieve all available virtual datacenters
        /// </summary>
        /// <returns>Collection of VirtualDataCenters</returns>
        public abstract VirtualDataCenters GetVirtualDataCenters();

        /// <summary>
        /// Retrieve a specific virtual datacenter by id
        /// </summary>
        /// <param name="id">Id of the virtual datacenter</param>
        /// <returns>VirtualDataCenter</returns>
        public abstract VirtualDataCenter GetVirtualDataCenter(int id);

        #endregion VirtualDataCenters


        #region VirtualAppliances

        /// <summary>
        /// Retrieve all available virtual applicance of a specific virtual datacenter
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter</param>
        /// <returns>Collection of VirtualApplicances</returns>
        public abstract VirtualAppliances GetVirtualAppliances(int virtualDataCenterId);

        /// <summary>
        /// Retrieve a specific virtual appliance by id of a specific virtual datacenter
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter</param>
        /// <param name="id">Id of the virtual appliance</param>
        /// <returns>VirtualAppliance</returns>
        public abstract VirtualAppliance GetVirtualAppliance(int virtualDataCenterId, int id);

        #endregion VirtualAppliances


        #region DataCenterRepositories
        
        /// <summary>
        /// Retrieve all datacenter repositories of the current enterprise/tenant
        /// </summary>
        /// <returns>DataCenterRepositories</returns>
        public abstract DataCenterRepositories GetDataCenterRepositoriesOfCurrentEnterprise();

        /// <summary>
        /// Retrieve all datacenter repositories of an enterprise/tenant
        /// </summary>
        /// <param name="enterpriseId">Id of the enterprise/tenant</param>
        /// <returns>DataCenterRepositories</returns>
        public abstract DataCenterRepositories GetDataCenterRepositories(int enterpriseId);

        /// <summary>
        /// Retrieve a specific datacenter repository by id of the current enterprise/tenant
        /// </summary>
        /// <param name="id">Id of the datacenter repository</param>
        /// <returns>DataCenterRepository</returns>
        public abstract DataCenterRepository GetDataCenterRepositoryOfCurrentEnterprise(int id);

        /// <summary>
        /// Retrieve a specific datacenter repository by id of a specific enterprise/tenant
        /// </summary>
        /// <param name="enterpriseId">Id of the enterprise/tenant</param>
        /// <param name="id">Id of the datacenter repository</param>
        /// <returns>DataCenterRepository</returns>
        public abstract DataCenterRepository GetDataCenterRepository(int enterpriseId, int id);

        #endregion DataCenterRepositories


        #region Tasks

        /// <summary>
        /// Wait for a task to complete
        /// </summary>
        /// <param name="task">Task object</param>
        /// <param name="basePollingWaitTimeMilliseconds">Polling wait time in milliseconds</param>
        /// <param name="timeoutMilliseconds">timoeut in milliseconds</param>
        /// <returns>Completed Task</returns>
        public abstract biz.dfch.CS.Abiquo.Client.v1.Model.Task WaitForTaskCompletion(biz.dfch.CS.Abiquo.Client.v1.Model.Task task, int basePollingWaitTimeMilliseconds, int timeoutMilliseconds);
        
        #endregion Tasks
    }
}
