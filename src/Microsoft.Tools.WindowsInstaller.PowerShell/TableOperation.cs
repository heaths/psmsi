// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Operations performed on a table.
    /// </summary>
    public enum TableOperation
    {
        /// <summary>
        /// No operation performed on the table.
        /// </summary>
        None,

        /// <summary>
        /// The table or its rows were modified.
        /// </summary>
        Modify,

         /// <summary>
        /// The table was created.
        /// </summary>
        Create,

       /// <summary>
        /// The table was dropped.
        /// </summary>
        Drop,
    }
}
