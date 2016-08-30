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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Abiquo.Client.v1.Model
{
    public class Enterprise : AbiquoBaseDto
    {
        public string ChefClient { get; set; }

        public string ChefClientCertificate { get; set; }

        public string ChefUrl { get; set; }

        public string ChefValidator { get; set; }

        public string ChefValidatorCertificate { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int CpuCountHardLimit { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int CpuCountSoftLimit { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long DiskHardLimitInMb { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long DiskSoftLimitInMb { get; set; }

        public int Id { get; set; }

        public int IdPricingTemplate { get; set; }
        
        public bool IsReservationRestricted { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long PublicIpsHard { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long PublicIpsSoft { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int RamHardLimitInMb { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int RamSoftLimitInMb { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long RepositoryHardInMb { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long RepositorySoftInMb { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long StorageHardInMb { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long StorageSoftInMb { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long VlansHard { get; set; }

        [Required]
        [Range(1, Int64.MaxValue)]
        public long VlansSoft { get; set; }

        // The following properties get delivered by the rest response 
        // but are not mentioned in the wiki 
        // http://wiki.abiquo.com/display/ABI38/Abiquo+Data+Media+Types#AbiquoDataMediaTypes-EnterpriseMediaType
        public bool Workflow { get; set; }

        public bool TwoFactorAuthenticationMandatory { get; set; }
    }
}
