// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains information from an Internal Consistency Evaluator (ICE).
    /// </summary>
    public sealed class IceMessage
    {
        internal IceMessage(string message)
        {
            string[] parts = message.Split('\t');

            // A valid ICE message has at least 3 parts.
            if (3 > parts.Length)
            {
                throw new ArgumentException();
            }

            this.Name = parts[0];
            this.Type = (IceMessageType)Convert.ToInt32(parts[1]);
            this.Description = parts[2];

            if (3 < parts.Length)
            {
                this.Url = parts[3];
            }

            if (4 < parts.Length)
            {
                this.Table = parts[4];
            }

            if (5 < parts.Length)
            {
                this.PrimaryKeys = new string[parts.Length - 5];
                Array.Copy(parts, 5, this.PrimaryKeys, 0, parts.Length - 5);
            }
        }

        /// <summary>
        /// Gets the name of the ICE.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the ICE.
        /// </summary>
        public IceMessageType Type { get; private set; }

        /// <summary>
        /// Gets the description of the ICE.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the URL to more information about the ICE.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the name of the table where the ICE was found.
        /// </summary>
        public string Table { get; private set; }

        /// <summary>
        /// Gets the primary keys of the table row where the ICE was found.
        /// </summary>
        public string[] PrimaryKeys { get; private set; }
    }

    /// <summary>
    /// The type of the <see cref="IceMessage"/>.
    /// </summary>
    public enum IceMessageType
    {
        /// <summary>
        /// Failure message reporting the failure of the ICE custom action.
        /// </summary>
        Failure,

        /// <summary>
        /// Error message reporting database authoring that cause incorrect behavior.
        /// </summary>
        Error,

        /// <summary>
        /// Warning message reporting database authoring that causes incorrect behavior in certain cases.
        /// Warnings can also report unexpected side-effects of database authoring.
        /// </summary>
        Warning,

        /// <summary>
        /// Informational message.
        /// </summary>
        Information,
    }
}
