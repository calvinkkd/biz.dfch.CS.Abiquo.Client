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

namespace biz.dfch.CS.Abiquo.Client.General
{
    public class HeaderBuilder
    {
        private Dictionary<string, string> _headers; 

        public HeaderBuilder()
        {
            _headers = new Dictionary<string, string>();
        }

        public HeaderBuilder BuildAccept(string acceptHeaderValue)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(acceptHeaderValue));

            _headers.Add(Constants.ACCEPT_HEADER_KEY, acceptHeaderValue);
            return this;
        }

        public HeaderBuilder BuildContentType(string contentTypeHeaderValue)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(contentTypeHeaderValue));

            _headers.Add(Constants.CONTENT_TYPE_HEADER_KEY, contentTypeHeaderValue);
            return this;
        }

        public Dictionary<string, string> GetHeaders()
        {
            return _headers;
        }
    }
}
