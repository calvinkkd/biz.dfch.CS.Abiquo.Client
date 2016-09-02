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
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Utilities.Testing;
using biz.dfch.CS.Abiquo.Client.v1.Model;
using biz.dfch.CS.Abiquo.Client.v1;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1.Model
{
    [TestClass]
    public class AbiquoBaseCollectionDtoTest
    {
        private const string FIRST_HREF = "https://abiquo/api/admin/enterprises?limit=25&by=name";
        private const string LAST_HREF = "https://abiquo/api/admin/enterprises?startwith=0&limit=25&by=name";
        private const string FIRST_REL = "first";
        private const string LAST_REL = "last";

        [TestMethod]
        public void GetLinkByRelWithExistingRelReturnsExpectedLink()
        {
            // Arrange
            var enterprises = CreateEnterprisesWithLinks();

            // Act
            var firstLink = enterprises.GetLinkByRel(FIRST_REL);

            // Assert
            Assert.IsNotNull(firstLink);
            Assert.AreEqual(FIRST_REL, firstLink.Rel);
            Assert.AreEqual(FIRST_HREF, firstLink.Href);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetLinkByRelWithInexistentRelThrowsContractException()
        {
            // Arrange
            var enterprises = CreateEnterprisesWithLinks();

            // Act
            enterprises.GetLinkByRel("Arbitrary");

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetLinkByRelWithNullRelThrowsContractException()
        {
            // Arrange
            var enterprises = CreateEnterprisesWithLinks();

            // Act
            enterprises.GetLinkByRel(null);

            // Assert
        }

        private Enterprises CreateEnterprisesWithLinks()
        {
            var firstLink = new LinkBuilder().BuildRel(FIRST_REL).BuildHref(FIRST_HREF).GetLink();
            var lastLink = new LinkBuilder().BuildRel(LAST_REL).BuildHref(LAST_HREF).GetLink();

            var enterprises = new Enterprises()
            {
                Collection = new List<Enterprise>() { CreateEnterpriseWithLinks() }
                ,
                Links = new List<Link>() { firstLink, lastLink }
                ,
                TotalSize = 1

            };

            return enterprises;
        }

        private Enterprise CreateEnterpriseWithLinks()
        {
            var enterprise = new Enterprise()
            {
                Id = 42
                ,
                Name = "Arbitrary Enterprise"
                ,
                Links = new List<Link>()
            };

            return enterprise;
        }
    }
}