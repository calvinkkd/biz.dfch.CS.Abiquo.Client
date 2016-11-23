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
using System.IO;
using System.Management.Automation;
using biz.dfch.CS.PowerShell.Commons;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// This class defines the ImportConfiguration Cmdlet that imports the configuration of the module
    /// </summary>
    [Cmdlet(
         VerbsData.Import, "Configuration"
         ,
         ConfirmImpact = ConfirmImpact.Medium
         ,
         SupportsShouldProcess = true
         ,
         HelpUri = "http://dfch.biz/biz/dfch/PS/Abiquo/Client/Import-Configuration/"
    )]
    [OutputType(typeof(ModuleContext))]
    public class ImportConfiguration : PsCmdletBase
    {
        /// <summary>
        /// Specifies the configuration file name to be imported. If no value is specified the default configuration file name will be used.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        public FileInfo Path { get; set; }

        /// <summary>
        /// Specifies whether the configuration should be saved to the module configuration variable
        /// </summary>
        [Parameter(Mandatory = false)]
        [Alias("save")]
        [PSDefaultValue(Value = false)]
        public SwitchParameter SaveToModuleVariable { get; set; }

        /// <summary>
        /// Main processing logic.
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Path = ModuleConfiguration.ResolveConfigurationFileInfo(Path);

            var shouldProcessMessage = string.Format(Messages.ImportConfigurationShouldProcess, Path.FullName);
            if (!ShouldProcess(shouldProcessMessage))
            {
                return;
            }

            var moduleContextSection = ModuleConfiguration.GetModuleContextSection(Path);
            ModuleConfiguration.SetModuleContext(moduleContextSection);
            WriteObject(ModuleConfiguration.Current);

            if (!SaveToModuleVariable)
            {
                return;
            }

            this.SessionState.PSVariable.Set(ModuleConfiguration.MODULE_VARIABLE_NAME, ModuleConfiguration.Current);

            var result = this.GetVariableValue(ModuleConfiguration.MODULE_VARIABLE_NAME);
            if (null != result)
            {
                return;
            }

            // ReSharper disable once NotResolvedInText
            var exception = new ArgumentException(string.Format(Messages.ImportConfigurationSaveToModuleVariableFailed), "SaveToModuleVariable");
            var errorRecord = new ErrorRecord(exception, ErrorIdEnum.ImportConfigurationFailed.ToString(), ErrorCategory.InvalidResult, this);
            WriteError(errorRecord);
        }

    }
}
