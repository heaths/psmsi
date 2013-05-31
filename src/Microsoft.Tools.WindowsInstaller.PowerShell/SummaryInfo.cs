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
    /// Contains locally cached values from a <see cref="Deployment.WindowsInstaller.SummaryInfo"/> object
    /// since it requires an open handle to the storage file.
    /// </summary>
    public sealed class SummaryInfo
    {
        private SummaryInfo()
        {
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

        /// <summary>
        /// Creates a locally cached copy of a <see cref="Deployment.WindowsInstaller.SummaryInfo"/> object.
        /// </summary>
        /// <param name="value">The <see cref="Deployment.WindowsInstaller.SummaryInfo"/> to copy.</param>
        /// <returns>A localled cached copy of a <see cref="Deployment.WindowsInstaller.SummaryInfo"/> object.</returns>
        public static explicit operator SummaryInfo(Deployment.WindowsInstaller.SummaryInfo value)
        {
            if (null == value)
            {
                throw new ArgumentNullException("value");
            }

            return new SummaryInfo()
            {
                Author = value.Author,
                CharacterCount = value.CharacterCount,
                CodePage = value.CodePage,
                Comments = value.Comments,
                CreateTime = value.CreateTime,
                CreatingApp = value.CreatingApp,
                Keywords = value.Keywords,
                LastPrintTime = value.LastPrintTime,
                LastSavedBy = value.LastSavedBy,
                LastSaveTime = value.LastSaveTime,
                PageCount = value.PageCount,
                RevisionNumber = value.RevisionNumber,
                Security = value.Security,
                Subject = value.Subject,
                Template = value.Template,
                Title = value.Title,
                WordCount = value.WordCount,
            };
        }
    }
}
