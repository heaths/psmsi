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

using Microsoft.Deployment.WindowsInstaller.Package;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// The Get-MSIProperty cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MSIProperty", DefaultParameterSetName = ParameterSet.Path)]
    [OutputType(typeof(System.IO.FileInfo), typeof(Record))]
    public sealed class GetPropertyCommand : PackageCommandBase
    {
        private IList<WildcardPattern> propertyPatterns = null;

        /// <summary>
        /// Gets or sets the name(s) of the property to get. The default is all properties.
        /// </summary>
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string[] Property { get; set; }

        /// <summary>
        /// Gets or sets whether to attach properties to a <see cref="FileInfo"/> object and pass that through the pipeline.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Creates a list of property name wildcard patterns.
        /// </summary>
        protected override void BeginProcessing()
        {
            var count = this.Property.Count();
            if (0 < count)
            {
                this.propertyPatterns = new List<WildcardPattern>(count);
                foreach (var property in this.Property)
                {
                    var pattern = new WildcardPattern(property, WildcardOptions.Compiled | WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase);
                    this.propertyPatterns.Add(pattern);
                }
            }

            base.BeginProcessing();
        }

        /// <summary>
        /// Clears the list of property name wildcard patterns.
        /// </summary>
        protected override void EndProcessing()
        {
            this.propertyPatterns = null;
            base.EndProcessing();
        }

        /// <summary>
        /// Gets property records from the item.
        /// </summary>
        /// <param name="item">The item from which properties are retrieved.</param>
        protected override void ProcessItem(PSObject item)
        {
            var path = item.GetPropertyValue<string>("PSPath");
            var providerPath = this.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

            var db = this.OpenDatabase(providerPath);
            if (null != db)
            {
                // Keep track of which properties were selected.
                IList<string> properties = null;
                if (this.PassThru)
                {
                    properties = new List<string>();
                }

                using (db)
                {
                    TransformView transform = null;
                    if (db.Tables.Contains(TransformView.TableName))
                    {
                        transform = new TransformView(db);
                    }

                    var table = "Property";
                    if ((db is PatchPackage) && db.Tables.Contains("MsiPatchMetadata"))
                    {
                        table = "MsiPatchMetadata";
                    }

                    var query = string.Format("SELECT `Property`, `Value` FROM `{0}`", table);
                    using (var view = db.OpenView(query))
                    {
                        view.Execute();

                        // Get column information from the view before being disposed.
                        var columns = ViewManager.GetColumns(view);

                        var record = view.Fetch();
                        while (null != record)
                        {
                            using (record)
                            {
                                var name = record.GetString(1);
                                if (0 == this.propertyPatterns.Count() || name.Match(this.propertyPatterns))
                                {
                                    if (this.PassThru)
                                    {
                                        if (0 == item.Properties.Match(name).Count())
                                        {
                                            properties.Add(name);

                                            var property = new PSNoteProperty(name, record.GetString(2));
                                            item.Properties.Add(property, true);
                                        }
                                    }
                                    else
                                    {
                                        // Create a locally cached copy of the record.
                                        var copy = new Record(record, columns, transform, providerPath);
                                        var obj = PSObject.AsPSObject(copy);

                                        // Show only column properties by default.
                                        var memberSet = ViewManager.GetMemberSet(view);
                                        obj.Members.Add(memberSet, true);

                                        base.WriteObject(obj);
                                    }
                                }
                            }

                            record = view.Fetch();
                        }
                    }
                }

                if (this.PassThru)
                {
                    var propertySet = new PSPropertySet("MSIProperties", properties);
                    item.Members.Add(propertySet, true);

                    base.WriteObject(item);
                }
            }
        }
    }
}
