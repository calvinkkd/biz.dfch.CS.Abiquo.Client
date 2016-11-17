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
using System.Management.Automation;
using biz.dfch.CS.Testing.Attributes;
using biz.dfch.CS.Testing.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Current = biz.dfch.CS.Abiquo.Client.v1;
namespace biz.dfch.PS.Abiquo.Client.Tests
{
    [TestClass]
    public class EnterServerTest
    {
        [TestMethod]
        [ExpectParameterBindingException(MessagePattern = @"'Uri'.+'System\.Uri'")]
        public void InvokeWithInvalidUriParameterThrowsParameterBindingException1()
        {
            var parameters = @"-Uri ";
            var results = PsCmdletAssert.Invoke(typeof(EnterServer), parameters);
        }

        [TestMethod]
        [ExpectParameterBindingException(MessagePattern = @"Credential")]
        public void InvokeWithMissingiParameterThrowsParameterBindingException()
        {
            var parameters = @"-Uri http://localhost";
            var results = PsCmdletAssert.Invoke(typeof(EnterServer), parameters);
        }

        [TestMethod]
        [ExpectParameterBindingException(MessagePattern = @"'Credential'.+.System\.String.")]
        public void InvokeWithMissingiParameterThrowsParameterBindingException2()
        {
            var parameters = @"-Uri http://localhost -Credential arbitrary-user-as-string";
            var results = PsCmdletAssert.Invoke(typeof(EnterServer), parameters);
        }

        //[TestMethod]
        //public void InvokeWithParameterSetPlainSucceeds()
        //{
        //    var user = "arbitrary-user";
        //    var password = "arbitrary-password";
        //    var tenantId = 42;
        //    var parameters = string.Format(@"-Uri http://localhost -User '{0}' -Password '{1}' -TenantId {2}", user, password, tenantId);
        //    var results = PsCmdletAssert.Invoke(typeof(EnterServer), parameters);
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(1, results.Count);
        //    var result = results[0].BaseObject.ToString();
        //    Assert.AreEqual("tralala", result);
        //}

        [TestMethod]
        [ExpectedException(typeof(IncompleteParseException))]
        public void InvokeWithInvalidStringThrowsIncompleteParseException()
        {
            var user = "arbitrary-user";
            var password = "arbitrary-password";
            // missing string terminator
            var parameters = string.Format(@"-Uri http://localhost -User '{0} -Password '{1}'", user, password);
            var results = PsCmdletAssert.Invoke(typeof(EnterServer), parameters);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            var result = results[0].BaseObject.ToString();
            Assert.AreEqual("tralala", result);
        }
    }
}
