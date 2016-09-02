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
 
﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
﻿using System.ComponentModel.DataAnnotations;
﻿using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Abiquo.Client.v1.Model
{
    public class VirtualAppliance : AbiquoBaseDto
    {
        public int Error { get; set; }

        public int HighDisponibility { get; set; }

        public int Id { get; set; }

        public Tasks LastTasks { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string NodeConnections {get; set; }
        
        public int PublicApp { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public VirtualApplianceState State { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public VirtualApplianceState SubState { get; set; }
    }
}