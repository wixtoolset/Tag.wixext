// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using System;
    using WixToolset.Data;

    public enum TagSymbolDefinitionType
    {
        SoftwareIdentificationTag,
        WixBundleTag,
        WixProductTag,
    }

    public static partial class TagSymbolDefinitions
    {
        public static readonly Version Version = new Version("4.0.0");

        public static IntermediateSymbolDefinition ByName(string name)
        {
            if (!Enum.TryParse(name, out TagSymbolDefinitionType type))
            {
                return null;
            }

            return ByType(type);
        }

        public static IntermediateSymbolDefinition ByType(TagSymbolDefinitionType type)
        {
            switch (type)
            {
                case TagSymbolDefinitionType.SoftwareIdentificationTag:
                    return TagSymbolDefinitions.SoftwareIdentificationTag;

                case TagSymbolDefinitionType.WixBundleTag:
                    return TagSymbolDefinitions.WixBundleTag;

                case TagSymbolDefinitionType.WixProductTag:
                    return TagSymbolDefinitions.WixProductTag;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
