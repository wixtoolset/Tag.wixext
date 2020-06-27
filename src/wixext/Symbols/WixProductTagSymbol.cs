// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using WixToolset.Data;
    using WixToolset.Tag.Symbols;

    public static partial class TagSymbolDefinitions
    {
        public static readonly IntermediateSymbolDefinition WixProductTag = new IntermediateSymbolDefinition(
            TagSymbolDefinitionType.WixProductTag.ToString(),
            new[]
            {
                new IntermediateFieldDefinition(nameof(WixProductTagSymbolFields.FileRef), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixProductTagSymbolFields.Regid), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixProductTagSymbolFields.Name), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixProductTagSymbolFields.Attributes), IntermediateFieldType.Number),
                new IntermediateFieldDefinition(nameof(WixProductTagSymbolFields.Type), IntermediateFieldType.String),
            },
            typeof(WixProductTagSymbol));
    }
}

namespace WixToolset.Tag.Symbols
{
    using WixToolset.Data;

    public enum WixProductTagSymbolFields
    {
        FileRef,
        Regid,
        Name,
        Attributes,
        Type,
    }

    public class WixProductTagSymbol : IntermediateSymbol
    {
        public WixProductTagSymbol() : base(TagSymbolDefinitions.WixProductTag, null, null)
        {
        }

        public WixProductTagSymbol(SourceLineNumber sourceLineNumber, Identifier id = null) : base(TagSymbolDefinitions.WixProductTag, sourceLineNumber, id)
        {
        }

        public IntermediateField this[WixProductTagSymbolFields index] => this.Fields[(int)index];

        public string FileRef
        {
            get => this.Fields[(int)WixProductTagSymbolFields.FileRef].AsString();
            set => this.Set((int)WixProductTagSymbolFields.FileRef, value);
        }

        public string Regid
        {
            get => this.Fields[(int)WixProductTagSymbolFields.Regid].AsString();
            set => this.Set((int)WixProductTagSymbolFields.Regid, value);
        }

        public string Name
        {
            get => this.Fields[(int)WixProductTagSymbolFields.Name].AsString();
            set => this.Set((int)WixProductTagSymbolFields.Name, value);
        }

        public int Attributes
        {
            get => this.Fields[(int)WixProductTagSymbolFields.Attributes].AsNumber();
            set => this.Set((int)WixProductTagSymbolFields.Attributes, value);
        }

        public string Type
        {
            get => this.Fields[(int)WixProductTagSymbolFields.Type].AsString();
            set => this.Set((int)WixProductTagSymbolFields.Type, value);
        }
    }
}