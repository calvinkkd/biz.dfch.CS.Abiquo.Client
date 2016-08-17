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

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("biz.dfch.CS.Abiquo.Client.Tests")]
namespace biz.dfch.CS.Abiquo.Client
{
    [ContractClass(typeof(ContractClassForBaseAbiquoClient))]
    public abstract class BaseAbiquoClient
    {
        public string Version { get; protected set; }

        public bool IsLoggedIn { get; protected set; }

        public string AbiquoApiBaseUrl { get; protected set; }

        public IAuthenticationInformation AuthenticationInformation { get; protected set; }


        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(Version));
        }


        public abstract bool Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation);

        public void Logout()
        {
            Debug.WriteLine(string.Format("START {0}", Method.fn()));

            this.IsLoggedIn = false;
            this.AbiquoApiBaseUrl = null;
            this.AuthenticationInformation = null;

            Trace.WriteLine(string.Format("END {0}", Method.fn()));
        }

        internal string ExecuteRequest(HttpMethod httpMethod, string urlSuffix)
        {
            Contract.Requires(httpMethod != HttpMethod.Post);
            Contract.Requires(httpMethod != HttpMethod.Put);

            return ExecuteRequest(httpMethod, urlSuffix, null, null);
        }

        internal string ExecuteRequest(HttpMethod httpMethod, string urlSuffix, string body)
        {
            return ExecuteRequest(httpMethod, urlSuffix, null, body);
        }

        internal string ExecuteRequest(HttpMethod httpMethod, string urlSuffix, IDictionary<string, string> headers, string body)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(urlSuffix));
            Contract.Requires(!string.IsNullOrWhiteSpace(this.AbiquoApiBaseUrl));
            Contract.Requires(null != this.AuthenticationInformation);

            var requestUrl = UrlHelper.ConcatUrl(this.AbiquoApiBaseUrl, urlSuffix);
            Debug.WriteLine(string.Format("START Executing request '{0} {1}' ...", httpMethod, requestUrl));

            var requestHeaders = new Dictionary<string, string>(AuthenticationInformation.GetAuthorizationHeaders());
            if (null != headers)
            {
                headers.ToList().ForEach(header => requestHeaders[header.Key] = header.Value);
            }

            var restCallExecutor = new RestCallExecutor();
            var result = restCallExecutor.Invoke(HttpMethod.Get, requestUrl, requestHeaders, body);

            Trace.WriteLine(string.Format("END Executing request '{0} {1}' SUCCEEDED", httpMethod, requestUrl));
            return result;
        }
    }
}
