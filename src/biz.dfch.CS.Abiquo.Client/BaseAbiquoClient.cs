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

namespace biz.dfch.CS.Abiquo.Client
{
    [ContractClass(typeof(ContractClassForBaseAbiquoClient))]
    public abstract class BaseAbiquoClient
    {
        public bool IsLoggedIn { get; protected set; }

        public string AbiquoApiBaseUrl { get; protected set; }

        public IAuthenticationInformation AuthenticationInformation { get; protected set; }

        public abstract LoginResultEnum Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation);

        public void Logout()
        {
            if (IsLoggedIn)
            {
                Trace.WriteLine(string.Format("START {0}", Method.fn()));

                this.IsLoggedIn = false;
                this.AbiquoApiBaseUrl = null;
                this.AuthenticationInformation = null;

                Trace.WriteLine(string.Format("END {0}", Method.fn()));
            }
        }

        internal void ExecuteRequest(string urlSuffix)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(urlSuffix));

            // DFTODO - implement logging
            var restCallExecutor = new RestCallExecutor();

            var requestUri = CreateRequestUri(urlSuffix);
            // DFTODO - set wait time millis, etc
            // DFTODO - implement retry
            // DFTODO - honour result
            restCallExecutor.Invoke(HttpMethod.Get, requestUri, AuthenticationInformation.GetAuthorizationHeaders(), null);
        }

        private string CreateRequestUri(string urlSuffix)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(urlSuffix));

            return string.Format("{0}/{1}", this.AbiquoApiBaseUrl.TrimEnd('/'), urlSuffix.TrimStart('/'));
        }
    }
}
