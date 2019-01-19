// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using WixToolset.Data;
    using WixToolset.Tag.Tuples;

    public static partial class TagTupleDefinitions
    {
        public static readonly IntermediateTupleDefinition WixBundleTag = new IntermediateTupleDefinition(
            TagTupleDefinitionType.WixBundleTag.ToString(),
            new[]
            {
                new IntermediateFieldDefinition(nameof(WixBundleTagTupleFields.Filename), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixBundleTagTupleFields.Regid), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixBundleTagTupleFields.Name), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixBundleTagTupleFields.Attributes), IntermediateFieldType.Number),
                new IntermediateFieldDefinition(nameof(WixBundleTagTupleFields.Xml), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixBundleTagTupleFields.Type), IntermediateFieldType.String),
            },
            typeof(WixBundleTagTuple));
    }
}

namespace WixToolset.Tag.Tuples
{
    using WixToolset.Data;

    public enum WixBundleTagTupleFields
    {
        Filename,
        Regid,
        Name,
        Attributes,
        Xml,
        Type,
    }

    public class WixBundleTagTuple : IntermediateTuple
    {
        public WixBundleTagTuple() : base(TagTupleDefinitions.WixBundleTag, null, null)
        {
        }

        public WixBundleTagTuple(SourceLineNumber sourceLineNumber, Identifier id = null) : base(TagTupleDefinitions.WixBundleTag, sourceLineNumber, id)
        {
        }

        public IntermediateField this[WixBundleTagTupleFields index] => this.Fields[(int)index];

        public string Filename
        {
            get => this.Fields[(int)WixBundleTagTupleFields.Filename].AsString();
            set => this.Set((int)WixBundleTagTupleFields.Filename, value);
        }

        public string Regid
        {
            get => this.Fields[(int)WixBundleTagTupleFields.Regid].AsString();
            set => this.Set((int)WixBundleTagTupleFields.Regid, value);
        }

        public string Name
        {
            get => this.Fields[(int)WixBundleTagTupleFields.Name].AsString();
            set => this.Set((int)WixBundleTagTupleFields.Name, value);
        }

        public int Attributes
        {
            get => this.Fields[(int)WixBundleTagTupleFields.Attributes].AsNumber();
            set => this.Set((int)WixBundleTagTupleFields.Attributes, value);
        }

        public string Xml
        {
            get => this.Fields[(int)WixBundleTagTupleFields.Xml].AsString();
            set => this.Set((int)WixBundleTagTupleFields.Xml, value);
        }

        public string Type
        {
            get => this.Fields[(int)WixBundleTagTupleFields.Type].AsString();
            set => this.Set((int)WixBundleTagTupleFields.Type, value);
        }
    }
}