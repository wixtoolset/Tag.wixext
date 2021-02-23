// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolsetTest.Tag
{
    using System.Linq;
    using WixBuildTools.TestSupport;
    using WixToolset.Core.TestPackage;
    using WixToolset.Tag;
    using Xunit;

    public class PackageTagExtensionFixture
    {
        [Fact]
        public void CanBuildPackageWithTag()
        {
            var folder = TestData.Get(@"TestData\ProductTag");
            var build = new Builder(folder, typeof(TagExtensionFactory), new[] { folder });

            var results = build.BuildAndQuery(Build, "File", "SoftwareIdentificationTag");
            WixAssert.CompareLineByLine(new[]
            {
                "File:filF5_pLhBuF5b4N9XEo52g_hUM5Lo\tfilF5_pLhBuF5b4N9XEo52g_hUM5Lo\texample.txt\t20\t\t\t512\t1",
                "File:tagkVqdRrA2JRwvrWCPR6pTv7eOzoE\ttagkVqdRrA2JRwvrWCPR6pTv7eOzoE\tth_a8yhh|regid.2008-09.org.wixtoolset ~TagTestPackage.swidtag\t910\t\t\t1\t2",
                "SoftwareIdentificationTag:tagkVqdRrA2JRwvrWCPR6pTv7eOzoE\tregid.2008-09.org.wixtoolset\t8738B0C5-C4AA-4634-8C03-11EAA2F1E15D\tComponent"
            }, results.ToArray());
        }

        private static void Build(string[] args)
        {
            var result = WixRunner.Execute(args)
                                  .AssertSuccess();
        }
    }
}
