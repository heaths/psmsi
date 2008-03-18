// Cmdlet to get or enumerator Windows Installer products.
//
// Author: Heath Stewart <heaths@microsoft.com>
// Created: Thu, 01 Feb 2007 06:55:55 GMT
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using Microsoft.Windows.Installer;
using Microsoft.Windows.Installer.PowerShell;
using Microsoft.Windows.Installer.Properties;

namespace Microsoft.Windows.Installer.PowerShell.Commands
{
    /// <summary>
    /// Abstract base class for cmdlets that call Windows Installer enumerator functions.
    /// </summary>
    /// <typeparam name="T">The type of object output to the pipeline.</typeparam>
    public abstract class EnumCommand<T> : CommandBase where T : class
    {
        /// <summary>
        /// Runs the enumeration function for the current object in the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (T obj in new MsiEnumerable<T>(EnumerateWrapper))
            {
                WritePSObject(obj);
            }
        }

        /// <summary>
        /// Abstract method for the enumeration function.
        /// </summary>
        /// <param name="index">The current index to pass to the enumeration function.</param>
        /// <param name="data">The data to be returned upon success from the enumeration function output parameters.</param>
        /// <returns>The result code of the native enumeration function. See overrides for more details.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        protected abstract int Enumerate(int index, out T data);

        /// <summary>
        /// Handles common errors returned from <see cref="Enumerate"/>.
        /// </summary>
        /// <param name="index">The current index to pass to the enumeration function.</param>
        /// <param name="data">The data to be returned upon success from the enumeration function output parameters.</param>
        /// <returns>The result code of the native enumeration function. See overrides for more details.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        private int EnumerateWrapper(int index, out T data)
        {
            int ret = Enumerate(index, out data);
            return HandleCommonErrors(ret);
        }
    }
}
