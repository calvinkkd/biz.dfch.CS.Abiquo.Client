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
using biz.dfch.CS.Abiquo.Client.Authentication;

namespace biz.dfch.CS.Abiquo.Client.Tests
{
    internal class IntegrationTestEnvironment
    {
        public const string ABIQUO_API_BASE_URI = @"https://abiquo.example.com/api/";
        public const string ABIQUO_USERNAME = "admin";
        public const string ABIQUO_PASSWORD = "xabiquo";
        public const int  ABIQUO_TENANT_ID = 1;
        
        static IntegrationTestEnvironment()
        {
            AbiquoApiBaseUri = Environment.GetEnvironmentVariable("ABIQUO_API_BASE_URI");
            if (string.IsNullOrWhiteSpace(AbiquoApiBaseUri))
            {
                AbiquoApiBaseUri = ABIQUO_API_BASE_URI;
            }

            Username = Environment.GetEnvironmentVariable("ABIQUO_USERNAME");
            if (string.IsNullOrWhiteSpace(Username))
            {
                Username = ABIQUO_USERNAME;
            }

            Password = Environment.GetEnvironmentVariable("ABIQUO_PASSWORD");
            if (string.IsNullOrWhiteSpace(Password))
            {
                Password = ABIQUO_PASSWORD;
            }

            int tenantId;
            var isDefined = int.TryParse(Environment.GetEnvironmentVariable("ABIQUO_TENANT_ID"), out tenantId);
            if (!isDefined)
            {
                TenantId = ABIQUO_TENANT_ID;
            }

            AuthenticationInformation = new BasicAuthenticationInformation(Username, Password, TenantId);
        }

        public static string AbiquoApiBaseUri { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static int TenantId { get; set; }

        public static IAuthenticationInformation AuthenticationInformation { get; set; }
    }
}
