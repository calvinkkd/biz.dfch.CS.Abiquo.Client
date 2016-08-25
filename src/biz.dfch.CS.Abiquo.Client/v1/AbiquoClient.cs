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
            var uriSuffix = string.Format(AbiquoUriSuffixes.USER_BY_ENTERPRISE_AND_USER_ID, enterpriseId, id);
            
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

        public override VirtualMachines GetVirtualMachines()
        {
            var headers = new HeaderBuilder().BuildAccept(AbiquoMediaDataTypes.VND_ABIQUO_VIRTUALMACHINES).GetHeaders();

            return Invoke<VirtualMachines>(AbiquoUriSuffixes.VIRTUALMACHINES, headers);
        }

        #endregion VirtualMachines


        #region Virtual Data Centers

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

        #endregion Virtual Data Centers
    }
}
