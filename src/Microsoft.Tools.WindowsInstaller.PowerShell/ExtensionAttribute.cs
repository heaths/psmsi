// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Identifies that an attributed assembly, class, or method contains extension methods.
    /// </summary>
    /// <remarks>
    /// Surprisingly redeclaring this attribute with the same name works for a .NET 2.0 project.
    /// I have it on good authority that, in general, the compiler does not verify the full type declaration.
    /// </remarks>
    [AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class ExtensionAttribute : Attribute
    {
    }
}
