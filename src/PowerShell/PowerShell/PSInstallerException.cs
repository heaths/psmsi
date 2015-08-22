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

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Globalization;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Tools.WindowsInstaller.PowerShell
{
    /// <summary>
    /// Exception for Windows Installer errors.
    /// </summary>
    [Serializable]
    public sealed class PSInstallerException : Exception, IContainsErrorRecord, IDisposable
    {
        private static readonly string ErrorId = "InstallerError";
        private static readonly string FieldCount = "msiFieldCount";
        private static readonly string FieldPrefix = "msiField";

        private ErrorRecord errorRecord;
        private Deployment.WindowsInstaller.Record record;

        /// <summary>
        /// Creates a <see cref="PSInstallerException"/> from the given <paramref name="record"/>.
        /// </summary>
        /// <param name="record">The <see cref="Deployment.WindowsInstaller.Record"/> containing error details.</param>
        public PSInstallerException(Deployment.WindowsInstaller.Record record)
        {
            this.errorRecord = null;
            this.record = record;
        }

        /// <summary>
        /// Creates a <see cref="PSInstallerException"/> from the given inner exception.
        /// </summary>
        /// <param name="innerException">The <see cref="InstallerException"/> containing error details.</param>
        public PSInstallerException(InstallerException innerException) : base(null, innerException)
        {
            this.errorRecord = null;
            this.record = null;
        }

        private PSInstallerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // ErrorRecord is cache only.
            this.errorRecord = null;

            int fieldCount = (int)info.GetValue(PSInstallerException.FieldCount, typeof(int));
            if (0 < fieldCount)
            {
                this.record = new Deployment.WindowsInstaller.Record(fieldCount);
                for (int i = 0; i <= fieldCount; ++i)
                {
                    string name = PSInstallerException.FieldPrefix + i.ToString(CultureInfo.InvariantCulture);
                    string value = (string)info.GetValue(name, typeof(string));

                    this.record.SetString(i, value);
                }
            }
            else
            {
                this.record = null;
            }
        }

        /// <summary>
        /// Disposes an instance of <see cref="PSInstallerException"/>.
        /// </summary>
        public void Dispose()
        {
            if (null != this.record)
            {
                this.record.Dispose();
            }
        }

        /// <summary>
        /// Serializes the error information.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> into which data is serialized.</param>
        /// <param name="context">a <see cref="StreamingContext"/> that describes the target of serialization.</param>
        [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var record = this.GetRecord();
            if (null != record)
            {
                info.AddValue(PSInstallerException.FieldCount, record.FieldCount, typeof(int));
                for (int i = 0; i <= this.record.FieldCount; ++i)
                {
                    string name = PSInstallerException.FieldPrefix + i.ToString(CultureInfo.InvariantCulture);
                    info.AddValue(name, record.GetString(i), typeof(string));
                }
            }
            else
            {
                info.AddValue(PSInstallerException.FieldCount, 0, typeof(int));
            }
        }

        /// <summary>
        /// Gets the <see cref="ErrorRecord"/> containing more information for PowerShell.
        /// </summary>
        public ErrorRecord ErrorRecord
        {
            get
            {
                if (null == this.errorRecord)
                {
                    // Attempt to create and cache the ErrorRecord.
                    var record = this.GetRecord();
                    if (null != record)
                    {
                        string resource;
                        var category = PSInstallerException.GetErrorCategory(record, out resource);

                        this.errorRecord = new ErrorRecord(this, PSInstallerException.ErrorId, category, resource);
                    }
                    else
                    {
                        this.errorRecord = new ErrorRecord(this, PSInstallerException.ErrorId, ErrorCategory.NotSpecified, null);
                    }
                }

                return this.errorRecord;
            }
        }

        /// <summary>
        /// Gets the error message from Windows Installer.
        /// </summary>
        public override string Message
        {
            get
            {
                var record = this.GetRecord();
                if (null != record)
                {
                    // Attempt to format the message using the current UI culture.
                    return Installer.GetErrorMessage(record, CultureInfo.CurrentUICulture);
                }
                else if (null != this.InnerException)
                {
                    return this.InnerException.Message;
                }
                else
                {
                    return base.Message;
                }
            }
        }

        private Deployment.WindowsInstaller.Record GetRecord()
        {
            if (null != this.record)
            {
                return this.record;
            }
            else if (this.InnerException is InstallerException)
            {
                var ex = (InstallerException)this.InnerException;
                return ex.GetErrorRecord();
            }

            return null;
        }

        private static ErrorCategory GetErrorCategory(Deployment.WindowsInstaller.Record record, out string resource)
        {
            resource = null;

            if (1 < record.FieldCount)
            {
                int code = record.GetInteger(1);
                if (1000 <= code && code < 25000)
                {
                    // Specifically handle common errors.
                    switch (code)
                    {
                        case 1301:
                        case 1304:
                            resource = record.GetString(2);
                            return ErrorCategory.WriteError;

                        case 1303:
                        case 1306:
                        case 1718:
                            resource = record.GetString(2);
                            return ErrorCategory.PermissionDenied;

                        case 1305:
                            resource = record.GetString(2);
                            return ErrorCategory.ReadError;

                        case 1308:
                        case 1334:
                            resource = record.GetString(2);
                            return ErrorCategory.ResourceUnavailable;

                        case 1706:
                            resource = record.GetString(2);
                            return ErrorCategory.ResourceUnavailable;

                        case 1715:
                        case 1716:
                        case 1717:
                            resource = record.GetString(2);
                            return ErrorCategory.NotSpecified;

                        case 1935:
                        case 1937:
                            resource = record.GetString(6);
                            return ErrorCategory.InvalidData;
                    }
                }
            }

            return ErrorCategory.NotSpecified;
        }
    }
}
