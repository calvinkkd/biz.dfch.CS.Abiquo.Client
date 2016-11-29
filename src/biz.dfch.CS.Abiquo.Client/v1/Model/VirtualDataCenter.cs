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
 
 using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
 using System.ComponentModel.DataAnnotations;

namespace biz.dfch.CS.Abiquo.Client.v1.Model
{
    public class VirtualDataCenter : AbiquoLinkBaseDto
    {
        [Required]
        [Range(0, Int32.MaxValue)]
        public int CpuCountHardLimit { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        public int CpuCountSoftLimit { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long DiskHardLimitInMb { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long DiskSoftLimitInMb { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public HypervisorTypeEnum HypervisorType { get; set; }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long PublicIpsHard { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long PublicIpsSoft { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        public int RamHardLimitInMb { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        public int RamSoftLimitInMb { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long StorageHardInMb { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long StorageSoftInMb { get; set; }

        [Required]
        public VlanNetwork Vlan { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long VlansHard { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public long VlansSoft { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SyncStateEnum SyncState { get; set; }

        public string ProviderId { get; set; }
    }
}
