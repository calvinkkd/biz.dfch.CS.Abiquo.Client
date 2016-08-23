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

        public bool IsLoggedIn { get; protected set; }

        public string AbiquoApiBaseUrl { get; protected set; }

        public IAuthenticationInformation AuthenticationInformation { get; protected set; }


        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(AbiquoApiVersion));
        }


        public abstract bool Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation);

        public void Logout()
        {
            Debug.WriteLine(string.Format("START {0}", Method.fn()));

            IsLoggedIn = false;
            AbiquoApiBaseUrl = null;
            AuthenticationInformation = null;

            Trace.WriteLine(string.Format("END {0}", Method.fn()));
        }

        #region ExecuteRequest

        internal string ExecuteRequest(string urlSuffix)
        {
            return ExecuteRequest(HttpMethod.Get, urlSuffix, null, null);
        }

        internal string ExecuteRequest(HttpMethod httpMethod, string urlSuffix, IDictionary<string, string> headers, string body)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(urlSuffix));
            Contract.Requires(!string.IsNullOrWhiteSpace(AbiquoApiBaseUrl));
            Contract.Requires(null != AuthenticationInformation);

            var requestUrl = UrlHelper.ConcatUrl(AbiquoApiBaseUrl, urlSuffix);
            Debug.WriteLine(string.Format("START Executing request '{0} {1} - {2} - {3}' ...", httpMethod, requestUrl, headers, body));

            var requestHeaders = new Dictionary<string, string>(AuthenticationInformation.GetAuthorizationHeaders());
            if (null != headers)
            {
                headers.ToList().ForEach(header => requestHeaders[header.Key] = header.Value);
            }

            var restCallExecutor = new RestCallExecutor();
            var result = restCallExecutor.Invoke(httpMethod, requestUrl, requestHeaders, body);

            Trace.WriteLine(string.Format("END Executing request '{0} {1}' SUCCEEDED", httpMethod, requestUrl));
            return result;
        }

        #endregion ExecuteRequest

        #region Invoke

        public string Invoke(string urlSuffix)
        {
            return Invoke(HttpMethod.Get, urlSuffix, null, null, default(string));
        }

        public string Invoke(string urlSuffix, IDictionary<string, object> filter)
        {
            return Invoke(HttpMethod.Get, urlSuffix, filter, null, default(string));
        }

        public string Invoke(HttpMethod httpMethod, string urlSuffix)
        {
            Contract.Requires(httpMethod != HttpMethod.Put);

            return Invoke(httpMethod, urlSuffix, null, null, default(string));
        }

        public string Invoke(HttpMethod httpMethod, string urlSuffix, BaseDto body)
        {
            Contract.Requires(null != body);

            return Invoke(httpMethod, urlSuffix, null, null, body.SerializeObject());
        }

        public string Invoke(HttpMethod httpMethod, string urlSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers)
        {
            return Invoke(httpMethod, urlSuffix, filter, headers, default(string));
        }

        public string Invoke(HttpMethod httpMethod, string urlSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers, BaseDto body)
        {
            Contract.Requires(null != body);

            return Invoke(httpMethod, urlSuffix, filter, headers, body.SerializeObject());
        }

        public string Invoke(HttpMethod httpMethod, string urlSuffix, IDictionary<string, object> filter, IDictionary<string, string> headers, string body)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(urlSuffix));
            Contract.Requires(Uri.IsWellFormedUriString(urlSuffix, UriKind.Relative), "Invalid relative url");
            Contract.Requires(IsLoggedIn, "Not logged in, call method login first");

            Debug.WriteLine(string.Format("START calling invoke method({0}, {1}, {2} - {3} - {4}) ...", httpMethod, urlSuffix, filter, headers, body));

            if (null != filter)
            {
                var filterString = UrlHelper.CreateFilterString(filter);
                urlSuffix = string.Format("{0}?{1}", urlSuffix, filterString);
            }

            var response = ExecuteRequest(httpMethod, urlSuffix, headers, body);

            Trace.WriteLine(string.Format("END calling invoke method({0}, {1} - {2} - {3}) SUCCEEDED", httpMethod, urlSuffix, headers, body));

            return response;
        }

        #endregion Invoke
    }
}
