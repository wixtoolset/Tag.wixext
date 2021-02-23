// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using WixToolset.Data;
    using WixToolset.Data.Symbols;
    using WixToolset.Data.WindowsInstaller;
    using WixToolset.Extensibility;
    using WixToolset.Extensibility.Data;
    using WixToolset.Extensibility.Services;
    using WixToolset.Tag.Symbols;
    using WixToolset.Tag.Writer;

    /// <summary>
    /// The Windows Installer backend enhancements for the WiX Toolset Software Id Tag Extension.
    /// </summary>
    public sealed class TagWindowsInstallerBackendBinderExtension : BaseWindowsInstallerBackendBinderExtension, IResolverExtension
    {
        public override IEnumerable<TableDefinition> TableDefinitions => TagTableDefinitions.All;

        private IBackendHelper backendHelper;
        private string workingFolder;

        /// <summary>
        /// Ensures the first draft of the SWID Tag exists on disk when being included
        /// in MSI packages.
        /// </summary>
        /// <param name="context">Resolve context.</param>
        public void PreResolve(IResolveContext context)
        {
            var section = context.IntermediateRepresentation.Sections.FirstOrDefault();

            // Only process MSI packages.
            if (SectionType.Product != section?.Type)
            {
                return;
            }

            this.backendHelper = context.ServiceProvider.GetService<IBackendHelper>();

            this.workingFolder = CalculateWorkingFolder(context.IntermediateFolder);

            // Ensure any tag files are generated to be imported by the MSI.
            var tagSymbols = this.CreateProductTagFiles(section);

            var tagSymbol = tagSymbols.FirstOrDefault();
            if (tagSymbol != null)
            {
                // If we created any tag files, add a WixVariable to map to the regid.
                section.AddSymbol(new WixVariableSymbol(tagSymbol.SourceLineNumbers, new Identifier(AccessModifier.Private, "WixTagRegid"))
                {
                    Value = tagSymbol.Regid,
                    Overridable = false
                });
            }
        }

        /// <summary>
        /// Unused.
        /// </summary>
        public IResolveFileResult ResolveFile(string source, IntermediateSymbolDefinition symbolDefinition, SourceLineNumber sourceLineNumbers, BindStage bindStage) => null;

        /// <summary>
        /// Unused.
        /// </summary>
        public void PostResolve(IResolveResult result) { }

        /// <summary>
        /// Called before database binding occurs.
        /// </summary>
        public override void PreBackendBind(IBindContext context)
        {
            base.PreBackendBind(context);

            this.backendHelper = base.BackendHelper;

            this.workingFolder = CalculateWorkingFolder(context.IntermediateFolder);
        }

        /// <summary>
        /// Called after database variable resolution occurs.
        /// </summary>
        public override void SymbolsFinalized(IntermediateSection section)
        {
            // Only process MSI packages.
            if (section.Type != SectionType.Product)
            {
                return;
            }

            var tagSymbols = this.CreateProductTagFiles(section);

            foreach (var tagSymbol in tagSymbols)
            {
                section.Symbols.Add(tagSymbol);
            }
        }

        private IEnumerable<SoftwareIdentificationTagSymbol> CreateProductTagFiles(IntermediateSection section)
        {
            var productTags = section.Symbols.OfType<WixProductTagSymbol>().ToList();
            if (!productTags.Any())
            {
                return null;
            }

            string productCode = null;
            string productName = null;
            string productVersion = null;
            string manufacturer = null;

            foreach (var property in section.Symbols.OfType<PropertySymbol>())
            {
                switch (property.Id.Id)
                {
                    case "ProductCode":
                        productCode = property.Value;
                        break;
                    case "ProductName":
                        productName = property.Value;
                        break;
                    case "ProductVersion":
                        productVersion = TagWriter.CreateFourPartVersion(property.Value);
                        break;
                    case "Manufacturer":
                        manufacturer = property.Value;
                        break;
                }
            }

            // If the ProductCode is available, only keep it if it is a GUID.
            if (!String.IsNullOrEmpty(productCode))
            {
                if (productCode.Equals("*"))
                {
                    productCode = null;
                }
                else if (Guid.TryParse(productCode, out var guid))
                {
                    productCode = guid.ToString("D").ToUpperInvariant();
                }
                else // not a GUID, erase it.
                {
                    productCode = null;
                }
            }

            Directory.CreateDirectory(this.workingFolder);

            var fileSymbols = section.Symbols.OfType<FileSymbol>().ToList();
            var swidSymbols = new Dictionary<string, SoftwareIdentificationTagSymbol>();

            foreach (var tagSymbol in productTags)
            {
                var tagFileRef = tagSymbol.FileRef;
                var licensed = (tagSymbol.Attributes == 1);
                var uniqueId = String.IsNullOrEmpty(productCode) ? tagSymbol.Name.Replace(" ", "-") : productCode;

                if (!Enum.TryParse(tagSymbol.Type, out TagType type))
                {
                    type = TagType.Application;
                }

                // Ensure all tag symbols in this product share a regid.
                var firstTagSymbol = swidSymbols.Values.FirstOrDefault();
                if (firstTagSymbol != null && firstTagSymbol.Regid != tagSymbol.Regid)
                {
                    this.Messaging.Write(TagErrors.SingleRegIdPerProduct(tagSymbol.SourceLineNumbers, tagSymbol.Regid, firstTagSymbol.Regid, firstTagSymbol.SourceLineNumbers));
                    continue;
                }

                // Find the FileSymbol that is referenced by this WixProductTag.
                var fileSymbol = fileSymbols.FirstOrDefault(f => f.Id.Id == tagFileRef);
                if (fileSymbol != null)
                {
                    var fileName = String.IsNullOrEmpty(fileSymbol.Name) ? Path.GetFileName(fileSymbol.Source.Path) : fileSymbol.Name;

                    // Write the tag file.
                    fileSymbol.Source = new IntermediateFieldPathValue
                    {
                        Path = Path.Combine(this.workingFolder, fileName)
                    };

                    this.backendHelper.TrackFile(fileSymbol.Source.Path, TrackedFileType.Temporary, fileSymbol.SourceLineNumbers);

                    using (var fs = new FileStream(fileSymbol.Source.Path, FileMode.Create))
                    {
                        TagWriter.CreateTagFile(fs, tagSymbol.Regid, uniqueId, productName, productVersion, manufacturer, licensed, type, null);
                    }

                    // Ensure the matching "SoftwareIdentificationTag" symbol exists and is populated correctly.
                    if (!swidSymbols.TryGetValue(tagFileRef, out var swidRow))
                    {
                        swidRow = new SoftwareIdentificationTagSymbol(fileSymbol.SourceLineNumbers)
                        {
                            FileRef = tagFileRef,
                            Regid = tagSymbol.Regid
                        };

                        swidSymbols.Add(tagFileRef, swidRow);
                    }

                    // Always rewrite.
                    swidRow.TagId = uniqueId;
                    swidRow.Type = type.ToString();
                }
            }

            return swidSymbols.Values;
        }

        private static string CalculateWorkingFolder(string intermediateFolder) => Path.Combine(intermediateFolder, "_swidtag");
    }
}
