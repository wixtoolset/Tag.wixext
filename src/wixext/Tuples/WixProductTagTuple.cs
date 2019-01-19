// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using WixToolset.Data;
    using WixToolset.Tag.Tuples;

    public static partial class TagTupleDefinitions
    {
        public static readonly IntermediateTupleDefinition WixProductTag = new IntermediateTupleDefinition(
            TagTupleDefinitionType.WixProductTag.ToString(),
            new[]
            {
                new IntermediateFieldDefinition(nameof(WixProductTagTupleFields.File_), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixProductTagTupleFields.Regid), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixProductTagTupleFields.Name), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(WixProductTagTupleFields.Attributes), IntermediateFieldType.Number),
                new IntermediateFieldDefinition(nameof(WixProductTagTupleFields.Type), IntermediateFieldType.String),
            },
            typeof(WixProductTagTuple));
    }
}

namespace WixToolset.Tag.Tuples
{
    using WixToolset.Data;

    public enum WixProductTagTupleFields
    {
        File_,
        Regid,
        Name,
        Attributes,
        Type,
    }

    public class WixProductTagTuple : IntermediateTuple
    {
        public WixProductTagTuple() : base(TagTupleDefinitions.WixProductTag, null, null)
        {
        }

        public WixProductTagTuple(SourceLineNumber sourceLineNumber, Identifier id = null) : base(TagTupleDefinitions.WixProductTag, sourceLineNumber, id)
        {
        }

        public IntermediateField this[WixProductTagTupleFields index] => this.Fields[(int)index];

        public string File_
        {
            get => this.Fields[(int)WixProductTagTupleFields.File_].AsString();
            set => this.Set((int)WixProductTagTupleFields.File_, value);
        }

        public string Regid
        {
            get => this.Fields[(int)WixProductTagTupleFields.Regid].AsString();
            set => this.Set((int)WixProductTagTupleFields.Regid, value);
        }

        public string Name
        {
            get => this.Fields[(int)WixProductTagTupleFields.Name].AsString();
            set => this.Set((int)WixProductTagTupleFields.Name, value);
        }

        public int Attributes
        {
            get => this.Fields[(int)WixProductTagTupleFields.Attributes].AsNumber();
            set => this.Set((int)WixProductTagTupleFields.Attributes, value);
        }

        public string Type
        {
            get => this.Fields[(int)WixProductTagTupleFields.Type].AsString();
            set => this.Set((int)WixProductTagTupleFields.Type, value);
        }
    }
}