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
 
﻿using System;
using System.Collections.Generic;
﻿using System.ComponentModel.DataAnnotations;
﻿using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Abiquo.Client.v1.Model
{
    public class DhcpOption : AbiquoBaseDto
    {
        public int Id { get; set; }

        [Required]
        public string Gateway { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        public int Mask { get; set; }

        [Required]
        public string Netmask { get; set; }

        [Required]
        public string NetworkAddress { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        public int Option { get; set; }
    }
}
