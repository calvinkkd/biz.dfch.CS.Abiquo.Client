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
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// ContractEventHandler
    /// </summary>
    public class ContractEventHandler
    {
        internal const int EVENT_ID = Int16.MaxValue;

        internal const string CONTRACTS_RUNTIME_NAMESPACE1 = "System.Runtime.CompilerServices.ContractHelper";
        internal const string CONTRACTS_RUNTIME_NAMESPACE2 = "System.Diagnostics.Contracts.__ContractsRuntime";

        /// <summary>
        /// ContractFailedEventHandler
        /// </summary>
        /// <param name="sender">always null</param>
        /// <param name="args">holds the assertion being triggered</param>
        public static void ContractFailedEventHandler(object sender, ContractFailedEventArgs args)
        {
            var index = 0;
            var stackFrame = new StackTrace(index);
            
            var frames = stackFrame.GetFrames();
            if (null == frames)
            {
                return;
            }

            var frame = default(StackFrame);
            for (index = 1; index < frames.Length; index++)
            {
                var currentFrame = frames[index];
                var currentDeclaringType = currentFrame.GetMethod().DeclaringType;
                if (null == currentDeclaringType)
                {
                    continue;
                }

                if (currentDeclaringType.FullName.StartsWith(CONTRACTS_RUNTIME_NAMESPACE1) ||
                    currentDeclaringType.FullName.StartsWith(CONTRACTS_RUNTIME_NAMESPACE2))
                {
                    continue;
                }

                frame = currentFrame;
                break;
            }

            if (null == frame)
            {
                return;
            }

            var methodBase = frame.GetMethod();
            var declaringType = frame.GetMethod().DeclaringType;
            var methodName = null != declaringType ? string.Format("{0}.{1}", declaringType.FullName, methodBase.Name) : methodBase.Name;

            if (ContractFailureKind.Precondition == args.FailureKind ||
                ContractFailureKind.Postcondition == args.FailureKind)
            {
                ModuleContext.TraceSourceInternal.Value.TraceEvent(TraceEventType.Error, EVENT_ID, "{0}: {1}", methodName, args.Message);
            }
            else
            {
                stackFrame = new StackTrace(index);
                ModuleContext.TraceSourceInternal.Value.TraceEvent(TraceEventType.Error, EVENT_ID, "{0}: {1}\r\n{2}", methodName, args.Message, stackFrame);
            }
        }
    }
}