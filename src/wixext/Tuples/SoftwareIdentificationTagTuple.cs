// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using WixToolset.Data;
    using WixToolset.Tag.Tuples;

    public static partial class TagTupleDefinitions
    {
        public static readonly IntermediateTupleDefinition SoftwareIdentificationTag = new IntermediateTupleDefinition(
            TagTupleDefinitionType.SoftwareIdentificationTag.ToString(),
            new[]
            {
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagTupleFields.File_), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagTupleFields.Regid), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagTupleFields.UniqueId), IntermediateFieldType.String),
                new IntermediateFieldDefinition(nameof(SoftwareIdentificationTagTupleFields.Type), IntermediateFieldType.String),
            },
            typeof(SoftwareIdentificationTagTuple));
    }
}

namespace WixToolset.Tag.Tuples
{
    using WixToolset.Data;

    public enum SoftwareIdentificationTagTupleFields
    {
        File_,
        Regid,
        UniqueId,
        Type,
    }

    public class SoftwareIdentificationTagTuple : IntermediateTuple
    {
        public SoftwareIdentificationTagTuple() : base(TagTupleDefinitions.SoftwareIdentificationTag, null, null)
        {
        }

        public SoftwareIdentificationTagTuple(SourceLineNumber sourceLineNumber, Identifier id = null) : base(TagTupleDefinitions.SoftwareIdentificationTag, sourceLineNumber, id)
        {
        }

        public IntermediateField this[SoftwareIdentificationTagTupleFields index] => this.Fields[(int)index];

        public string File_
        {
            get => this.Fields[(int)SoftwareIdentificationTagTupleFields.File_].AsString();
            set => this.Set((int)SoftwareIdentificationTagTupleFields.File_, value);
        }

        public string Regid
        {
            get => this.Fields[(int)SoftwareIdentificationTagTupleFields.Regid].AsString();
            set => this.Set((int)SoftwareIdentificationTagTupleFields.Regid, value);
        }

        public string UniqueId
        {
            get => this.Fields[(int)SoftwareIdentificationTagTupleFields.UniqueId].AsString();
            set => this.Set((int)SoftwareIdentificationTagTupleFields.UniqueId, value);
        }

        public string Type
        {
            get => this.Fields[(int)SoftwareIdentificationTagTupleFields.Type].AsString();
            set => this.Set((int)SoftwareIdentificationTagTupleFields.Type, value);
        }
    }
}