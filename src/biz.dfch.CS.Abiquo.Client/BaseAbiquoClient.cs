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
﻿using Newtonsoft.Json;
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
        /// Retrieve enterprise/tenant by Id
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
        /// Retrieve a specific user of the enterprise/tenant specified in the authentication information
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns>User</returns>
        public abstract User GetUserOfCurrentEnterprise(int id);

        /// <summary>
        /// Retrieve a specific user of a specific enterprise/tenant
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
        /// Retrieve a specific role
        /// </summary>
        /// <param name="id">Id of the role</param>
        /// <returns>Role</returns>
        public abstract Role GetRole(int id);

        #endregion Roles


        #region VirtualMachines

        /// <summary>
        /// Retrieve all the virtual machines that the current user has access to
        /// </summary>
        /// <returns>Collection of virtual machines</returns>
        public abstract VirtualMachines GetVirtualMachines();



        #endregion VirtualMachines


        #region Virtual Data Centers

        /// <summary>
        /// Retrieve all available virtual datacenters
        /// </summary>
        /// <returns>Collection of VirtualDataCenters</returns>
        public abstract VirtualDataCenters GetVirtualDataCenters();

        /// <summary>
        /// Retrieve a specific virtual datacenter
        /// </summary>
        /// <param name="id">Id of the virtual datacenter</param>
        /// <returns>VirtualDataCenter</returns>
        public abstract VirtualDataCenter GetVirtualDataCenter(int id);

        /// <summary>
        /// Retrieve all available virtual applicance of a specific virtual datacenter
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter</param>
        /// <returns>Collection of VirtualApplicances</returns>
        public abstract VirtualAppliances GetVirtualAppliances(int virtualDataCenterId);

        /// <summary>
        /// Retrieve a specific virtual appliance of a specific virtual datacenter
        /// </summary>
        /// <param name="virtualDataCenterId">Id of the virtual datacenter</param>
        /// <param name="id">Id of the virtual appliance</param>
        /// <returns>VirtualAppliance</returns>
        public abstract VirtualAppliance GetVirtualAppliance(int virtualDataCenterId, int id);

        #endregion Virtual Data Centers
    }
}
