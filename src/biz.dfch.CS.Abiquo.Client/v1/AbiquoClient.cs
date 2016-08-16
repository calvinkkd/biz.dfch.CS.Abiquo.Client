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
 
﻿using biz.dfch.CS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using biz.dfch.CS.Abiquo.Client.Authentication;
﻿using biz.dfch.CS.Web.Utilities.Rest;

namespace biz.dfch.CS.Abiquo.Client.v1
{
    public class AbiquoClient : BaseAbiquoClient
    {
        public override LoginResultEnum Login(string abiquoApiBaseUrl, IAuthenticationInformation authenticationInformation)
        {
            Trace.WriteLine(string.Format("START Login (AbiquoApiBaseUrl = '{0}'; TenantId = '{1}'; ) ...", abiquoApiBaseUrl, authenticationInformation.GetTenantId()));

            Logout();

            this.AuthenticationInformation = authenticationInformation;
            this.AbiquoApiBaseUrl = abiquoApiBaseUrl;

            DoMakeRequest(Constants.LOGIN_URL_SUFFIX);

            IsLoggedIn = true;

            Trace.WriteLine("END Login SUCCEEDED");

            return LoginResultEnum.Success;
        }

        private void DoMakeRequest(string urlSuffix)
        {
            var requestUri = CreateRequestUri(urlSuffix);
            
            var restCallExecutor = new RestCallExecutor();

            // DFTODO - set wait time millis, etc
            // DFTODO - implement retry
            // DFTODO - honour result
            restCallExecutor.Invoke(HttpMethod.Get, requestUri, AuthenticationInformation.GetAuthorizationHeaders(), null);
        }

        private string CreateRequestUri(string urlSuffix)
        {
            return string.Format("{0}/{1}", this.AbiquoApiBaseUrl.TrimEnd('/'), urlSuffix.TrimStart('/'));
        }
    }
}
