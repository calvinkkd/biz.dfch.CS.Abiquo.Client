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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Abiquo.Client.v1.Model
{
    public class Enterprise : AbiquoBaseDto
    {
        public long RamSoftLimitInMb { get; set; }
        public long RamHardLimitInMb { get; set; }
        public long CpuCountSoftLimit { get; set; }
        public long CpuCountHardLimit { get; set; }
        public long DiskSoftLimitInMb { get; set; }
        public long DiskHardLimitInMb { get; set; }
        public long StorageSoftInMb { get; set; }
        public long StorageHardInMb { get; set; }
        public long VlansSoft { get; set; }
        public long VlansHard { get; set; }
        public long PublicIpsSoft { get; set; }
        public long PublicIpsHard { get; set; }
        public long RepositorySoftInMb { get; set; }
        public long RepositoryHardInMb { get; set; }
        public bool IsReservationRestricted { get; set; }
        public bool Workflow { get; set; }
        public bool TwoFactorAuthenticationMandatory { get; set; }
    }
}
