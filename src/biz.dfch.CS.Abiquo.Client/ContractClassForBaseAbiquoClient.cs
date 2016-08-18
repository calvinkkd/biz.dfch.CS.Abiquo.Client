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

namespace biz.dfch.CS.Abiquo.Client
{
    [ContractClassFor(typeof(BaseAbiquoClient))]
    abstract class ContractClassForBaseAbiquoClient : BaseAbiquoClient
    {
        public override bool Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(abiquoApiBaseUrl));
            Contract.Requires(null != authenticationInformation);
            Contract.Ensures(Contract.Result<bool>() == !string.IsNullOrWhiteSpace(this.AbiquoApiBaseUrl));
            Contract.Ensures(Contract.Result<bool>() == (null != this.AuthenticationInformation));

            return default(bool);
        }
    }
}