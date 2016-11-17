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

using System;
using System.Diagnostics.Contracts;
using biz.dfch.CS.Abiquo.Client;
using biz.dfch.CS.Abiquo.Client.Factory;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleContext
    {
        private static string _apiVersion = AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1;

        /// <summary>
        /// Specifies the API version to use
        /// </summary>
        public static string ApiVersion
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(_apiVersion));

                return _apiVersion;
            }
            set
            {
                Contract.Requires(!string.IsNullOrWhiteSpace(value));
                
                _apiVersion = value;
            }
        }

        private static readonly Lazy<BaseAbiquoClient> _client = new Lazy<BaseAbiquoClient>(() =>
        {
            Contract.Ensures(null != Contract.Result<BaseAbiquoClient>());
            
            var client = AbiquoClientFactory.GetByVersion(ApiVersion);
            return client;
        });

        /// <summary>
        /// Returns a reference to the underlying Abiquo client
        /// </summary>
        public BaseAbiquoClient Client
        {
            get
            {
                Contract.Ensures(null != Contract.Result<BaseAbiquoClient>());
                
                return _client.Value;
            }
        }
    }
}
