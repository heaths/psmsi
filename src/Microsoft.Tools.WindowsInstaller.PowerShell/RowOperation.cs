// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Operations performed on a row.
    /// </summary>
    public enum RowOperation
    {
        /// <summary>
        /// No operation performed on the row.
        /// </summary>
        None,

        /// <summary>
        /// The row was modified.
        /// </summary>
        Modify,

        /// <summary>
        /// The row was inserted.
        /// </summary>
        Insert,

        /// <summary>
        /// The row was deleted.
        /// </summary>
        Delete,
    }
}
