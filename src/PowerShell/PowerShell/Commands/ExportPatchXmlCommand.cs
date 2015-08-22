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
            var path = item.GetPropertyValue<string>("PSPath");
            path = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            string xml = Installer.ExtractPatchXmlData(path);

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
