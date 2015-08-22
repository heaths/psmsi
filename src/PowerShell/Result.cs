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
    /// The results of an operation on a product or patch package.
    /// </summary>
    public sealed class Result
    {
        private bool _rebootInitiated;
        private bool _rebootRequired;

        /// <summary>
        /// Creates a new instance of the <see cref="Result"/> class.
        /// </summary>
        internal Result()
        {
            this._rebootInitiated = false;
            this._rebootRequired = false;
        }

        /// <summary>
        /// Gets whether a reboot has been initiated already.
        /// </summary>
        public bool RebootInitiated
        {
            get { return this._rebootInitiated; }
            internal set
            {
                this._rebootInitiated |= value;
                this._rebootRequired |= value;
            }
        }

        /// <summary>
        /// Gets whether a reboot is required or has been initiated already.
        /// </summary>
        public bool RebootRequired
        {
            get { return this._rebootRequired; }
            internal set { this._rebootRequired |= value; }
        }

        /// <summary>
        /// Combines results while preserving their semantics.
        /// </summary>
        /// <param name="x">The first <see cref="Result"/> to combine.</param>
        /// <param name="y">The second <see cref="Result"/> to combine.</param>
        /// <returns>A combined <see cref="Result"/> from <paramref name="x"/> and <paramref name="y"/>.</returns>
        public static Result operator |(Result x, Result y)
        {
            if (null == x)
            {
                throw new ArgumentNullException("x");
            }

            if (null == y)
            {
                throw new ArgumentNullException("y");
            }

            var result = new Result();

            result._rebootInitiated = x._rebootInitiated | y._rebootInitiated;
            result._rebootRequired = result._rebootInitiated | x._rebootRequired | y._rebootRequired;

            return result;
        }
    }
}
