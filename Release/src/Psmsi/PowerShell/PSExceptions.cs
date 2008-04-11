// Exceptions extending PowerShell exceptions.
//
// Author: Heath Stewart <heaths@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.Globalization;
using System.Management;
using System.Management.Automation;
using System.Runtime.Serialization;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer.PowerShell
{
    /// <summary>
    /// Exception for an invalid value on a parameter.
    /// </summary>
    [Serializable]
    public class PSInvalidParameterException : PSArgumentException
    {
        const string MessageOverride = "PSInvalidParameterException_MessageOverride";
        string message;

        /// <summary>
        /// Creates a new instance of the <see cref="PSInvalidParameterException"/>.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The invlaid value passed to the parameter.</param>
        public PSInvalidParameterException(string name, object value) : base(null, name)
        {
            message = string.Format(Resources.Argument_InvalidParameter, name, value);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PSInvalidParameterException"/> from serialization data.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the data.</param>
        /// <param name="context">The <see cref="StreamingContext"/> for serialization.</param>
        protected PSInvalidParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            message = info.GetString(MessageOverride);
        }

        /// <summary>
        /// Populates the <see cref="SerializationInfo"/> with specific private data.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the data.</param>
        /// <param name="context">The <see cref="StreamingContext"/> for serialization.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(MessageOverride, message);
        }

        /// <summary>
        /// Gets the message describing this exception.
        /// </summary>
        /// <value>The message describing this exception.</value>
        public override string Message
        {
            get { return message; }
        }
    }
}
