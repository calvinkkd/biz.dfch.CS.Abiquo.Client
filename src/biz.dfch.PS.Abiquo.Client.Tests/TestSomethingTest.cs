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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Automation;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.PS.Abiquo.Client.Tests
{
    [TestClass]
    public class TestSomethingTest
    {
        [TestMethod]
        public void TestSometingSucceeds()
        {
            var sut = new TestSomething
            {
                BaseUrl = new Uri("http://www.example.com/")
                ,
                Username = "arbitraryUserName"
                ,
                Password = "arbitraryPassword"
            };

            //var results = sut.Invoke<string>();
            var enumerable = sut.Invoke();
            Assert.IsNotNull(enumerable);
            
            var results = GetInvocationResults(enumerable);
            Assert.IsNotNull(results);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("tralala", results[0]);
        }

        #region CmdletEvaluationHelper - should go into testing library when finished
        public static Type GetOutputType<T>(T cmdlet)
            where T : Cmdlet
        {
            Contract.Requires(null != cmdlet);

            return GetOutputType(typeof(T), default(string));
        }
            
        public static Type GetOutputType<T>(T cmdlet, string parameterSetName)
            where T : Cmdlet
        {
            Contract.Requires(null != cmdlet);

            return GetOutputType(typeof(T), parameterSetName);
        }

        public static Type GetOutputType(Type type)
        {
            Contract.Requires(null != type);
            return GetOutputType(type, default(string));
        }

        public static Type GetOutputType(Type type, string parameterSetName)
        {
            Contract.Requires(null != type);

            var outputTypeAttributes = (OutputTypeAttribute[]) type.GetCustomAttributes(typeof(OutputTypeAttribute), true);

            if (0 >= outputTypeAttributes.Length)
            {
                return default(Type);
            }

            foreach (var outputTypeAttribute in outputTypeAttributes)
            {
                foreach (var psTypeName in outputTypeAttribute.Type)
                {
                    if (string.IsNullOrEmpty(parameterSetName))
                    {
                        return psTypeName.Type;
                    }
                    
                    if (!string.IsNullOrEmpty(parameterSetName) && outputTypeAttribute.ParameterSetName.Contains(parameterSetName))
                    {
                        return psTypeName.Type;
                    }
                }
            }

            return default(Type);
        }

        public IList<object> GetInvocationResults(IEnumerable enumerable)
        {
            Contract.Requires(null != enumerable);
            Contract.Ensures(null != Contract.Result<IList<object>>());

            return enumerable.Cast<object>().ToList();
        }

        public IList<T> GetInvocationResults<T>(IEnumerable<T> enumerable)
            where T : class
        {
            Contract.Requires(null != enumerable);
            Contract.Ensures(null != Contract.Result<IList<T>>());

            return enumerable.ToList();
        }

        #endregion
            
    }
}
