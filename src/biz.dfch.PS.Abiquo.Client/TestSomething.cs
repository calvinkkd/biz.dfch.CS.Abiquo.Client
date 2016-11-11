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
using System.Management.Automation;

namespace biz.dfch.PS.Abiquo.Client
{
    [Cmdlet(VerbsDiagnostic.Test, "Something")]
    [Alias("Test-Something")]
    [OutputType( typeof(string))]
    //[OutputType( typeof(double), ParameterSetName = new string[] { } )]
    //[OutputType( typeof(bool), ParameterSetName = new[] { "set1" } )]
    //[OutputType( typeof(object), ParameterSetName = new[] { "set2" , "set3"} )]
    public class TestSomething : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public Uri BaseUrl { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "plain")]
        public string Username { get; set; }

        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "plain")]
        public string Password { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "cred")]
        [Alias("cred")]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "oauth2")]
        public string OAuth2Token { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            WriteDebug("myWriteDebug");

            WriteVerbose("myWriteVerbose");

            var informationRecord = new InformationRecord("myInformationRecord", "mySource");
            WriteInformation(informationRecord);

            WriteWarning("myWriteWarning");

            var myPSCmdlet = this;

            //var errorRecord = new ErrorRecord(new Exception("myExceptionMessage"), "myErrorId", ErrorCategory.WriteError, this);
            //WriteError(errorRecord);

            System.Diagnostics.Trace.TraceInformation("BeginProcessing. Username '{0}'. Password '{1}'", Username, Password);
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (!ShouldProcess("myTarget"))
            {
                return;
            }

            if (!ShouldContinue("queryToContinue", "captionToContinue"))
            {
                return;
            }

            WriteObject("tralala");

            System.Diagnostics.Trace.TraceInformation("ProcessRecord. Username '{0}'. Password '{1}'", Username, Password);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            System.Diagnostics.Trace.TraceInformation("EndProcessing. Username '{0}'. Password '{1}'", Username, Password);
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();

            System.Diagnostics.Trace.TraceInformation("StopProcessing. Username '{0}'. Password '{1}'", Username, Password);
        }
    }
}
