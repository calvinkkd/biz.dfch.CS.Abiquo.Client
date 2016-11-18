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
using System.IO;
using System.Management.Automation;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// Class for methods that need to be globally reachable
    /// </summary>
    public static class RemoveScriptsToProcessModules
    {
        private static bool _hasBeenInvoked = false;

        private const string REMOVE_IMPORT_MODULE_SCRIPT_FORMAT =
            @"$moduleInfo = Get-Module 'Import-Module' | ? Path -eq '{0}\Import-Module.ps1'; if($moduleInfo) {{ Remove-Module $moduleInfo -Force -ErrorAction:SilentlyContinue; }}";
        
        /// <summary>
        /// This method will remove the module 'Import-Module' that is shown as loaded due to the "ScriptsToProcess" directive in PSD1
        /// </summary>
        /// <param name="cmdlet"></param>
        public static void Invoke(PSCmdlet cmdlet)
        {
            Contract.Requires(null != cmdlet);

            if (_hasBeenInvoked)
            {
                return;
            }

            var location = cmdlet.GetType().Assembly.Location;
            Contract.Assert(!string.IsNullOrWhiteSpace(location));

            var path = Path.GetDirectoryName(location);
            Contract.Assert(Directory.Exists(path));

            var script = string.Format(REMOVE_IMPORT_MODULE_SCRIPT_FORMAT, path);
            cmdlet.InvokeCommand.InvokeScript(script);

            _hasBeenInvoked = true;
        }
    }
}
