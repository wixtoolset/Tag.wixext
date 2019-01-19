// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using System;
    using WixToolset.Data;

    public enum TagTupleDefinitionType
    {
        SoftwareIdentificationTag,
        WixBundleTag,
        WixProductTag,
    }

    public static partial class TagTupleDefinitions
    {
        public static readonly Version Version = new Version("4.0.0");

        public static IntermediateTupleDefinition ByName(string name)
        {
            if (!Enum.TryParse(name, out TagTupleDefinitionType type))
            {
                return null;
            }

            return ByType(type);
        }

        public static IntermediateTupleDefinition ByType(TagTupleDefinitionType type)
        {
            switch (type)
            {
                case TagTupleDefinitionType.SoftwareIdentificationTag:
                    return TagTupleDefinitions.SoftwareIdentificationTag;

                case TagTupleDefinitionType.WixBundleTag:
                    return TagTupleDefinitions.WixBundleTag;

                case TagTupleDefinitionType.WixProductTag:
                    return TagTupleDefinitions.WixProductTag;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
