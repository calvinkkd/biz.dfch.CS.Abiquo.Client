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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Automation;
using biz.dfch.CS.Abiquo.Client.v1.Model;
using biz.dfch.CS.PowerShell.Commons;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// This class defines the GetMachine Cmdlet that retrieves a list of machines
    /// </summary>
    [Cmdlet(
         VerbsCommon.Get, "Machine"
         ,
         ConfirmImpact = ConfirmImpact.Low
         ,
         DefaultParameterSetName = ParameterSets.LIST
         ,
         SupportsShouldProcess = true
         ,
         HelpUri = "http://dfch.biz/biz/dfch/PS/Abiquo/Client/Get-Machine/"
    )]
    [OutputType(typeof(ICollection<VirtualMachine>), ParameterSetName = new [] { ParameterSets.LIST, ParameterSets.NAME })]
    [OutputType(typeof(VirtualMachine), ParameterSetName = new [] { ParameterSets.ID })]
    public class GetMachine : PsCmdletBase
    {
        /// <summary>
        /// Defines all valid parameter sets for this cmdlet
        /// </summary>
        public static class ParameterSets
        {
            /// <summary>
            /// ParameterSetName used when specifying a credential object
            /// </summary>
            public const string LIST = "ListAvailable";

            /// <summary>
            /// ParameterSetName used when specifying a single machine by its id
            /// </summary>
            public const string ID = "Id";

            /// <summary>
            /// ParameterSetName used when specifying an OAuth2 token
            /// </summary>
            public const string NAME = "name";
        }

        /// <summary>
        /// Specifies the machine id
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSets.ID)]
        public long Id { get; set; }

        /// <summary>
        /// Specifies the name name
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSets.NAME)]
        public string Name { get; set; }

        /// <summary>
        /// Retrieve all machines for the current enterprise
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = ParameterSets.LIST)]
        public SwitchParameter ListAvailable { get; set; }

        /// <summary>
        /// ProcessRecord
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Contract.Assert(null != ModuleConfiguration.Current.Client);
            Contract.Assert(ModuleConfiguration.Current.Client.IsLoggedIn);

            var shouldProcessMessage = string.Format(Messages.GetMachineShouldProcess, ParameterSetName);
            if (!ShouldProcess(shouldProcessMessage))
            {
                return;
            }

            switch(ParameterSetName)
            {
                case ParameterSets.LIST:
                {
                    ProcessParameterSetList();
                    return;
                }

                case ParameterSets.ID:
                {
                    ProcessParameterSetId();
                    return;
                }

                case ParameterSets.NAME:
                {
                    ProcessParameterSetName();
                    return;
                }

                default:
                    const bool isValidParameterSetName = false;
                    Contract.Assert(isValidParameterSetName, ParameterSetName);
                    break;
            }

        }

        private void ProcessParameterSetId()
        {
            var virtualMachines = ModuleConfiguration.Current.Client.GetAllVirtualMachines();

            var virtualMachine = virtualMachines.Collection.FirstOrDefault(e => e.Id.HasValue && e.Id.Value == Id);
            
            var virtualMachineFound = null != virtualMachine;
            Contract.Assert(virtualMachineFound, Id.ToString());

            WriteObject(virtualMachine);
        }

        private void ProcessParameterSetName()
        {
            var virtualMachines = ModuleConfiguration.Current.Client
                .GetAllVirtualMachines()
                .Collection
                .Where(e => Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            var virtualMachineFound = 0 != virtualMachines.Count;
            Contract.Assert(virtualMachineFound, Name);

            if (1 == virtualMachines.Count)
            {
                WriteObject(virtualMachines.First());
            }
            else
            {
                WriteObject(virtualMachines);
            }
        }

        private void ProcessParameterSetList()
        {
            var virtualMachines = ModuleConfiguration.Current.Client
                .GetAllVirtualMachines()
                .Collection
                .Where(e => Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            WriteObject(virtualMachines);
        }
    }
}
