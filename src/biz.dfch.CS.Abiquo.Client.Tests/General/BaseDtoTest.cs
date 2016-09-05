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

using System.ComponentModel.DataAnnotations;
using biz.dfch.CS.Abiquo.Client.Factory;
using biz.dfch.CS.Utilities.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Abiquo.Client.General;

namespace biz.dfch.CS.Abiquo.Client.Tests.General
{
    [TestClass]
    public class BaseDtoTest
    {
        private const string SAMPLE_DTO_NAME = "ArbitraryName";
        private const string SAMPLE_DTO_JSON_REPRESENTATION = "{\"name\":\"ArbitraryName\"}";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // IMPORTANT: A new AbiquoClient gets created to make sure 
            // the serialization settings defined in the static constructor 
            // of the AbiquoClient get applied to the Newtonsoft JsonConverter
            AbiquoClientFactory.GetByVersion(AbiquoClientFactory.ABIQUO_CLIENT_VERSION_V1);            
        }

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
            BaseDto.DeserializeObject(null, typeof(SampleDto));

            // Assert
        }

        [ExpectContractFailure]
        public void DeserializeSampleDtoWithEmptyValueThrowsContractException()
        {
            // Arrange

            // Act
            BaseDto.DeserializeObject(" ", typeof(SampleDto));

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure]
        public void DeserializeSampleDtoWithNullTypeThrowsContractException()
        {
            // Arrange

            // Act
            BaseDto.DeserializeObject(SAMPLE_DTO_JSON_REPRESENTATION, null);

            // Assert
        }

        [TestMethod]
        public void DeserializeSampleDtoSucceeds()
        {
            // Arrange

            // Act
            var sampleDto = BaseDto.DeserializeObject(SAMPLE_DTO_JSON_REPRESENTATION, typeof(SampleDto));

            // Assert
            Assert.IsTrue(typeof(SampleDto) == sampleDto.GetType());
            Assert.AreEqual(SAMPLE_DTO_NAME, ((SampleDto)sampleDto).Name);
        }

        [TestMethod]
        public void GenericDeserializeSampleDtoSucceeds()
        {
            // Arrange

            // Act
            var sampleDto = BaseDto.DeserializeObject<SampleDto>(SAMPLE_DTO_JSON_REPRESENTATION);

            // Assert
            Assert.IsTrue(typeof(SampleDto) == sampleDto.GetType());
            Assert.AreEqual(SAMPLE_DTO_NAME, sampleDto.Name);
        }

        [TestMethod]
        public void IsValidForSampleDtoWithoutNameReturnsFalse()
        {
            // Arrange
            var sampleDto = new SampleDto();

            // Act
            var isValid = sampleDto.IsValid();

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidForSampleDtoWithValidNameReturnsTrue()
        {
            // Arrange
            var sampleDto = new SampleDto() { Name = SAMPLE_DTO_NAME };

            // Act
            var isValid = sampleDto.IsValid();

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void GetValidationResultsForSampleDtoWithInvalidNameReturnsListOfResults()
        {
            // Arrange
            var sampleDto = new SampleDto() { Name = " " };

            // Act
            var validationResults = sampleDto.GetValidationResults();

            // Arrange
            Assert.AreEqual(1, validationResults.Count);
        }

        [TestMethod]
        public void GetValidationResultsForSampleDtoWithValidNameReturnsEmptyList()
        {
            // Arrange
            var sampleDto = new SampleDto() { Name = SAMPLE_DTO_NAME };

            // Act
            var validationResults = sampleDto.GetValidationResults();

            // Arrange
            Assert.AreEqual(0, validationResults.Count);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ValidateSampleDtoWithInvalidNameThrowsContractException()
        {
            // Arrange
            var sampleDto = new SampleDto() { Name = " " };

            // Act
            sampleDto.Validate();

            // Arrange
        }

        [TestMethod]
        public void ValidateSampleDtoWithValidNameSucceeds()
        {
            // Arrange
            var sampleDto = new SampleDto() { Name = SAMPLE_DTO_NAME };

            // Act
            sampleDto.Validate();

            // Arrange
        }

        private class SampleDto : BaseDto
        {
            [Required(AllowEmptyStrings = false)]
            public string Name { get; set; }
        }
    }
}
