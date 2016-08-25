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

        public override Roles GetRoles()
        {
            Contract.Ensures(null != Contract.Result<Roles>());

            return default(Roles);
        }

        public override Role GetRole(int id)
        {
            Contract.Requires(id > 0);

            return default(Role);
        }

        public override VirtualMachines GetVirtualMachines()
        {
            Contract.Ensures(null != Contract.Result<VirtualMachines>());

            return default(VirtualMachines);
        }

        public override VirtualDataCenters GetVirtualDataCenters()
        {
            Contract.Ensures(null != Contract.Result<VirtualDataCenters>());

            return default(VirtualDataCenters);
        }

        public override VirtualDataCenter GetVirtualDataCenter(int id)
        {
            Contract.Requires(id > 0);

            return default(VirtualDataCenter);
        }
    }
}
