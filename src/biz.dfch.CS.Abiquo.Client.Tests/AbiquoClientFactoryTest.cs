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
﻿using biz.dfch.CS.Abiquo.Client.v1;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Abiquo.Client.Tests
{
    [TestClass]
    public class AbiquoClientFactoryTest
    {
        [TestMethod]
        public void GetByVersionReturnsCorrespondingAbiquoClient()
        {
            var abiquoClient = AbiquoClientFactory.GetByVersion("v2");
            Assert.IsNotNull(abiquoClient);
            Assert.AreEqual(typeof(AbiquoClient).FullName, abiquoClient.GetType().FullName);
        }

        [TestMethod]
        public void GetByVersionReturnsNull()
        {
            var abiquoClient = AbiquoClientFactory.GetByVersion("vx");
            Assert.IsNull(abiquoClient);
        }

        [TestMethod]
        public void GetByCommitHashReturnsCorrespondingAbiquoClient()
        {
            var abiquoClient = AbiquoClientFactory.GetByCommitHash("hash");
            Assert.IsNotNull(abiquoClient);
            Assert.AreEqual(typeof(AbiquoClient).FullName, abiquoClient.GetType().FullName);
        }
    }
}
