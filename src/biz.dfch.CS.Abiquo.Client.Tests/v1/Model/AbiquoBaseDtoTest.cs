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

using System.Collections.Generic;
using biz.dfch.CS.Abiquo.Client.v1;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Abiquo.Client.v1.Model;
using biz.dfch.CS.Testing.Attributes;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1.Model
{
    [TestClass]
    public class AbiquoBaseDtoTest
    {
        private const string USERS_HREF = "https://abiquo/api/admin/enterprises/1/users";
        private const string PROPERTIES_HREF = "https://abiquo/api/admin/enterprises/1/properties";
        private const string USERS_REL = "users";
        private const string PROPERTIES_REL = "properties";

        [TestMethod]
        public void GetLinkByRelWithExistingRelReturnsExpectedLink()
        {
            // Arrange
            var enterprise = CreateEnterpriseWithLinks();

            // Act
            var usersLink = enterprise.GetLinkByRel(USERS_REL);

            // Assert
            Assert.IsNotNull(usersLink);
            Assert.AreEqual(USERS_REL, usersLink.Rel);
            Assert.AreEqual(USERS_HREF, usersLink.Href);
            Assert.AreEqual(USERS_REL, usersLink.Title);
            Assert.AreEqual(AbiquoMediaDataTypes.VND_ABIQUO_USERS, usersLink.Type);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetLinkByRelWithInexistentRelThrowsContractException()
        {
            // Arrange
            var enterprise = CreateEnterpriseWithLinks();

            // Act
            enterprise.GetLinkByRel("Arbitrary");

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetLinkByRelWithNullRelThrowsContractException()
        {
            // Arrange
            var enterprise = CreateEnterpriseWithLinks();

            // Act
            enterprise.GetLinkByRel(null);

            // Assert
        }

        private Enterprise CreateEnterpriseWithLinks()
        {
            var usersLink = new LinkBuilder()
                .BuildRel(USERS_REL)
                .BuildHref(USERS_HREF)
                .BuildType(AbiquoMediaDataTypes.VND_ABIQUO_USERS)
                .BuildTitle(USERS_REL).GetLink();

            var propertiesLink = new LinkBuilder()
                .BuildRel(PROPERTIES_REL)
                .BuildHref(PROPERTIES_HREF)
                .BuildType(AbiquoMediaDataTypes.VND_ABIQUO_ENTERPRISEPROPERTIES)
                .BuildTitle(PROPERTIES_REL)
                .GetLink();

            var enterprise = new Enterprise()
            {
                Id = 42
                ,
                Name = "Arbitrary Enterprise"
                ,
                Links = new List<Link>() { usersLink, propertiesLink }
            };

            return enterprise;
        }
    }
}
