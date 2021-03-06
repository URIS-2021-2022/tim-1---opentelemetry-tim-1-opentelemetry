// <copyright file="DiagnosticSourceListenerTest.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Diagnostics;

namespace OpenTelemetry.Instrumentation.Tests
{
    public class DiagnosticSourceListenerTest
    {
        private const string TestSourceName = "TestSourceName";
        private readonly TestListenerHandler testListenerHandler;
        private readonly DiagnosticSourceSubscriber testDiagnosticSourceSubscriber;

        public DiagnosticSourceListenerTest()
        {
            this.testListenerHandler = new TestListenerHandler(TestSourceName);
            this.testDiagnosticSourceSubscriber = new DiagnosticSourceSubscriber(this.testListenerHandler, null);
            this.testDiagnosticSourceSubscriber.Subscribe();
        }
    }
}
