// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolsetTest.Tag
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using WixBuildTools.TestSupport;
    using WixToolset.Core.TestPackage;
    using WixToolset.Data;
    using WixToolset.Tag;
    using Xunit;

    public class BundleTagExtensionFixture
    {
        private static readonly XNamespace SwidTagNamespace = "http://standards.iso.org/iso/19770/-2/2009/schema.xsd";

        [Fact]
        public void CanBuildBundleWithTag()
        {
            var testDataFolder = TestData.Get(@"TestData");

            using (var fs = new DisposableFileSystem())
            {
                var baseFolder = fs.GetFolder();
                var intermediateFolder = Path.Combine(baseFolder, "obj");
                var extPath = Path.GetFullPath(new Uri(typeof(TagExtensionFactory).Assembly.CodeBase).LocalPath);

                var result = WixRunner.Execute(new[]
                {
                    "build",
                    Path.Combine(testDataFolder, "ProductTag", "PackageWithTag.wxs"),
                    Path.Combine(testDataFolder, "ProductTag", "PackageComponents.wxs"),
                    "-loc", Path.Combine(testDataFolder, "ProductTag", "Package.en-us.wxl"),
                    "-bindpath", Path.Combine(testDataFolder, "ProductTag"),
                    "-ext", extPath,
                    "-intermediateFolder", Path.Combine(intermediateFolder, "package"),
                    "-o", Path.Combine(baseFolder, "package", @"test.msi")
                });

                result.AssertSuccess();

                result = WixRunner.Execute(new[]
                {
                    "build",
                    Path.Combine(testDataFolder, "BundleTag", "BundleWithTag.wxs"),
                    "-ext", extPath,
                    "-bindpath", Path.Combine(testDataFolder, "BundleTag"),
                    "-bindpath", Path.Combine(baseFolder, "package"),
                    "-intermediateFolder", intermediateFolder,
                    "-o", Path.Combine(baseFolder, @"bin\test.exe")
                });

                result.AssertSuccess();

                Assert.True(File.Exists(Path.Combine(baseFolder, @"bin\test.exe")));
                Assert.True(File.Exists(Path.Combine(baseFolder, @"bin\test.wixpdb")));

                using var ouput = WixOutput.Read(Path.Combine(baseFolder, @"bin\test.wixpdb"));

                var badata = ouput.GetDataStream("wix-badata.xml");
                var doc = XDocument.Load(badata);

                var swidTag = doc.Root.Element("WixSoftwareTag").Value;
                var docTag = XDocument.Parse(swidTag);

                var title = docTag.Root.Element(SwidTagNamespace + "product_title").Value;
                var version = docTag.Root.Element(SwidTagNamespace + "product_version").Element(SwidTagNamespace + "name").Value;
                Assert.Equal("~TagTestBundle", title);
                Assert.Equal("4.3.2.1", version);
            }
        }

    }
}
