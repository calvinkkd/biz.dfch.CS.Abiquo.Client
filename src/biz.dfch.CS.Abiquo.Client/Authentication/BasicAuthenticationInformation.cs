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

namespace biz.dfch.CS.Abiquo.Client.Authentication
{
    public class BasicAuthenticationInformation : IAuthenticationInformation
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _tenantId;

        public const string BASIC_AUTHORIZATION_HEADER_VALUE_TEMPLATE = "Basic {0}";

        public BasicAuthenticationInformation(string username, string password, string tenantId)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password));
            Contract.Requires(!string.IsNullOrWhiteSpace(tenantId));

            this._username = username;
            this._password = password;
            this._tenantId = tenantId;
        }

        public IDictionary<string, object> GetAuthorizationHeaders()
        {
            var headers = new Dictionary<string, object>
            {
                {Constants.AUTHORIZATION_HEADER_KEY, CreateBasicAuthorizationHeaderValue()}
            };

            return headers;
        }

        private string CreateBasicAuthorizationHeaderValue()
        {
            var plainText = string.Format("{0}:{1}", _username, _password);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            var base64EncodedAuthorizationHeaderValue = Convert.ToBase64String(plainTextBytes);

            return string.Format(BASIC_AUTHORIZATION_HEADER_VALUE_TEMPLATE, base64EncodedAuthorizationHeaderValue);
        }

        public string GetTenantId()
        {
            return this._tenantId;
        }
    }
}
