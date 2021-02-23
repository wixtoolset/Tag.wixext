// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using WixToolset.Data;
    using WixToolset.Data.Symbols;
    using WixToolset.Dtf.WindowsInstaller;
    using WixToolset.Extensibility;
    using WixToolset.Tag.Symbols;
    using WixToolset.Tag.Writer;

    /// <summary>
    /// The Binder for the WiX Toolset Software Id Tag Extension.
    /// </summary>
    public sealed class TagBurnBackendBinderExtension : BaseBurnBackendBinderExtension
    {
        /// <summary>
        /// Called for each extension symbol that hasn't been handled yet.
        /// </summary>
        /// <param name="section">The linked section.</param>
        /// <param name="symbol">The current symbol.</param>
        /// <returns>True if the symbol is a Bundle tag symbol.</returns>
        public override bool TryProcessSymbol(IntermediateSection section, IntermediateSymbol symbol)
        {
            if (symbol is WixBundleTagSymbol tagSymbol)
            {
                tagSymbol.Xml = CalculateTagXml(section, tagSymbol);

                var fragment = new XDocument(
                    new XElement("WixSoftwareTag",
                        new XAttribute("Filename", tagSymbol.Filename),
                        new XAttribute("Regid", tagSymbol.Regid),
                        new XCData(tagSymbol.Xml)
                        )
                    );

                this.BackendHelper.AddBootstrapperApplicationData(fragment.Root.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces));
                return true;
            }

            return false;
        }

        private static string CalculateTagXml(IntermediateSection section, WixBundleTagSymbol tagSymbol)
        {
            var bundleSymbol = section.Symbols.OfType<WixBundleSymbol>().Single();
            var uniqueId = Guid.Parse(bundleSymbol.BundleId).ToString("D");
            var bundleVersion = TagWriter.CreateFourPartVersion(bundleSymbol.Version);

            var packageTags = CollectPackageTags(section);

            var licensed = (tagSymbol.Attributes == 1);
            if (!Enum.TryParse(tagSymbol.Type, out TagType type))
            {
                type = TagType.Application;
            }

            var containedTags = CalculateContainedTagsAndType(packageTags, ref type);

            using (var ms = new MemoryStream())
            {
                TagWriter.CreateTagFile(ms, tagSymbol.Regid, uniqueId.ToUpperInvariant(), bundleSymbol.Name, bundleVersion, bundleSymbol.Manufacturer, licensed, type, containedTags);

                // Use StreamReader to "eat" the BOM if present.
                ms.Position = 0;
                using (var streamReader = new StreamReader(ms, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private static IList<SoftwareTag> CollectPackageTags(IntermediateSection section)
        {
            var tags = new List<SoftwareTag>();

            var msiPackages = section.Symbols.OfType<WixBundlePackageSymbol>().Where(s => s.Type == WixBundlePackageType.Msi).ToList();
            if (msiPackages.Any())
            {
                var payloadSymbolsById = section.Symbols.OfType<WixBundlePayloadSymbol>().ToDictionary(s => s.Id.Id);

                foreach (var msiPackage in msiPackages)
                {
                    var payload = payloadSymbolsById[msiPackage.PayloadRef];

                    using (var db = new Database(payload.SourceFile.Path))
                    {
                        using (var view = db.OpenView("SELECT `Regid`, `UniqueId`, `Type` FROM `SoftwareIdentificationTag`"))
                        {
                            view.Execute();
                            while (true)
                            {
                                using (var record = view.Fetch())
                                {
                                    if (null == record)
                                    {
                                        break;
                                    }

                                    if (!Enum.TryParse(record.GetString(3), out TagType type))
                                    {
                                        type = TagType.Unknown;
                                    }

                                    tags.Add(new SoftwareTag() { Regid = record.GetString(1), Id = record.GetString(2), Type = type });
                                }
                            }
                        }
                    }
                }
            }

            return tags;
        }

        private static IEnumerable<SoftwareTag> CalculateContainedTagsAndType(IEnumerable<SoftwareTag> allTags, ref TagType type)
        {
            var containedTags = new List<SoftwareTag>();

            foreach (var tag in allTags)
            {
                // If this tag type is an Application or Group then try to coerce our type to a Group.
                if (TagType.Application == tag.Type || TagType.Group == tag.Type)
                {
                    // If the type is still unknown, change our tag type and clear any contained tags that might have already
                    // been colllected.
                    if (TagType.Unknown == type)
                    {
                        type = TagType.Group;
                        containedTags = new List<SoftwareTag>();
                    }

                    // If we are a Group then we can add this as a contained tag, otherwise skip it.
                    if (TagType.Group == type)
                    {
                        containedTags.Add(tag);
                    }

                    // TODO: should we warn if this bundle is typed as a non-Group software id tag but is actually
                    // carrying Application or Group software tags?
                }
                else if (TagType.Component == tag.Type || TagType.Feature == tag.Type)
                {
                    // We collect Component and Feature tags only if the our tag is an Application or might still default to an Application.
                    if (TagType.Application == type || TagType.Unknown == type)
                    {
                        containedTags.Add(tag);
                    }
                }
            }

            // If our type was not set by now, we'll default to an Application.
            if (TagType.Unknown == type)
            {
                type = TagType.Application;
            }

            return containedTags;
        }
    }
}
