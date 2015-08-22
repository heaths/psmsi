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

namespace Microsoft.Tools.WindowsInstaller
{
    /// <summary>
    /// Contains locally cached values from a <see cref="Deployment.WindowsInstaller.SummaryInfo"/> object
    /// since it requires an open handle to the storage file.
    /// </summary>
    public sealed class SummaryInfo
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SummaryInfo"/> class copying values
        /// from the <see cref="Deployment.WindowsInstaller.SummaryInfo"/> object.
        /// </summary>
        /// <param name="info">The <see cref="Deployment.WindowsInstaller.SummaryInfo"/> from which to copy values.</param>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="info"/> is null.</exception>
        internal SummaryInfo(Deployment.WindowsInstaller.SummaryInfo info)
        {
            if (null == info)
            {
                throw new ArgumentNullException("info");
            }

            this.Author = info.Author;
            this.CharacterCount = info.CharacterCount;
            this.CodePage = info.CodePage;
            this.Comments = info.Comments;
            this.CreateTime = info.CreateTime;
            this.CreatingApp = info.CreatingApp;
            this.Keywords = info.Keywords;
            this.LastPrintTime = info.LastPrintTime;
            this.LastSavedBy = info.LastSavedBy;
            this.LastSaveTime = info.LastSaveTime;
            this.PageCount = info.PageCount;
            this.RevisionNumber = info.RevisionNumber;
            this.Security = info.Security;
            this.Subject = info.Subject;
            this.Template = info.Template;
            this.Title = info.Title;
            this.WordCount = info.WordCount;
        }

        /// <summary>
        /// Gets the Author summary information property.
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// Gets the CharacterCount summary information property.
        /// </summary>
        public int CharacterCount { get; private set; }

        /// <summary>
        /// Gets the CodePage summary inforomation property.
        /// </summary>
        public short CodePage { get; private set; }

        /// <summary>
        /// Gets the Comments summary information property.
        /// </summary>
        public string Comments { get; private set; }

        /// <summary>
        /// Gets the CreateTime summary information property.
        /// </summary>
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// Gets the CreatingApp summary information property.
        /// </summary>
        public string CreatingApp { get; private set; }

        /// <summary>
        /// Gets the Keywords summary information property.
        /// </summary>
        public string Keywords { get; private set; }

        /// <summary>
        /// Gets the LastPrintTime summary information property.
        /// </summary>
        public DateTime LastPrintTime { get; private set; }

        /// <summary>
        /// Gets the LastSavedBy summary information property.
        /// </summary>
        public string LastSavedBy { get; private set; }

        /// <summary>
        /// Gets the LastSaveTime summary information property.
        /// </summary>
        public DateTime LastSaveTime { get; private set; }

        /// <summary>
        /// Gets the PageCount summary information property.
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// Gets the RevisionNumber summary information property.
        /// </summary>
        public string RevisionNumber { get; private set; }

        /// <summary>
        /// Gets the Security summary information property.
        /// </summary>
        public int Security { get; private set; }

        /// <summary>
        /// Gets the Subject summary information property.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Gets the Template summary information property.
        /// </summary>
        public string Template { get; private set; }

        /// <summary>
        /// Gets the Title summary information property.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the WordCount summary information property.
        /// </summary>
        public int WordCount { get; private set; }
    }
}
