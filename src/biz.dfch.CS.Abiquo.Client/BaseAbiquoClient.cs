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
using biz.dfch.CS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Abiquo.Client
{
    [ContractClass(typeof(ContractClassForBaseAbiquoClient))]
    public abstract class BaseAbiquoClient
    {
        public bool IsLoggedIn { get; protected set; }

        public string AbiquoApiBaseUrl { get; set; }

        public IAuthenticationInformation AuthenticationInformation { get; set; }

        public abstract LoginResult Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation);

        /// <summary>
        /// Resets all connection information
        /// </summary>
        public void Logout()
        {
            this.IsLoggedIn = false;

            this.AuthenticationInformation = null;

            Trace.WriteLine("Logout (Clear/Reset authentication information)");
        }
    }
}
