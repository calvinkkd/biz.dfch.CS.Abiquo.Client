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
    public class OAuth2AuthenticationInformation : IAuthenticationInformation
    {
        private readonly string _oAuth2Token;
        private int _tenantId;

        public OAuth2AuthenticationInformation(string oAuth2Token, int tenantId)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(oAuth2Token));
            Contract.Requires(0 < tenantId);

            _oAuth2Token = oAuth2Token;
            _tenantId = tenantId;
        }

        public IDictionary<string, string> GetAuthorizationHeaders()
        {
            var headerValue = string.Format(Constants.BEARER_AUTHORIZATION_HEADER_VALUE_TEMPLATE, _oAuth2Token);

            var headers = new Dictionary<string, string>
            {
                {Constants.AUTHORIZATION_HEADER_KEY, headerValue}
            };

            return headers;
        }

        public int GetTenantId()
        {
            return _tenantId;
        }
    }
}
