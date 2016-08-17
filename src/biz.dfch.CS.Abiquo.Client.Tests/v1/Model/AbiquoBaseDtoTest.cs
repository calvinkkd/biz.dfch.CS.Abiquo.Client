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
 
﻿using biz.dfch.CS.Abiquo.Client.v1.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Abiquo.Client.Tests.v1.Model
{
    [TestClass]
    public class AbiquoBaseDtoTest
    {
        private const string SAMPLE_DTO_NAME = "ArbitraryName";
        private const string SAMPLE_DTO_JSON_REPRESENTATION = "{\"Name\":\"ArbitraryName\"}";

        [TestMethod]
        public void SerializeSampleDtoReturnsJsonRepresentationOfSampleDto()
        {
            // Arrange
            var sampleDto = new SampleDto() { Name = SAMPLE_DTO_NAME };

            // Act
            var serializationResult = sampleDto.SerializeObject();

            // Assert
            Assert.AreEqual(SAMPLE_DTO_JSON_REPRESENTATION, serializationResult);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeserializeSampleDtoWithNullValueThrowsContractException()
        {
            // Arrange

            // Act
            AbiquoBaseDto.DeserializeObject(null, typeof(SampleDto));

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeserializeSampleDtoWithNullTypeThrowsContractException()
        {
            // Arrange

            // Act
            AbiquoBaseDto.DeserializeObject(SAMPLE_DTO_JSON_REPRESENTATION, null);

            // Assert
        }

        [TestMethod]
        public void DeserializationOfSampleDtoSucceeds()
        {
            // Arrange

            // Act
            var dto = AbiquoBaseDto.DeserializeObject(SAMPLE_DTO_JSON_REPRESENTATION, typeof(SampleDto));

            // Assert
            Assert.IsTrue(dto.GetType() == typeof(SampleDto));
            Assert.AreEqual(SAMPLE_DTO_NAME, ((SampleDto)dto).Name);
        }

        [TestMethod]
        public void GenericDeserializationOfSampleDtoSucceeds()
        {
            // Arrange

            // Act
            var dto = AbiquoBaseDto.DeserializeObject(SAMPLE_DTO_JSON_REPRESENTATION, typeof(SampleDto));

            // Assert
            Assert.IsTrue(dto.GetType() == typeof(SampleDto));
            Assert.AreEqual(SAMPLE_DTO_NAME, ((SampleDto)dto).Name);
        }

        private class SampleDto : AbiquoBaseDto
        {
            [Required]
            public string Name { get; set; }
        }
    }
}
