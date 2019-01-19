// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolsetTest.UI
{
    using System.Linq;
    using WixBuildTools.TestSupport;
    using WixToolset.Core.TestPackage;
    using WixToolset.Tag;
    using Xunit;

    public class UIExtensionFixture
    {
        [Fact(Skip = "Currently fails")]
        public void CanBuildUsingProductTag()
        {
            var folder = TestData.Get(@"TestData\ProductTag");
            var build = new Builder(folder, typeof(TagExtensionFactory), new[] { folder });

            var results = build.BuildAndQuery(Build, "Property");
            Assert.Equal(new[]
            {
                "Property:",
            }, results.OrderBy(s => s).ToArray());
        }

        private static void Build(string[] args)
        {
            var result = WixRunner.Execute(args)
                                  .AssertSuccess();
        }
    }
}
