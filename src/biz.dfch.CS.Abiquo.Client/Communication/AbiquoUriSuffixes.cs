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
        public const string ENTERPRISES = "/admin/enterprises";
        public const string ENTERPRISE_BY_ID = "/admin/enterprises/{0}";
        public const string USERSWITHROLES_BY_ENTERPRISE_ID = "/admin/enterprises/{0}/users";
        public const string USER_BY_ENTERPRISE_AND_USER_ID = "/admin/enterprises/{0}/users/{1}";
        public const string ROLES = "/admin/roles";
        public const string ROLE_BY_ID = "/admin/roles/{0}";
    }
}
