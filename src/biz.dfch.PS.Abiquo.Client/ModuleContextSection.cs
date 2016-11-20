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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Abiquo.Client.Factory;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// This class defines the module context and configuration of the module
    /// </summary>
    public class ModuleContextSection : ConfigurationSection
    {
        /// <summary>
        /// The name of the configuration section
        /// </summary>
        public const string SECTION_NAME = "ModuleContext";

        /// <summary>
        /// Specifies the base uri of the Abiquo endpoint
        /// </summary>
        [ConfigurationProperty("uri", DefaultValue = "http://abiquo.example.com/api", IsRequired = false)]
        public Uri Uri
        {
            get { return (Uri) this["uri"]; }
            set { this["uri"] = value; }
        }
        
        /// <summary>
        /// Specifies the username to connect with
        /// </summary>
        [ConfigurationProperty("username", DefaultValue = "admin", IsRequired = false)]
        public string Username
        {
            get { return (string) this["username"]; }
            set { this["username"] = value; }
        }

        /// <summary>
        /// Specifies the password to connect with
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = "xabiquo", IsRequired = false)]
        public string Password
        {
            get { return (string) this["password"]; }
            set { this["password"] = value; }
        }

        /// <summary>
        /// Specifies the oAuth2Token to connect with
        /// </summary>
        [ConfigurationProperty("oAuth2Token", DefaultValue = "", IsRequired = false)]
        public string OAuth2Token
        {
            get { return (string) this["oAuth2Token"]; }
            set { this["oAuth2Token"] = value; }
        }

        /// <summary>
        /// Specifies the authenticationType to use
        /// </summary>
        [ConfigurationProperty("authenticationType", DefaultValue = EnterServer.ParameterSets.PLAIN, IsRequired = false)]
        public string AuthenticationType
        {
            get { return (string) this["authenticationType"]; }
            set { this["authenticationType"] = value; }
        }

        /// <summary>
        /// Specifies the api version of the Abiquo client to use
        /// </summary>
        [ConfigurationProperty("apiVersion", DefaultValue = AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1, IsRequired = false)]
        public string ApiVersion
        {
            get { return (string) this["apiVersion"]; }
            set { this["apiVersion"] = value; }
        }

        /// <summary>
        /// Specifies the source levels used for logging
        /// </summary>
        [ConfigurationProperty("sourceLevels", DefaultValue = SourceLevels.All, IsRequired = false)]
        public SourceLevels SourceLevels
        {
            get { return (SourceLevels) this["sourceLevels"]; }
            set { this["sourceLevels"] = value; }
        }
    }
}
