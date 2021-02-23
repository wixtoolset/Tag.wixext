// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Data
{
    using System;
    using System.Resources;

    public static class TagErrors
    {
        public static Message IllegalName(SourceLineNumber sourceLineNumbers, string parentElement, string name)
        {
            return Message(sourceLineNumbers, Ids.IllegalName, "The Tag/@Name attribute value, '{1}', contains invalid filename identifiers. The Tag/@Name may have defaulted from the {0}/@Name attrbute. If so, use the Tag/@Name attribute to provide a valid filename. Any character except for the follow may be used: \\ ? | > < : / * \".", parentElement, name);
        }

        public static Message SingleRegIdPerProduct(SourceLineNumber sourceLineNumbers, string regid, string firstRegid, SourceLineNumber firstSourceLineNumbers)
        {
            return Message(sourceLineNumbers, Ids.SingleRegIdPerProduct, "All of the Tag/@Regid attribute values in a package must match. The RegId '{0}' does not match the first RegId '{1}' found at: {2}.", regid, firstRegid, firstSourceLineNumbers.ToString());
        }

        private static Message Message(SourceLineNumber sourceLineNumber, Ids id, string format, params object[] args)
        {
            return new Message(sourceLineNumber, MessageLevel.Error, (int)id, format, args);
        }

        private static Message Message(SourceLineNumber sourceLineNumber, Ids id, ResourceManager resourceManager, string resourceName, params object[] args)
        {
            return new Message(sourceLineNumber, MessageLevel.Error, (int)id, resourceManager, resourceName, args);
        }

        public enum Ids
        {
            IllegalName = 6601,
            SingleRegIdPerProduct = 6602,
        }
    }
}
