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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstaller.Properties;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Base class for product and patch install commands.
    /// </summary>
    /// <typeparam name="T">A derivative of <see cref="InstallCommandActionData"/>.</typeparam>
    public abstract class InstallCommandBase<T> : PSCmdlet
        where T : InstallCommandActionData, new()
    {
        /// <summary>
        /// The default install level.
        /// </summary>
        protected const int INSTALLLEVEL_DEFAULT = 0;

        private Log log;
        private Result result;
        private ProgressDataCollection progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallCommandBase{T}"/> class.
        /// </summary>
        protected InstallCommandBase()
        {
            this.log = null;
            this.result = null;

            this.Actions = new ActionQueue();
            this.progress = new ProgressDataCollection();
        }

        /// <summary>
        /// Gets or sets the path supporting wildcards to enumerate files.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.Path, Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the literal path for one or more files.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSet.LiteralPath, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("PSPath")]
        public string[] LiteralPath
        {
            get { return this.Path; }
            set { this.Path = value; }
        }

        /// <summary>
        /// Gets or sets the logging path.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Log { get; set; }

        /// <summary>
        /// Gets or sets the remaining arguments as strings.
        /// </summary>
        [Parameter(ValueFromRemainingArguments = true)]
        public string[] Properties { get; set; }

        /// <summary>
        /// Gets or sets whether to queue and chain all install operations together.
        /// </summary>
        /// <remarks>
        /// <para>By default, cmdlets will process each product install request as it comes in through the pipeline.
        /// Set this switch parameter to queue them all up and chain them as seemingly one install.</para>
        /// <para>This will create a single system restore point (if supported) for all packages
        /// and suppress reboots (unless forced within a package).</para>
        /// </remarks>
        [Parameter]
        public SwitchParameter Chain { get; set; }

        /// <summary>
        /// Gets or sets whether any prompts should be suppressed.
        /// </summary>
        [Parameter]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Gets or sets the name of the variable to store the <see cref="Result"/>.
        /// </summary>
        /// <remarks>
        /// You may optionally prefix the name with a "+" to combine the new results with an existing variable of the same name.
        /// </remarks>
        [Parameter]
        [ValidateVariableName]
        public string ResultVariable { get; set; }

        /// <summary>
        /// Gets the queued actions to perform for installation.
        /// </summary>
        protected ActionQueue Actions { get; private set; }

        /// <summary>
        /// Gets the activity being performed.
        /// </summary>
        protected abstract string Activity { get; }

        /// <summary>
        /// Gets the <see cref="RestorePointType"/> of the current operation.
        /// </summary>
        internal virtual RestorePointType Operation
        {
            get { return RestorePointType.ApplicationInstall; }
        }

        /// <summary>
        /// Called to allow child classes to execute based on the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="InstallCommandActionData"/> on which to execute.</param>
        protected abstract void ExecuteAction(T data);

        /// <summary>
        /// Called to allow child classes to queue <see cref="InstallCommandActionData"/> into <see cref="Actions"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation gets the fully qualified path(s) of file(s) specified as arguments or piped into the command.
        /// </remarks>
        protected abstract void QueueActions();

        /// <summary>
        /// Allows child classes to update the action data without changing the default action queueing.
        /// </summary>
        /// <param name="data">The <see cref="InstallCommandActionData"/> to update.</param>
        protected virtual void UpdateAction(T data)
        {
            data.UpdateWeight();
        }

        /// <summary>
        /// Sets up the internal and external UI handlers.
        /// </summary>
        protected override void BeginProcessing()
        {
            // Resolve the requested log path, if specified.
            string path = this.Log;
            if (!string.IsNullOrEmpty(path))
            {
                string pwd = this.SessionState.Path.CurrentFileSystemLocation.ProviderPath;
                path = System.IO.Path.Combine(pwd, path);
            }

            // Initialize logging.
            this.log = new Log(path);

            // Initialize the result to write to the pipeline at the end.
            this.result = new Result();

            base.BeginProcessing();
        }

        /// <summary>
        /// Allows child classes to queue actions and executes them if <see cref="Chain"/> is not set.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Queue the action.
            this.QueueActions();

            // Execute the action if Chain is not set.
            if (!this.Chain)
            {
                this.ExecuteActions();
            }

            base.ProcessRecord();
        }

        /// <summary>
        /// Executes the queued actions if <see cref="Chain"/> is set and restores the previous internal and external UI handlers.
        /// </summary>
        protected override void EndProcessing()
        {
            // Execute the actions if Chain is set.
            if (this.Chain)
            {
                this.ExecuteActions();
            }

            // Write the final result to the pipeline for administrators.
            if (null != this.result)
            {
                this.SetResultVariable();
                this.result = null;
            }

            base.EndProcessing();
        }

        /// <summary>
        /// Handles a message <see cref="Deployment.WindowsInstaller.Record"/> from Windows Installer.
        /// </summary>
        /// <param name="messageType">The <see cref="InstallMessage"/> type of the message.</param>
        /// <param name="messageRecord">The <see cref="Deployment.WindowsInstaller.Record"/> containing more information.</param>
        /// <param name="buttons">the <see cref="MessageButtons"/> to display.</param>
        /// <param name="icon">The <see cref="MessageIcon"/> to display.</param>
        /// <param name="defaultButton">The <see cref="MessageDefaultButton"/> to display.</param>
        /// <returns>The <see cref="MessageResult"/> that informs Windows Installer was action to take in response to the message.</returns>
        protected MessageResult OnMessage(InstallMessage messageType, Deployment.WindowsInstaller.Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            // Log all messages in debug mode.
            this.WriteMessage(messageType, messageRecord);

            switch (messageType)
            {
                case InstallMessage.Initialize:
                    return this.OnInitialize(messageRecord);

                case InstallMessage.ActionStart:
                    return this.OnActionStart(messageRecord);

                case InstallMessage.ActionData:
                    return this.OnActionData(messageRecord);

                case InstallMessage.Error:
                case InstallMessage.FatalExit:
                case InstallMessage.OutOfDiskSpace:
                    return this.OnError(messageRecord);

                case InstallMessage.Warning:
                    return this.OnWarning(messageRecord);

                case InstallMessage.Info:
                case InstallMessage.User:
                    return this.OnVerbose(messageRecord);

                case InstallMessage.Progress:
                    return this.OnProgress(messageRecord);

                case InstallMessage.CommonData:
                    return this.OnCommonData(messageRecord);
            }

            return MessageResult.None;
        }

        /// <summary>
        /// Initializes the execution state.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing additional information.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnInitialize(Deployment.WindowsInstaller.Record record)
        {
            this.Reset();
            return MessageResult.OK;
        }

        /// <summary>
        /// Called when a new action starts.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing additional information.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnActionStart(Deployment.WindowsInstaller.Record record)
        {
            if (this.progress.IsValid)
            {
                var current = this.progress.Current;

                if (current.EnableActionData)
                {
                    current.EnableActionData = false;
                }

                if (current.GeneratingScript)
                {
                    this.progress.CurrentAction = Resources.Action_GeneratingScript;
                }
                else if (null != record && 2 < record.FieldCount)
                {
                    var action = record.GetString(1);
                    Debug.Assert(!string.IsNullOrEmpty(action), "Action should not be null at this point");

                    this.progress.CurrentAction = record.GetString(2);
                    this.progress.CurrentActionTemplate = record.GetString(3);

                    // Use current culture resources if ActionText is missing or doesn't match the current culture.
                    var culture = this.progress.CurrentCulture ?? CultureInfo.InvariantCulture;
                    if (string.IsNullOrEmpty(this.progress.CurrentAction) || !culture.Equals(CultureInfo.CurrentCulture))
                    {
                        this.progress.CurrentAction = ActionText.GetActionText(action) ?? string.Empty;
                    }

                    if (string.IsNullOrEmpty(this.progress.CurrentActionTemplate) || !culture.Equals(CultureInfo.CurrentCulture))
                    {
                        this.progress.CurrentActionTemplate = ActionText.GetActionData(action);
                    }
                }
            }

            return MessageResult.OK;
        }

        /// <summary>
        /// Called when an action sends more information.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing additional information.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnActionData(Deployment.WindowsInstaller.Record record)
        {
            if (this.progress.IsValid)
            {
                // Step the current progress completed.
                var current = this.progress.Current;

                if (current.EnableActionData)
                {
                    if (current.Forward)
                    {
                        current.Complete += current.Step;
                    }
                    else
                    {
                        current.Complete -= current.Step;
                    }
                }

                // Set the current action message when executing the script.
                if (null != record && !current.GeneratingScript)
                {
                    if (!string.IsNullOrEmpty(this.progress.CurrentActionTemplate))
                    {
                        record.FormatString = this.progress.CurrentActionTemplate;
                    }

                    this.progress.CurrentActionDetail = record.ToString();
                }

                this.WriteProgress();
            }

            return MessageResult.OK;
        }

        /// <summary>
        /// Writes and error to the pipeline.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing the error details.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnError(Deployment.WindowsInstaller.Record record)
        {
            if (null == record)
            {
                return MessageResult.None;
            }
            else if (0 < record.FieldCount)
            {
                // Ignore certain errors.
                int code = record.GetInteger(1);
                switch (code)
                {
                    case 1605:
                        // Continue even if there isn't enough disk space for rollback.
                        return MessageResult.Ignore;

                    case 1704:
                        // Roll back suspended installs so we can continue.
                        return MessageResult.OK;
                }
            }

            using (var ex = new PSInstallerException(record))
            {
                if (null != ex.ErrorRecord)
                {
                    this.WriteError(ex.ErrorRecord);
                }
            }

            return MessageResult.OK;
        }

        /// <summary>
        /// Writes a warning to the pipeline.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing the warning details.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnWarning(Deployment.WindowsInstaller.Record record)
        {
            if (null != record)
            {
                string message = record.ToString();
                this.WriteWarning(message);
            }

            return MessageResult.OK;
        }

        /// <summary>
        /// Writes a verbose message to the pipeline.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing the verbose message details.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnVerbose(Deployment.WindowsInstaller.Record record)
        {
            if (null != record)
            {
                string message = record.ToString();
                this.WriteVerbose(message);
            }

            return MessageResult.OK;
        }

        /// <summary>
        /// Writes progress information.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing the progress details.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnProgress(Deployment.WindowsInstaller.Record record)
        {
            if (null == record || 2 > record.FieldCount)
            {
                return MessageResult.None;
            }

            switch (record.GetInteger(1))
            {
                // Add a new phase of progress information.
                case 0:
                    if (4 > record.FieldCount)
                    {
                        // Invalid message.
                        break;
                    }

                    // Add a new phase of progress.
                    var current = this.progress.Add();

                    current.Forward = 0 == record.GetInteger(3);
                    current.Total = record.GetInteger(2);
                    current.Complete = current.Forward ? 0 : current.Total;
                    current.EnableActionData = false;
                    current.GeneratingScript = 1 == record.GetInteger(4);

                    // Windows Installer team advises to add ~50 ticks to script generation.
                    if (0 == this.progress.CurrentIndex)
                    {
                        current.Total += 50;
                    }

                    break;

                // Update progress information.
                case 1:
                    if (3 > record.FieldCount)
                    {
                        // Invalid message.
                        break;
                    }
                    else if (!this.progress.IsValid)
                    {
                        // Progress not initialized.
                        break;
                    }

                    if (0 == record.GetInteger(3))
                    {
                        this.progress.Current.EnableActionData = false;
                    }
                    else
                    {
                        this.progress.Current.EnableActionData = true;
                        this.progress.Current.Step = record.GetInteger(2);
                    }

                    break;

                // Report progress information.
                case 2:
                    if (!this.progress.IsValid || 0 == this.progress.Current.Total)
                    {
                        // Progress not initialized.
                        break;
                    }

                    // Adjust the current progress.
                    if (this.progress.Current.Forward)
                    {
                        this.progress.Current.Complete += record.GetInteger(2);
                    }
                    else
                    {
                        this.progress.Current.Complete -= record.GetInteger(2);
                    }

                    break;

                // Expand the total.
                case 3:
                    if (!this.progress.IsValid)
                    {
                        // Progress not initialized.
                        break;
                    }

                    // Expand the progress maximum.
                    this.progress.Current.Total += record.GetInteger(2);
                    break;

                default:
                    return MessageResult.None;
            }

            this.WriteProgress();
            return MessageResult.OK;
        }

        /// <summary>
        /// Provides information about the install session.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing common details.</param>
        /// <returns>The result code indicating how Windows Installer should proceed.</returns>
        protected MessageResult OnCommonData(Deployment.WindowsInstaller.Record record)
        {
            // Get the current session language and Windows Installer-generated caption.
            if (1 < record.FieldCount)
            {
                var subtype = record.GetInteger(1);
                if (0 == subtype)
                {
                    var langId = record.GetInteger(2);
                    if (0 != langId)
                    {
                        this.progress.CurrentCulture = CultureInfo.GetCultureInfo(langId);
                    }
                }
                else if (1 == subtype)
                {
                    this.progress.CurrentName = record.GetString(2);
                }
            }

            return MessageResult.OK;
        }

        private void ExecuteActions()
        {
            using (new UserInterfaceHandler(this.OnMessage, this.Force))
            {
                // Keep track of the total weight for all queued actions.
                this.Actions.OriginalWeight = this.Actions.Sum(data => data.Weight);

                // Create a single restore point for chained packages.
                SystemRestorePoint restorePoint = null;
                if (this.Chain)
                {
                    try
                    {
                        restorePoint = SystemRestorePoint.Create(this.Operation);
                    }
                    catch (Win32Exception ex)
                    {
                        var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_NoRestorePoint, ex.Message);
                        this.WriteWarning(message);
                    }
                }

                // Execute the actions.
                while (0 < this.Actions.Count)
                {
                    try
                    {
                        T data = this.Actions.Dequeue();
                        this.progress.CurrentWeight = data.Weight;

                        string extra = null != data ? data.LogName : null;
                        Installer.EnableLog(this.log.Mode, this.log.Next(extra));

                        // If a system restore point was successfully created,
                        // disable creating restore points for each package.
                        if (null != restorePoint)
                        {
                            data.CommandLine += " MSIFASTINSTALL=1";
                        }

                        // Suppress reboots when possible if chaining.
                        if (this.Chain)
                        {
                            data.CommandLine += " REBOOT=ReallySuppress";
                        }

                        this.ExecuteAction(data);
                    }
                    catch (InstallerException ex)
                    {
                        using (var psiex = new PSInstallerException(ex))
                        {
                            if (null != psiex.ErrorRecord)
                            {
                                this.WriteError(psiex.ErrorRecord);
                            }
                            else
                            {
                                // Unexpected not to have an ErrorRecord.
                                throw;
                            }
                        }
                    }
                    finally
                    {
                        if (null != this.result)
                        {
                            this.result.RebootInitiated = Installer.RebootInitiated;
                            this.result.RebootRequired = Installer.RebootRequired;
                        }
                    }
                }

                // Complete an existing restore point.
                if (null != restorePoint)
                {
                    restorePoint.Commit();
                }

                // Make sure progress is completed.
                this.WriteProgress(true);

                // Warn the user if a restart is required.
                if (Installer.RebootRequired)
                {
                    var ex = new Win32Exception(3010);
                    this.WriteWarning(ex.Message);
                }
            }
        }

        private void Reset()
        {
            // Reset progress.
            this.progress.Reset();
        }

        private void SetResultVariable()
        {
            var append = false;
            var name = this.ResultVariable;

            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith(ValidateVariableNameAttribute.AppendPrefix))
                {
                    append = true;
                    name = name.Substring(1);
                }
            }

            PSVariable variable = null;
            Result value = null;

            if (!string.IsNullOrEmpty(name))
            {
                if (append)
                {
                    variable = this.SessionState.PSVariable.Get(name);
                }

                if (null == variable)
                {
                    variable = new PSVariable(name, null, ScopedItemOptions.AllScope);
                }
                else
                {
                    value = variable.Value as Result;
                }

                if (null == value)
                {
                    value = this.result;
                }
                else if (null != this.result)
                {
                    value |= this.result;
                }

                variable.Value = value ?? new Result();

                // Let exceptions throw: user error.
                this.SessionState.PSVariable.Set(variable);
            }
        }

        [Conditional("DEBUG")]
        private void WriteMessage(InstallMessage type, Deployment.WindowsInstaller.Record record)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0} ({0:d})", type);

            if (null != record)
            {
                sb.Append(": ");

                // Show field 0 (format string) as well.
                for (int i = 0; i <= record.FieldCount; ++i)
                {
                    if (0 != i)
                    {
                        sb.Append(", ");
                    }

                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}", i, record.GetString(i));
                }
            }

            this.WriteDebug(sb.ToString());
        }

        private void WriteProgress(bool complete = false)
        {
            // Show operation and product name (if available yet).
            var activity = string.Format(CultureInfo.CurrentCulture, this.Activity, this.progress.CurrentName).TrimEnd();

            // Make sure there's always a status description.
            if (string.IsNullOrEmpty(this.progress.CurrentAction))
            {
                this.progress.CurrentAction = Resources.Action_Wait;
            }

            var record = new ProgressRecord(this.GetHashCode(), activity, this.progress.CurrentAction);
            if (complete)
            {
                record.RecordType = ProgressRecordType.Completed;
            }
            else
            {
                record.RecordType = ProgressRecordType.Processing;

                // Show details.
                if (!string.IsNullOrEmpty(this.progress.CurrentActionDetail))
                {
                    record.CurrentOperation = this.progress.CurrentActionDetail;
                }

                // Calculate progress for all completed actions. Current action was already dequeued so subtract its weight.
                var completed = Math.Max(0, this.Actions.OriginalWeight - this.Actions.RemainingWeight - this.progress.CurrentWeight);

                // Calculate the remaining progress for the current action.
                if (this.progress.IsValid)
                {
                    var factor = this.progress.CurrentPercentage / 100f;
                    completed += (int)(factor * this.progress.CurrentWeight);
                }

                var percentage = 100 * completed / this.Actions.OriginalWeight;
                record.PercentComplete = Math.Min(100, (int)percentage);
            }

            this.WriteProgress(record);
        }

        /// <summary>
        /// Action queue.
        /// </summary>
        protected internal sealed class ActionQueue : Queue<T>
        {
            private int previousCount = -1;
            private long previousWeight = 0;

            /// <summary>
            /// Gets the total original weight of all actions before processing.
            /// </summary>
            public long OriginalWeight { get; internal set; }

            /// <summary>
            /// Gets the total weight of all remaining actions in the queue.
            /// </summary>
            public long RemainingWeight
            {
                get
                {
                    if (this.Count != this.previousCount)
                    {
                        this.previousCount = this.Count;
                        this.previousWeight = this.Sum(data => data.Weight);
                    }

                    return this.previousWeight;
                }
            }
        }

        private sealed class ProgressData
        {
            internal int Total = 0;
            internal int Step = 0;
            internal int Complete = 0;
            internal bool Forward = true;
            internal bool GeneratingScript = false;
            internal bool EnableActionData = true;
        }

        private sealed class ProgressDataCollection : List<ProgressData>
        {
            private static readonly int[] Weights = { 15, 80, 5 };

            internal ProgressData Add()
            {
                var data = new ProgressData();
                this.Add(data);

                return data;
            }

            internal ProgressData Current
            {
                get { return this[this.CurrentIndex]; }
            }

            internal string CurrentAction { get; set; }
            internal string CurrentActionTemplate { get; set; }
            internal string CurrentActionDetail { get; set; }

            internal int CurrentIndex
            {
                get { return this.Count - 1; }
            }

            internal CultureInfo CurrentCulture { get; set; }
            internal string CurrentName { get; set; }

            internal int CurrentPercentage
            {
                get
                {
                    int percent = 0;

                    for (int i = 0; i < this.Count && i < ProgressDataCollection.Weights.Length; ++i)
                    {
                        if (i < this.CurrentIndex)
                        {
                            // If the phase is completed use the entire weight.
                            percent += ProgressDataCollection.Weights[i];
                        }
                        else if (i == this.CurrentIndex && 0 < this.Current.Total)
                        {
                            // Calculate the remaining progress for this phase.
                            percent += this.Current.Complete * ProgressDataCollection.Weights[i] / this.Current.Total;
                        }
                    }

                    return Math.Min(100, percent);
                }
            }

            internal long CurrentWeight { get; set; }

            internal bool IsValid
            {
                get { return 0 < this.Count; }
            }

            internal void Reset()
            {
                this.Clear();
                this.CurrentAction = Resources.Action_Wait;
                this.CurrentActionTemplate = null;
                this.CurrentActionDetail = null;
                this.CurrentCulture = CultureInfo.CurrentCulture;

                // Default to empty name so "(null)" doesn't show for activity.
                this.CurrentName = string.Empty;
            }
        }

        private sealed class UserInterfaceHandler : IDisposable
        {
            private InstallUIOptions previousInternalUI;
            private ExternalUIRecordHandler previousExternalUI;

            internal UserInterfaceHandler(ExternalUIRecordHandler handler, bool force)
            {
                var internalUI = InstallUIOptions.Silent;
                if (force)
                {
                    internalUI |= InstallUIOptions.SourceResolutionOnly | InstallUIOptions.UacOnly;
                }

                var externalUI = InstallLogModes.ActionStart | InstallLogModes.ActionData | InstallLogModes.CommonData | InstallLogModes.Error
                               | InstallLogModes.FatalExit | InstallLogModes.Info | InstallLogModes.Initialize | InstallLogModes.Progress
                               | InstallLogModes.User | InstallLogModes.Verbose | InstallLogModes.Warning;

                // Set up the internal and external UI handling.
                this.previousInternalUI = Installer.SetInternalUI(internalUI);
                this.previousExternalUI = Installer.SetExternalUI(handler, externalUI);
            }

            public void Dispose()
            {
                // Restore previous handlers.
                Installer.SetInternalUI(this.previousInternalUI);
                Installer.SetExternalUI(this.previousExternalUI, InstallLogModes.None);
            }
        }
    }
}
