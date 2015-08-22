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
using System.Globalization;

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
            this.Type = (IceMessageType)Convert.ToInt32(parts[1], CultureInfo.InvariantCulture);
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
                this.Column = parts[5];
            }

            if (6 < parts.Length)
            {
                this.PrimaryKeys = new string[parts.Length - 6];
                Array.Copy(parts, 6, this.PrimaryKeys, 0, this.PrimaryKeys.Length);
            }
        }

        /// <summary>
        /// Gets the path to the database for which the ICE was reported.
        /// </summary>
        public string Path { get; internal set; }

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
        /// Gets the name of the column where the ICE was found.
        /// </summary>
        public string Column { get; private set; }

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
