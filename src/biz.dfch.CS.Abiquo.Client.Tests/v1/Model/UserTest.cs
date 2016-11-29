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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Abiquo.Client.General;
using biz.dfch.CS.Abiquo.Client.v1.Model;
using System.IO;
using biz.dfch.CS.Abiquo.Client.v1;
using biz.dfch.CS.Testing.Attributes;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1.Model
{
    [TestClass]
    public class UserTest
    {
        private const string USERNAME = "ArbitraryUserName";
        private const string AUTH_TYPE = "Arbitrary AuthType";
        private const string LINK_TITLE = "Abiquo";
        private const string LINK_HREF = "https://abiquo/api/admin/enterprises/42";
        private const int ENTERPRISE_ID = 42;

        [TestMethod]
        public void GetEnterpriseIdReturnsEnterpriseIdFromEnterpriseLink()
        {
            // Arrange
            var user = new User
            {
                Active = true,
                AuthType = AUTH_TYPE,
                Name = USERNAME,
                Nick = USERNAME
            };

            var enterpriseLink =
                new LinkBuilder().BuildHref(LINK_HREF).BuildRel(AbiquoRelations.ENTERPRISE).BuildTitle(LINK_TITLE).GetLink();

            user.Links = new List<Link>()
            {
                enterpriseLink 
            };

            // Act
            var enterpriseId = user.GetEnterpriseId();

            // Assert
            Assert.AreEqual(ENTERPRISE_ID, enterpriseId);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = @"null != Contract.Result<Link>")]
        public void GetEnterpriseIdOfUserWithoutEnterpriseLinkThrowsContractException()
        {
            // Arrange
            var user = new User
            {
                Active = true,
                AuthType = AUTH_TYPE,
                Name = USERNAME,
                Nick = USERNAME
            };

            // Act
            user.GetEnterpriseId();
        }
    }
}
