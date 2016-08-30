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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using biz.dfch.CS.Abiquo.Client.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace biz.dfch.CS.Abiquo.Client.v1.Model
{
    public class Task : BaseDto
    {
        public List<Jobs> Jobs { get; set; }

        public string OwnerId { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public TaskState State { get; set; }
        
        public string TaskId { get; set; }
        
        public long Timestamp { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public TaskType Type { get; set; }
        
        public string UserId { get; set; }
        
        // DFTODO - check, if links get delivered by the REST response
        public List<Link> Links { get; set; }
    }
}
