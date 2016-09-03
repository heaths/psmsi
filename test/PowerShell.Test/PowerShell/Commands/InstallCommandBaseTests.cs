// The MIT License (MIT)
//
// Copyright (c) Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PS = System.Management.Automation.PowerShell;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Tests for the <see cref="InstallCommandBase"/> class.
    /// </summary>
    [TestClass]
    public sealed class InstallCommandBaseTests : TestBase
    {
        [TestMethod]
        public void ExecuteQueuedActions()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);

                var objs = ps.AddScript("@('foo', 'bar')").AddCommand("test-install").Invoke();

                Assert.AreEqual<int>(2, objs.Count, "The number of objects returned is incorrect.");
                Assert.AreEqual<int>(2, (int)objs[0].BaseObject, "The execute count is incorrect.");
                Assert.AreEqual<int>(1, (int)objs[1].BaseObject, "The maximum queue count is incorrect.");
            }
        }

        [TestMethod]
        public void ExecuteQueuedActionsAsChain()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);

                try
                {
                    SystemRestorePoint.DefaultServiceProvider = new SystemRestoreTestService();
                    var objs = ps.AddScript("@('foo', 'bar')").AddCommand("test-install").AddParameter("chain").Invoke();

                    Assert.AreEqual<int>(2, objs.Count, "The number of objects returned is incorrect.");
                    Assert.AreEqual<int>(2, (int)objs[0].BaseObject, "The execute count is incorrect.");
                    Assert.AreEqual<int>(2, (int)objs[1].BaseObject, "The maximum queue count is incorrect.");
                }
                finally
                {
                    SystemRestorePoint.DefaultServiceProvider = null;
                }
            }
        }

        [TestMethod]
        public void ExecuteInstallFailure()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);

                ps.AddCommand("test-install").AddArgument("fail").Invoke();

                Assert.AreEqual<int>(1, ps.Streams.Error.Count, "The number of errors returned is incorrect.");

                var error = ps.Streams.Error[0];
                Assert.AreEqual<ErrorCategory>(ErrorCategory.NotSpecified, error.CategoryInfo.Category, "The error category is incorrect.");
                Assert.AreEqual("Simulated install failure.", error.Exception.Message, "The exception message is incorrect.");
            }
        }

        [TestMethod]
        public void ExecuteErrorMessage()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);

                ps.AddCommand("test-install").AddArgument("error").Invoke();

                Assert.AreEqual<int>(1, ps.Streams.Error.Count, "The number of errors returned is incorrect.");

                var error = ps.Streams.Error[0];
                Assert.AreEqual<ErrorCategory>(ErrorCategory.WriteError, error.CategoryInfo.Category, "The error category is incorrect.");
                Assert.AreEqual("error", error.TargetObject as string, "The error target is incorrect.");
            }
        }

        [TestMethod]
        public void ExecuteWarningMessage()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);

                ps.AddCommand("test-install").AddArgument("warning").Invoke();

                Assert.AreEqual<int>(1, ps.Streams.Warning.Count, "The number of warnings returned is incorrect.");
                Assert.AreEqual("warning", ps.Streams.Warning[0].Message, "The warning message is incorrect.");
            }
        }

        [TestMethod]
        public void ExecuteVerboseMessage()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);

                ps.AddCommand("test-install").AddArgument("verbose").AddParameter("verbose").Invoke();

                Assert.AreEqual<int>(1, ps.Streams.Verbose.Count, "The number of verbose messages returned is incorrect.");
                Assert.AreEqual("verbose", ps.Streams.Verbose[0].Message, "The verbose message is incorrect.");
            }
        }

        [TestMethod]
        public void ExecuteSimulatedProgress()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);

                ps.AddCommand("test-install").AddArgument("progress").Invoke();

                Assert.AreEqual<int>(8, ps.Streams.Progress.Count, "The number of progress messages returned is incorrect.");

                // Phase 1: No product name or total tick count yet.
                var progress = ps.Streams.Progress[0];
                Assert.AreEqual("Testing", progress.Activity, "The phase 1 activity is incorrect.");
                Assert.AreEqual("Please wait...", progress.StatusDescription, "The phase 1 status description is incorrect.");

                // Phase 2: With product name, estimated tick count, generating script.
                for (int i = 1; i < 4; ++i)
                {
                    progress = ps.Streams.Progress[i];
                    Assert.AreEqual("Testing INSTALL", progress.Activity, "The phase 2 step {0:d} activity is incorrect.", i);

                    if (1 == i)
                    {
                        // ActionStart not run yet.
                        Assert.AreEqual("Please wait...", progress.StatusDescription, "The phase 2 step 1 activity is incorrect.");
                    }
                    else
                    {
                        Assert.AreEqual("Generating script...", progress.StatusDescription, "The phase 2 step {0:d} status description is incorrect.", i);
                    }
                }

                Assert.AreEqual<int>(0, ps.Streams.Progress[1].PercentComplete, "The phase 2 step 1 % complete is incorrect.");
                Assert.AreEqual<int>(7, ps.Streams.Progress[2].PercentComplete, "The phase 2 step 2 % complete is incorrect.");
                Assert.AreEqual<int>(5, ps.Streams.Progress[3].PercentComplete, "the phase 2 step 3 % complete is incorrect.");

                // Phase 3: With product name, estimated tick count, executing script.
                progress = ps.Streams.Progress[4];
                Assert.AreEqual("Testing INSTALL", progress.Activity, "The phase 3 step 1 activity is incorrect.");
                Assert.AreEqual("Generating script...", progress.StatusDescription, "The phase 3 step 1 status description is incorrect.");
                Assert.AreEqual<int>(15, progress.PercentComplete, "The phase 3 step 1 % complete is incorrect.");
                Assert.IsNull(progress.CurrentOperation, "The phase 3 step 1 operation is incorrect.");

                progress = ps.Streams.Progress[5];
                Assert.AreEqual("Testing INSTALL", progress.Activity, "The phase 3 step 2 activity is incorrect.");
                Assert.AreEqual("Testing", progress.StatusDescription, "The phase 3 step 2 status description is incorrect.");
                Assert.AreEqual<int>(15, progress.PercentComplete, "The phase 3 step 2 % complete is incorrect.");
                Assert.IsNull(progress.CurrentOperation, "The phase 3 step 2 operation is incorrect.");

                progress = ps.Streams.Progress[6];
                Assert.AreEqual("Testing INSTALL", progress.Activity, "The phase 3 step 3 activity is incorrect.");
                Assert.AreEqual("Testing", progress.StatusDescription, "The phase 3 step 3 status description is incorrect.");
                Assert.AreEqual<int>(55, progress.PercentComplete, "The phase 3 step 3 % complete is incorrect.");
                Assert.AreEqual("Testing: ActionData", progress.CurrentOperation, "The phase 3 step 2 operation is incorrect.");

                // Make sure progress was completed.
                Assert.AreEqual<ProgressRecordType>(ProgressRecordType.Completed, ps.Streams.Progress[7].RecordType, "Progress not completed.");
            }
        }

        [TestMethod]
        public void SetInstallResultVariable()
        {
            using (var ps = PS.Create())
            {
                var variable = ps.Runspace.SessionStateProxy.PSVariable.Get("a");
                Assert.IsNull(variable);

                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);
                ps.AddCommand("test-install").AddArgument("verbose").AddParameter("ResultVariable", "a").Invoke();

                variable = ps.Runspace.SessionStateProxy.PSVariable.Get("a");
                Assert.IsNotNull(variable);

                var value = variable.Value as Result;
                Assert.IsNotNull(value);
                Assert.IsFalse(value.RebootInitiated);
                Assert.IsFalse(value.RebootRequired);
            }
        }

        [TestMethod]
        public void AppendInstallResultVariable()
        {
            using (var ps = PS.Create())
            {
                var value = new Result()
                {
                    RebootInitiated = true,
                    RebootRequired = true,
                };

                var variable = new PSVariable("a", value, ScopedItemOptions.AllScope);
                ps.Runspace.SessionStateProxy.PSVariable.Set(variable);

                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);
                ps.AddCommand("test-install").AddArgument("verbose").AddParameter("ResultVariable", "+a").Invoke();

                variable = ps.Runspace.SessionStateProxy.PSVariable.Get("a");
                Assert.IsNotNull(variable);

                value = variable.Value as Result;
                Assert.IsNotNull(value);
                Assert.IsTrue(value.RebootInitiated);
                Assert.IsTrue(value.RebootRequired);
            }
        }

        [TestMethod]
        public void AppendNonexistentInstallResultVariable()
        {
            using (var ps = PS.Create())
            {
                TestInstallCommand.Register(ps.Runspace.RunspaceConfiguration);
                ps.AddCommand("test-install").AddArgument("verbose").AddParameter("ResultVariable", "+a").Invoke();

                var variable = ps.Runspace.SessionStateProxy.PSVariable.Get("a");
                Assert.IsNotNull(variable);

                var value = variable.Value as Result;
                Assert.IsNotNull(value);
                Assert.IsFalse(value.RebootInitiated);
                Assert.IsFalse(value.RebootRequired);
            }
        }

        [Cmdlet(VerbsDiagnostic.Test, "Install", DefaultParameterSetName = "Action")]
        internal sealed class TestInstallCommand : InstallCommandBase<TestInstallCommand.ActionData>
        {
            internal static void Register(RunspaceConfiguration config)
            {
                config.Cmdlets.Append(new CmdletConfigurationEntry("test-install", typeof(TestInstallCommand), null));
                config.Cmdlets.Update();
            }

            internal int ExecuteCount { get; set; }
            internal int QueueCount { get; set; }
            internal int QueueCountMax { get; set; }

            [Parameter(ParameterSetName = "Action", Position = 0, Mandatory = true, ValueFromPipeline = true)]
            public string[] Action { get; set; }

            protected override string Activity
            {
                get { return "Testing {0}"; }
            }

            protected override void ExecuteAction(ActionData data)
            {
                this.ExecuteCount++;

                // The action data was already dequeued so add 1 to the max count.
                this.QueueCountMax = Math.Max(this.QueueCountMax, this.Actions.Count + 1);

                switch (data.Action.ToLowerInvariant())
                {
                    case "error":
                        this.TestError();
                        break;

                    case "warning":
                        this.TestWarning();
                        break;

                    case "verbose":
                        this.TestVerbose();
                        break;

                    case "progress":
                        this.OnMessage(InstallMessage.Initialize);
                        this.TestProgressPhase1();
                        this.TestProgressPhase2();
                        this.TestProgressPhase3();
                        break;

                    case "fail":
                        throw new InstallerException("Simulated install failure.");
                }
            }

            protected override void QueueActions()
            {
                Assert.AreEqual("Action", this.ParameterSetName, "The resolved parameter set name is incorrect.");

                foreach (string token in this.Action)
                {
                    var data = new ActionData()
                    {
                        CommandLine = null != this.Properties && 0 < this.Properties.Length ? string.Join(" ", this.Properties) : null,
                        Action = token,
                        Weight = 100,
                    };

                    this.QueueCount++;
                    this.Actions.Enqueue(data);
                }
            }

            protected override void EndProcessing()
            {
                base.EndProcessing();

                this.WriteObject(this.ExecuteCount);
                this.WriteObject(this.QueueCountMax);
            }

            private void OnMessage(InstallMessage type, Deployment.WindowsInstaller.Record record = null)
            {
                this.OnMessage(type, record, MessageButtons.OKCancel, MessageIcon.None, MessageDefaultButton.Button1);
            }

            private void TestError()
            {
                using (var error = new Deployment.WindowsInstaller.Record(2))
                {
                    error.SetInteger(1, 1301);
                    error.SetString(2, "error");
                    this.OnMessage(InstallMessage.Error, error);
                }
            }

            private void TestWarning()
            {
                using (var warning = new Deployment.WindowsInstaller.Record(0))
                {
                    warning.FormatString = "warning";
                    this.OnMessage(InstallMessage.Warning, warning);
                }
            }

            private void TestVerbose()
            {
                using (var verbose = new Deployment.WindowsInstaller.Record(0))
                {
                    verbose.FormatString = "verbose";
                    this.OnMessage(InstallMessage.Info, verbose);
                }
            }

            private void TestProgressPhase1()
            {
                // No product name or total tick count yet.
                using (var rec = new Deployment.WindowsInstaller.Record(2))
                {
                    rec.SetInteger(1, 2);
                    rec.SetInteger(2, 0);
                    this.OnMessage(InstallMessage.Progress, rec);
                }
            }

            private void TestProgressPhase2()
            {
                // With product name, estimated tick count, generating script.
                using (var rec = new Deployment.WindowsInstaller.Record(4))
                {
                    // CommonData: set CurrentProductName.
                    rec.SetInteger(1, 1);
                    rec.SetString(2, "INSTALL");
                    this.OnMessage(InstallMessage.CommonData, rec);

                    // Progress: master rest.
                    rec.SetInteger(1, 0); // Master reset.
                    rec.SetInteger(2, 50); // Total; test that 50 is added.
                    rec.SetInteger(3, 0); // Forward.
                    rec.SetInteger(4, 1); // Generating script.
                    this.OnMessage(InstallMessage.Progress, rec);

                    // ActionStart: set CurrentAction.
                    rec.FormatString = "Action: [1]: [2]";
                    rec.SetString(1, "Time");
                    rec.SetString(2, "Testing");
                    this.OnMessage(InstallMessage.ActionStart, rec);

                    // Progress: report progress.
                    rec.SetInteger(1, 2);
                    rec.SetInteger(2, 50);
                    this.OnMessage(InstallMessage.Progress, rec);

                    // Progress: expand total.
                    rec.SetInteger(1, 3);
                    rec.SetInteger(2, 50);
                    this.OnMessage(InstallMessage.Progress, rec);
                }
            }

            private void TestProgressPhase3()
            {
                // With product name, estimated tick count, executing script.
                using (var rec = new Deployment.WindowsInstaller.Record(4))
                {
                    // CommonData: set CurrentProductName.
                    rec.SetInteger(1, 1);
                    rec.SetString(2, "INSTALL");
                    this.OnMessage(InstallMessage.CommonData, rec);

                    // Progress: master reset.
                    rec.SetInteger(1, 0); // Master reset.
                    rec.SetInteger(2, 100); // Total.
                    rec.SetInteger(3, 0); // Forward.
                    rec.SetInteger(4, 0); // Executing script.
                    this.OnMessage(InstallMessage.Progress, rec);

                    // ActionStart: set CurrentAction.
                    rec.FormatString = "Action: [1]: [2]";
                    rec.SetString(1, "Time");
                    rec.SetString(2, "Testing");
                    rec.SetString(3, null);
                    this.OnMessage(InstallMessage.ActionStart, rec);

                    // Progress: update progress.
                    rec.SetInteger(1, 1); // update progress.
                    rec.SetInteger(2, 50); // Step.
                    rec.SetInteger(3, 1); // Enable ActionData.
                    this.OnMessage(InstallMessage.Progress, rec);

                    // ActionData: set CurrentActionDetail and step progress.
                    rec.FormatString = "Testing: [1]";
                    rec.SetString(1, "ActionData");
                    this.OnMessage(InstallMessage.ActionData, rec);
                }
            }

            internal sealed class ActionData : InstallCommandActionData
            {
                internal string Action { get; set; }
            }
        }
    }
}
