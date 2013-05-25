// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using Microsoft.Deployment.WindowsInstaller;
using System.Management.Automation;
using System.Text;
using System.Xml;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Export-MSIPatchXml cmdlet.
    /// </summary>
    [Cmdlet(VerbsData.Export, "MSIPatchXml")]
    public sealed class ExportPatchXmlCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the path to a patch package.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the file path to which the patch XML is exported.
        /// </summary>
        [Parameter(Position = 1, Mandatory = true)]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> to use to write to the file path.
        /// </summary>
        /// <remarks>
        /// You may also pass a string or code page integer form of an <see cref="Encoding"/>.
        /// </remarks>
        [Parameter]
        [Encoding, ValidateNotNullOrEmpty]
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets whether to export formatted XML.
        /// </summary>
        [Parameter]
        public SwitchParameter Formatted { get; set; }

        /// <summary>
        /// Exports the patch XML to a file using the optional encoding.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Resolve the file system path to the patch package.
            var item = this.InvokeProvider.Item.Get(this.Path).FirstOrDefault();
            string path = item.GetPropertyValue<string>("PSPath");
            path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            string xml = Installer.ExtractPatchXmlData(this.Path);

            // Load and format the XML if requested.
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var settings = new XmlWriterSettings()
            {
                Encoding = this.Encoding ?? Encoding.UTF8,
            };

            if (this.Formatted)
            {
                settings.Indent = true;
                settings.IndentChars = "\t";
            }

            // Save the XML.
            path = System.IO.Path.Combine(this.SessionState.Path.CurrentFileSystemLocation.ProviderPath, this.FilePath);
            using (var writer = XmlWriter.Create(path, settings))
            {
                doc.Save(writer);
            }
        }
    }
}
