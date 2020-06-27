// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using WixToolset.Data;
    using WixToolset.Tag.Symbols;

    public static partial class TagSymbolDefinitions
    {
        public static readonly IntermediateSymbolDefinition SoftwareIdentificationTag = new IntermediateSymbolDefinition(
            TagSymbolDefinitionType.SoftwareIdentificationTag.ToString(),
            new[]
            {
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagSymbolFields.FileRef), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagSymbolFields.Regid), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagSymbolFields.UniqueId), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagSymbolFields.Type), IntermediateFieldType.String),
            },
            typeof(SoftwareIdentificationTagSymbol));
    }
}

namespace WixToolset.Tag.Symbols
{
    using WixToolset.Data;

    public enum SoftwareIdentificationTagSymbolFields
    {
        FileRef,
        Regid,
        UniqueId,
        Type,
    }

    public class SoftwareIdentificationTagSymbol : IntermediateSymbol
    {
        public SoftwareIdentificationTagSymbol() : base(TagSymbolDefinitions.SoftwareIdentificationTag, null, null)
        {
        }

        public SoftwareIdentificationTagSymbol(SourceLineNumber sourceLineNumber, Identifier id = null) : base(TagSymbolDefinitions.SoftwareIdentificationTag, sourceLineNumber, id)
        {
        }

        public IntermediateField this[SoftwareIdentificationTagSymbolFields index] => this.Fields[(int)index];

        public string FileRef
        {
            get => this.Fields[(int)SoftwareIdentificationTagSymbolFields.FileRef].AsString();
            set => this.Set((int)SoftwareIdentificationTagSymbolFields.FileRef, value);
        }

        public string Regid
        {
            get => this.Fields[(int)SoftwareIdentificationTagSymbolFields.Regid].AsString();
            set => this.Set((int)SoftwareIdentificationTagSymbolFields.Regid, value);
        }

        public string UniqueId
        {
            get => this.Fields[(int)SoftwareIdentificationTagSymbolFields.UniqueId].AsString();
            set => this.Set((int)SoftwareIdentificationTagSymbolFields.UniqueId, value);
        }

        public string Type
        {
            get => this.Fields[(int)SoftwareIdentificationTagSymbolFields.Type].AsString();
            set => this.Set((int)SoftwareIdentificationTagSymbolFields.Type, value);
        }
    }
}