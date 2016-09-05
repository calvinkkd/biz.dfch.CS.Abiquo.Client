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

namespace biz.dfch.CS.Abiquo.Client.Communication
{
    public abstract class AbiquoUriSuffixes
    {
        public const string LOGIN = "/login";

        #region Enterprises
        public const string ENTERPRISES = "/admin/enterprises";
        public const string ENTERPRISE_BY_ID = "/admin/enterprises/{0}";
        #endregion Enterprises


        #region Users
        public const string USERSWITHROLES_BY_ENTERPRISE_ID = "/admin/enterprises/{0}/users";
        public const string USER_BY_ENTERPRISE_ID_AND_USER_ID = "/admin/enterprises/{0}/users/{1}";
        #endregion Users


        #region Roles
        public const string ROLES = "/admin/roles";
        public const string ROLE_BY_ID = "/admin/roles/{0}";
        #endregion Roles

        #region VirtualMachines
        public const string VIRTUALMACHINES = "/cloud/virtualmachines";
        public const string VIRTUALMACHINES_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID = "/cloud/virtualdatacenters/{0}/virtualappliances/{1}/virtualmachines";
        public const string VIRTUALMACHINE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID_AND_VIRTUALMACHINE_ID = "/cloud/virtualdatacenters/{0}/virtualappliances/{1}/virtualmachines/{2}";
        public const string CHANGE_VIRTUALMACHINE_STATE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID_AND_VIRTUALMACHINE_ID = "/cloud/virtualdatacenters/{0}/virtualappliances/{1}/virtualmachines/{2}/state";
        public const string DEPLOY_VIRTUALMACHINE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID_AND_VIRTUALMACHINE_ID = "/cloud/virtualdatacenters/{0}/virtualappliances/{1}/virtualmachines/{2}/action/deploy";
        public const string VIRTUALMACHINETASKS_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPPLIANCE_ID_AND_VIRTUALMACHINE_ID = "/cloud/virtualdatacenters/{0}/virtualappliances/{1}/virtualmachines/{2}/tasks";
        public const string VIRTUALMACHINETASK_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPPLIANCE_ID_AND_VIRTUALMACHINE_ID_AND_TASK_ID = "/cloud/virtualdatacenters/{0}/virtualappliances/{1}/virtualmachines/{2}/tasks/{3}";
        #endregion VirtualMachines


        #region VirtualMachineTemplates
        public const string VIRTUALMACHINETEMPLATES_BY_ENTERPISE_ID_AND_DATACENTERREPOSITORY_ID = "/admin/enterprises/{0}/datacenterrepositories/{1}/virtualmachinetemplates";
        public const string VIRTUALMACHINETEMPLATE_BY_ENTERPISE_ID_AND_DATACENTERREPOSITORY_ID_AND_VIRTUALMACHINETEMPLATE_ID = "/admin/enterprises/{0}/datacenterrepositories/{1}/virtualmachinetemplates/{2}";
        #endregion VirtualMachineTemplates


        #region VirtualDataCenters
        public const string VIRTUALDATACENTERS = "/cloud/virtualdatacenters";
        public const string VIRTUALDATACENTER_BY_ID = "/cloud/virtualdatacenters/{0}";
        #endregion VirtualDataCenters


        #region VirtualAppliances
        public const string VIRTUALAPPLIANCES_BY_VIRTUALDATACENTER_ID = "/cloud/virtualdatacenters/{0}/virtualappliances";
        public const string VIRTUALAPPLIANCE_BY_VIRTUALDATACENTER_ID_AND_VIRTUALAPLLIANCE_ID = "/cloud/virtualdatacenters/{0}/virtualappliances/{1}";
        #endregion VirtualAppliances


        #region DateCenterRepositories
        public const string DATACENTERREPOSITORIES_BY_ENTERPRISE_ID = "/admin/enterprises/{0}/datacenterrepositories";
        public const string DATACENTERREPOSITORIES_BY_ENTERPRISE_ID_AND_DATACENTERREPOSITORY_ID = "/admin/enterprises/{0}/datacenterrepositories/{1}";
        #endregion DateCenterRepositories
    }
}
