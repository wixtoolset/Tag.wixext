// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using WixToolset.Data;
    using WixToolset.Data.Tuples;
    using WixToolset.Extensibility;
    using WixToolset.Tag.Tuples;

    /// <summary>
    /// The compiler for the WiX Toolset Software Id Tag Extension.
    /// </summary>
    public sealed class TagCompiler : BaseCompilerExtension
    {
        public override XNamespace Namespace => "http://wixtoolset.org/schemas/v4/wxs/tag";

        /// <summary>
        /// Processes an element for the Compiler.
        /// </summary>
        /// <param name="sourceLineNumbers">Source line number for the parent element.</param>
        /// <param name="parentElement">Parent element of element to process.</param>
        /// <param name="element">Element to process.</param>
        /// <param name="contextValues">Extra information about the context in which this element is being parsed.</param>
        public override void ParseElement(Intermediate intermediate, IntermediateSection section, XElement parentElement, XElement element, IDictionary<string, string> context)
        {
            switch (parentElement.Name.LocalName)
            {
                case "Bundle":
                    switch (element.Name.LocalName)
                    {
                        case "Tag":
                            this.ParseBundleTagElement(intermediate, section, element);
                            break;
                        default:
                            this.ParseHelper.UnexpectedElement(parentElement, element);
                            break;
                    }
                    break;
                case "Product":
                    switch (element.Name.LocalName)
                    {
                        case "Tag":
                            this.ParseProductTagElement(intermediate, section, element);
                            break;
                        default:
                            this.ParseHelper.UnexpectedElement(parentElement, element);
                            break;
                    }
                    break;
                case "PatchFamily":
                    switch (element.Name.LocalName)
                    {
                        case "TagRef":
                            this.ParseTagRefElement(intermediate, section, element);
                            break;
                        default:
                            this.ParseHelper.UnexpectedElement(parentElement, element);
                            break;
                    }
                    break;
                default:
                    this.ParseHelper.UnexpectedElement(parentElement, element);
                    break;
            }
        }

        /// <summary>
        /// Parses a Tag element for Software Id Tag registration under a Bundle element.
        /// </summary>
        /// <param name="node">The element to parse.</param>
        private void ParseBundleTagElement(Intermediate intermediate, IntermediateSection section, XElement node)
        {
            SourceLineNumber sourceLineNumbers = this.ParseHelper.GetSourceLineNumbers(node);
            string name = null;
            string regid = null;
            YesNoType licensed = YesNoType.NotSet;
            string type = null;

            foreach (XAttribute attrib in node.Attributes())
            {
                if (String.IsNullOrEmpty(attrib.Name.NamespaceName) || this.Namespace == attrib.Name.Namespace)
                {
                    switch (attrib.Name.LocalName)
                    {
                        case "Name":
                            name = this.ParseHelper.GetAttributeLongFilename(sourceLineNumbers, attrib, false);
                            break;
                        case "Regid":
                            regid = this.ParseHelper.GetAttributeValue(sourceLineNumbers, attrib);
                            break;
                        case "Licensed":
                            licensed = this.ParseHelper.GetAttributeYesNoValue(sourceLineNumbers, attrib);
                            break;
                        case "Type":
                            type = this.ParseTagTypeAttribute(sourceLineNumbers, node, attrib);
                            break;
                        default:
                            this.ParseHelper.UnexpectedAttribute(node, attrib);
                            break;
                    }
                }
                else
                {
                    this.ParseHelper.ParseExtensionAttribute(this.Context.Extensions, intermediate, section, node, attrib);
                }
            }

            this.ParseHelper.ParseForExtensionElements(this.Context.Extensions, intermediate, section, node);

            if (String.IsNullOrEmpty(name))
            {
                XAttribute productNameAttribute = node.Parent.Attribute("Name");
                if (null != productNameAttribute)
                {
                    name = productNameAttribute.Value;
                }
                else
                {
                    this.Messaging.Write(ErrorMessages.ExpectedAttribute(sourceLineNumbers, node.Name.LocalName, "Name"));
                }
            }

            if (!String.IsNullOrEmpty(name) && !this.ParseHelper.IsValidLongFilename(name, false))
            {
                this.Messaging.Write(TagErrors.IllegalName(sourceLineNumbers, node.Parent.Name.LocalName, name));
            }

            if (String.IsNullOrEmpty(regid))
            {
                this.Messaging.Write(ErrorMessages.ExpectedAttribute(sourceLineNumbers, node.Name.LocalName, "Regid"));
            }

            if (!this.Messaging.EncounteredError)
            {
                string fileName = String.Concat(regid, " ", name, ".swidtag");

                var tagRow = (WixBundleTagTuple)this.ParseHelper.CreateRow(section, sourceLineNumbers, "WixBundleTag");
                tagRow.Filename = fileName;
                tagRow.Regid = regid;
                tagRow.Name = name;
                if (YesNoType.Yes == licensed)
                {
                    tagRow.Attributes = 1;
                }
                // TagXml is set by the binder.
                tagRow.Type = type;
            }
        }

        /// <summary>
        /// Parses a Tag element for Software Id Tag registration under a Product element.
        /// </summary>
        /// <param name="node">The element to parse.</param>
        private void ParseProductTagElement(Intermediate intermediate, IntermediateSection section, XElement node)
        {
            SourceLineNumber sourceLineNumbers = this.ParseHelper.GetSourceLineNumbers(node);
            string name = null;
            string regid = null;
            string feature = "WixSwidTag";
            YesNoType licensed = YesNoType.NotSet;
            string type = null;

            foreach (XAttribute attrib in node.Attributes())
            {
                if (String.IsNullOrEmpty(attrib.Name.NamespaceName) || this.Namespace == attrib.Name.Namespace)
                {
                    switch (attrib.Name.LocalName)
                    {
                        case "Name":
                            name = this.ParseHelper.GetAttributeLongFilename(sourceLineNumbers, attrib, false);
                            break;
                        case "Regid":
                            regid = this.ParseHelper.GetAttributeValue(sourceLineNumbers, attrib);
                            break;
                        case "Feature":
                            feature = this.ParseHelper.GetAttributeIdentifierValue(sourceLineNumbers, attrib);
                            break;
                        case "Licensed":
                            licensed = this.ParseHelper.GetAttributeYesNoValue(sourceLineNumbers, attrib);
                            break;
                        case "Type":
                            type = this.ParseTagTypeAttribute(sourceLineNumbers, node, attrib);
                            break;
                        default:
                            this.ParseHelper.UnexpectedAttribute(node, attrib);
                            break;
                    }
                }
                else
                {
                    this.ParseHelper.ParseExtensionAttribute(this.Context.Extensions, intermediate, section, node, attrib);
                }
            }

            this.ParseHelper.ParseForExtensionElements(this.Context.Extensions, intermediate, section, node);

            if (String.IsNullOrEmpty(name))
            {
                XAttribute productNameAttribute = node.Parent.Attribute("Name");
                if (null != productNameAttribute)
                {
                    name = productNameAttribute.Value;
                }
                else
                {
                    this.Messaging.Write(ErrorMessages.ExpectedAttribute(sourceLineNumbers, node.Name.LocalName, "Name"));
                }
            }

            if (!String.IsNullOrEmpty(name) && !this.ParseHelper.IsValidLongFilename(name, false))
            {
                this.Messaging.Write(TagErrors.IllegalName(sourceLineNumbers, node.Parent.Name.LocalName, name));
            }

            if (String.IsNullOrEmpty(regid))
            {
                this.Messaging.Write(ErrorMessages.ExpectedAttribute(sourceLineNumbers, node.Name.LocalName, "Regid"));
            }

            if (!this.Messaging.EncounteredError)
            {
                string directoryId = "WixTagRegidFolder";
                Identifier fileId = this.ParseHelper.CreateIdentifier("tag", regid, ".product.tag");
                string fileName = String.Concat(regid, " ", name, ".swidtag");
                string shortName = this.ParseHelper.CreateShortName(fileName, false, false);

                this.ParseHelper.CreateSimpleReference(section, sourceLineNumbers, "Directory", directoryId);

#if TODO_MODERNIZATION
                var componentRow = (ComponentTuple)this.ParseHelper.CreateRow(section, sourceLineNumbers, "Component", fileId);
                componentRow.ComponentId = "*";
                componentRow.Attributes = 0;
                componentRow.Directory_ = directoryId;
                componentRow.KeyPath = fileId.Id;

                this.ParseHelper.CreateSimpleReference(section, sourceLineNumbers, "Feature", feature);
                this.ParseHelper.CreateComplexReference(section, sourceLineNumbers, ComplexReferenceParentType.Feature, feature, null, ComplexReferenceChildType.Component, fileId.Id, true);

                var fileRow = (FileTuple)this.ParseHelper.CreateRow(section, sourceLineNumbers, "File", fileId);
                fileRow.Component_ = fileId.Id;
                fileRow.File = String.Concat(shortName, "|", fileName);

                var wixFileRow = (WixFileTuple)this.ParseHelper.CreateRow(section, sourceLineNumbers, "WixFile");
                wixFileRow.Directory_ = directoryId;
                wixFileRow.File_ = fileId.Id;
                wixFileRow.DiskId = 1;
                wixFileRow.Attributes = 1;
                wixFileRow.Source = new IntermediateFieldPathValue { Path = String.Concat("%TEMP%\\", fileName) };
#endif

                this.ParseHelper.EnsureTable(section, sourceLineNumbers, "SoftwareIdentificationTag");
                var row = (WixProductTagTuple)this.ParseHelper.CreateRow(section, sourceLineNumbers, "WixProductTag", fileId);
                row.Regid = regid;
                row.Name = name;
                if (YesNoType.Yes == licensed)
                {
                    row.Attributes = 1;
                }
                row.Type = type;

                this.ParseHelper.CreateSimpleReference(section, sourceLineNumbers, "File", fileId.Id);
            }
        }

        /// <summary>
        /// Parses a TagRef element for Software Id Tag registration under a PatchFamily element.
        /// </summary>
        /// <param name="node">The element to parse.</param>
        private void ParseTagRefElement(Intermediate intermediate, IntermediateSection section, XElement node)
        {
            SourceLineNumber sourceLineNumbers = this.ParseHelper.GetSourceLineNumbers(node);
            string regid = null;

            foreach (XAttribute attrib in node.Attributes())
            {
                if (String.IsNullOrEmpty(attrib.Name.NamespaceName) || this.Namespace == attrib.Name.Namespace)
                {
                    switch (attrib.Name.LocalName)
                    {
                        case "Regid":
                            regid = this.ParseHelper.GetAttributeValue(sourceLineNumbers, attrib);
                            break;
                        default:
                            this.ParseHelper.UnexpectedAttribute(node, attrib);
                            break;
                    }
                }
                else
                {
                    this.ParseHelper.ParseExtensionAttribute(this.Context.Extensions, intermediate, section, node, attrib);
                }
            }

            this.ParseHelper.ParseForExtensionElements(this.Context.Extensions, intermediate, section, node);

            if (String.IsNullOrEmpty(regid))
            {
                this.Messaging.Write(ErrorMessages.ExpectedAttribute(sourceLineNumbers, node.Name.LocalName, "Regid"));
            }

            if (!this.Messaging.EncounteredError)
            {
                Identifier id = this.ParseHelper.CreateIdentifier("tag", regid, ".product.tag");
#if TODO_PATCHING
                this.ParseHelper.CreatePatchFamilyChildReference(sourceLineNumbers, "Component", id.Id);
#endif
            }
        }

        private string ParseTagTypeAttribute(SourceLineNumber sourceLineNumbers, XElement node, XAttribute attrib)
        {
            string typeValue = this.ParseHelper.GetAttributeValue(sourceLineNumbers, attrib);
            switch (typeValue)
            {
                case "application":
                    typeValue = "Application";
                    break;
                case "component":
                    typeValue = "Component";
                    break;
                case "feature":
                    typeValue = "Feature";
                    break;
                case "group":
                    typeValue = "Group";
                    break;
                case "patch":
                    typeValue = "Patch";
                    break;
                default:
                    this.Messaging.Write(ErrorMessages.IllegalAttributeValue(sourceLineNumbers, node.Name.LocalName, attrib.Name.LocalName, typeValue, "application", "component", "feature", "group", "patch"));
                    break;
            }

            return typeValue;
        }
    }
}
