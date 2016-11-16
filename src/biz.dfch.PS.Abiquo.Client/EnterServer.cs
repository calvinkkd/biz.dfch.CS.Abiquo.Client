﻿/**
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
using System.Management.Automation;
using biz.dfch.CS.Abiquo.Client.Authentication;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// 
    /// </summary>
    [Cmdlet(
        VerbsCommon.Enter, "Server"
        , 
        ConfirmImpact = ConfirmImpact.Medium
        , 
        DefaultParameterSetName = ParameterSets.Credential
        , 
        SupportsShouldProcess = true
        , 
        HelpUri = "http://dfch.biz/biz/dfch/PS/Abiquo/Client/Enter-Server/"
    )]
    [OutputType( typeof(string))]
    public class EnterServer : PSCmdlet
    {
        /// <summary>
        /// Defines all valid parameter sets for this cmdlet
        /// </summary>
        public static class ParameterSets
        {
            public const string Credential = "cred";
            public const string Plain = "plain";
            public const string OAuth2 = "oauth2";
        }
            
        /// <summary>
        /// Specifies the base url of the Abiquo endpoint
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        [Alias("ConnectionUri")]
        public Uri Uri { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSets.Plain)]
        public string Username { get; set; }

        [Parameter(Mandatory = true, Position = 2, ParameterSetName = ParameterSets.Plain)]
        public string Password { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSets.Credential)]
        [Alias("cred")]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = true, Position = 3, ParameterSetName = ParameterSets.Plain)]
        [Parameter(Mandatory = true, Position = 2, ParameterSetName = ParameterSets.Credential)]
        [Alias("tid")]
        [ValidateRange(1, int.MaxValue)]
        public int TenantId { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSets.OAuth2)]
        [Alias("token")]
        public string OAuth2Token { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            System.Diagnostics.Trace.TraceInformation("BeginProcessing. ParameterSetName '{0}'.", ParameterSetName);
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            //if (!ShouldProcess(ParameterSetName))
            //{
            //    return;
            //}

            var client = ModuleConfiguration.Current.Client;
            var authInfo = new BasicAuthenticationInformation(Username, Password, TenantId);
            var result = client.Login(Uri.AbsoluteUri, authInfo);

            WriteObject(result);

            System.Diagnostics.Trace.TraceInformation("ProcessRecord. ParameterSetName '{0}'.", ParameterSetName);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            System.Diagnostics.Trace.TraceInformation("EndProcessing. ParameterSetName '{0}'.", ParameterSetName);
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();

            System.Diagnostics.Trace.TraceInformation("StopProcessing. ParameterSetName '{0}'.", ParameterSetName);
        }
    }
}
