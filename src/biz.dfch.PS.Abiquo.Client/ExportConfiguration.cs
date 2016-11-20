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
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Management.Automation;
using System.Threading;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// This class defines the ExportConfiguration Cmdlet that imports the configuration of the module
    /// </summary>
    [Cmdlet(
         VerbsData.Export, "Configuration"
         ,
         ConfirmImpact = ConfirmImpact.High
         ,
         SupportsShouldProcess = true
         ,
         HelpUri = "http://dfch.biz/biz/dfch/PS/Abiquo/Client/Export-Configuration/"
    )]
    public class ExportConfiguration : PsCmdletBase
    {
        /// <summary>
        /// Specifies the configuration file name to be exported. If no value is specified the default configuration file name will be used.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public FileInfo Path { get; set; }

        /// <summary>
        /// Specifies whether to overwrite an existing configuration file
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Main processing logic.
        /// </summary>
        protected override void EndProcessing()
        {
            base.EndProcessing();

            if (null == Path)
            {
                Path = ModuleConfiguration.ResolveConfigurationFileInfo(Path);

                if (Force && !ShouldProcess(string.Format(Messages.ExportConfigurationShouldProcessForce, Path.FullName)))
                {
                    return;
                }
            }
            else
            {
                if (Force && File.Exists(Path.FullName) && !ShouldProcess(string.Format(Messages.ExportConfigurationShouldProcessForce, Path.FullName)))
                {
                    return;
                }
            }

            if (!ShouldProcess(string.Format(Messages.ExportConfigurationShouldProcess, Path.FullName)))
            {
                return;
            }

            var moduleContextFromVariable = this.GetVariableValue(ModuleConfiguration.MODULE_VARIABLE_NAME) as ModuleContext;
            Contract.Assert(null != moduleContextFromVariable, Messages.ExportConfigurationModuleVariableNotFound);

            var moduleContextSection = ModuleConfiguration.ConvertToModuleContextSection();
            ModuleConfiguration.Save(Path, moduleContextSection);
        }
    }
}
