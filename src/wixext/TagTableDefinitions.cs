// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag
{
    using WixToolset.Data.WindowsInstaller;

    public static class TagTableDefinitions
    {
        public static readonly TableDefinition WixProductTag = new TableDefinition(
            "WixProductTag",
            TagSymbolDefinitions.WixProductTag,
            new[]
            {
                new ColumnDefinition("File_", ColumnType.String, 72, primaryKey: true, nullable: false, ColumnCategory.Identifier, keyTable: "File", keyColumn: 1, description: "The file representing the software id tag.", modularizeType: ColumnModularizeType.Column),
                new ColumnDefinition("Regid", ColumnType.String, 0, primaryKey: false, nullable: false, ColumnCategory.Text, description: "The regid for the software id tag."),
                new ColumnDefinition("Name", ColumnType.String, 0, primaryKey: false, nullable: true, ColumnCategory.Text, description: "The name for the software id tag."),
                new ColumnDefinition("Attributes", ColumnType.Number, 4, primaryKey: false, nullable: true, ColumnCategory.Unknown, minValue: 0, maxValue: 2147483647, description: "A 32-bit word that specifies bits of software id tag."),
                new ColumnDefinition("Type", ColumnType.String, 0, primaryKey: false, nullable: true, ColumnCategory.Text, description: "The type of the software id tag."),
            },
            unreal: true,
            symbolIdIsPrimaryKey: false
        );

        public static readonly TableDefinition SoftwareIdentificationTag = new TableDefinition(
            "SoftwareIdentificationTag",
            TagSymbolDefinitions.SoftwareIdentificationTag,
            new[]
            {
                new ColumnDefinition("File_", ColumnType.String, 72, primaryKey: true, nullable: false, ColumnCategory.Identifier, keyTable: "File", keyColumn: 1, description: "The file that installs the software id tag.", modularizeType: ColumnModularizeType.Column),
                new ColumnDefinition("Regid", ColumnType.String, 0, primaryKey: false, nullable: false, ColumnCategory.Text, description: "The regid for the software id tag."),
                new ColumnDefinition("UniqueId", ColumnType.String, 0, primaryKey: false, nullable: false, ColumnCategory.Text, description: "The unique id for the software id tag."),
                new ColumnDefinition("Type", ColumnType.String, 0, primaryKey: false, nullable: false, ColumnCategory.Text, description: "The type of the software id tag."),
            },
            symbolIdIsPrimaryKey: false
        );

        public static readonly TableDefinition[] All = new[]
        {
            WixProductTag,
            SoftwareIdentificationTag,
        };
    }
}
