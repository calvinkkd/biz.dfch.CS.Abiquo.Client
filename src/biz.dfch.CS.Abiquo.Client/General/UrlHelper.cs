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

namespace biz.dfch.CS.Abiquo.Client.General
{
    public class UrlHelper
    {
        public const char CHARACTER_TO_TRIM_ON = '/';
        public const string FILTER_SEPARATOR = "&";

        public static string ConcatUrl(string baseUrl, string urlSuffix)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(baseUrl));
            Contract.Requires(!string.IsNullOrWhiteSpace(urlSuffix));

            var resultingUrl = string.Format("{0}/{1}", baseUrl.TrimEnd(CHARACTER_TO_TRIM_ON), urlSuffix.TrimStart(CHARACTER_TO_TRIM_ON).TrimEnd(CHARACTER_TO_TRIM_ON));

            return resultingUrl;
        }

        public static string CreateFilterString(IDictionary<string, object> filter)
        {
            Contract.Requires(null != filter);
            Contract.Requires(filter.Count > 0);

            var filterString = string.Empty;
            var separator = string.Empty;

            foreach (var parameter in filter)
            {
                filterString += string.Format("{0}{1}={2}", separator, parameter.Key, parameter.Value);
                separator = FILTER_SEPARATOR;
            }

            return filterString;
        }
    }
}
