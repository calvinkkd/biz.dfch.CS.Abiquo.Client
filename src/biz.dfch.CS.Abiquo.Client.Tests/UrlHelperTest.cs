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
﻿using biz.dfch.CS.Abiquo.Client.Communication;
﻿using biz.dfch.CS.Utilities.Testing;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Abiquo.Client.Tests
{
    [TestClass]
    public class UrlHelperTest
    {
        private const string ABIQUO_API_BASE_URL = "https://abiquo/api/";

        [TestMethod]
        [ExpectContractFailure]
        public void ConcatUrlWithInvalidBaseUrlThrowsContractException()
        {
            // Arrange

            // Act
            UrlHelper.ConcatUrl(null, AbiquoUrlSuffix.LOGIN);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConcatUrlWithEmptyBaseUrlThrowsContractException()
        {
            // Arrange

            // Act
            UrlHelper.ConcatUrl(" ", AbiquoUrlSuffix.LOGIN);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConcatUrlWithNullUrlSuffixThrowsContractException()
        {
            // Arrange

            // Act
            UrlHelper.ConcatUrl(ABIQUO_API_BASE_URL, null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConcatUrlWithEmptyUrlSuffixThrowsContractException()
        {
            // Arrange

            // Act
            UrlHelper.ConcatUrl(ABIQUO_API_BASE_URL, " ");
            
            // Assert
        }

        [TestMethod]
        public void ConcatUrlReturnsValidUrl()
        {
            // Arrange
            var expectedUrl = "http://example.com/api";

            // Act

            // Assert
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api/", "login"));
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api/", "/login"));
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api/", "login/"));
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api/", "/login/"));
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api", "login"));
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api", "/login"));
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api", "login/"));
            Assert.AreEqual(expectedUrl, UrlHelper.ConcatUrl("http://example.com/api", "/login/"));
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateFilterStringWithNullDictionaryThrowsContractException()
        {
            // Arrange

            // Act
            UrlHelper.CreateFilterString(null);

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void CreateFilterStringWithEmptyDictionaryThrowsContractException()
        {
            // Arrange
            var emptyFilter = new Dictionary<string, object>();

            // Act
            UrlHelper.CreateFilterString(emptyFilter);

            // Assert
        }

        [TestMethod]
        public void CreateFilterStringWithReturnsFilterAsString()
        {
            // Arrange
            var expectedFilterString = "maxSize=5&currentPage=1&limit=25";

            var filter = new Dictionary<string, object>()
            {
                {"maxSize", 5},
                {"currentPage", 1},
                {"limit", "25"}
            };

            // Act
            var filterString = UrlHelper.CreateFilterString(filter);

            // Assert
            Assert.AreEqual(expectedFilterString, filterString);
        }
    }
}
