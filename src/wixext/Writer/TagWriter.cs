// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Tag.Writer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    public static class TagWriter
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
        {
            CloseOutput = false,
            Encoding = Encoding.UTF8
        };

        public static string CreateFourPartVersion(string versionString)
        {
            if (Version.TryParse(versionString, out var version))
            {
                versionString = new Version(version.Major,
                                            -1 < version.Minor ? version.Minor : 0,
                                            -1 < version.Build ? version.Build : 0,
                                            -1 < version.Revision ? version.Revision : 0).ToString();
            }

            return versionString;
        }

        public static void CreateTagFile(Stream stream, string regid, string uniqueId, string name, string version, string manufacturer, bool licensed, TagType tagType, IEnumerable<SoftwareTag> containedTags)
        {
            using (var writer = XmlWriter.Create(stream, WriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("software_identification_tag", "http://standards.iso.org/iso/19770/-2/2009/schema.xsd");
                writer.WriteElementString("entitlement_required_indicator", licensed ? "true" : "false");

                writer.WriteElementString("product_title", name);

                writer.WriteStartElement("product_version");
                writer.WriteElementString("name", version.ToString());
                if (Version.TryParse(version, out var parsedVersion))
                {
                    writer.WriteStartElement("numeric");
                    writer.WriteElementString("major", parsedVersion.Major.ToString());
                    writer.WriteElementString("minor", parsedVersion.Minor.ToString());
                    writer.WriteElementString("build", parsedVersion.Build.ToString());
                    writer.WriteElementString("review", parsedVersion.Revision.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("software_creator");
                writer.WriteElementString("name", manufacturer);
                writer.WriteElementString("regid", regid);
                writer.WriteEndElement();

                if (licensed)
                {
                    writer.WriteStartElement("software_licensor");
                    writer.WriteElementString("name", manufacturer);
                    writer.WriteElementString("regid", regid);
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("software_id");
                writer.WriteElementString("unique_id", uniqueId);
                writer.WriteElementString("tag_creator_regid", regid);
                writer.WriteEndElement();

                writer.WriteStartElement("tag_creator");
                writer.WriteElementString("name", manufacturer);
                writer.WriteElementString("regid", regid);
                writer.WriteEndElement();

                if (containedTags?.Any() == true)
                {
                    writer.WriteStartElement("complex_of");
                    foreach (var tag in containedTags)
                    {
                        writer.WriteStartElement("software_id");
                        writer.WriteElementString("unique_id", tag.Id);
                        writer.WriteElementString("tag_creator_regid", tag.Regid);
                        writer.WriteEndElement(); // </software_id>
                    }
                    writer.WriteEndElement(); // </complex_of>
                }

                if (TagType.Unknown != tagType)
                {
                    writer.WriteStartElement("extended_information");
                    writer.WriteStartElement("tag_type", "http://www.tagvault.org/tv_extensions.xsd");
                    writer.WriteValue(tagType.ToString());
                    writer.WriteEndElement(); // </tag_type>
                    writer.WriteEndElement(); // </extended_information>
                }

                writer.WriteEndElement(); // </software_identification_tag>
            }
        }
    }
}
