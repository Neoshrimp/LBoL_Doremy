using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NoMetaScaling
{
    internal static class Statics
    {
        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());
        internal static DirectorySource directorySource = new DirectorySource(NoMetaScalling.PInfo.GUID, "");

        internal static BatchLocalization seBatchLoc = new BatchLocalization(directorySource, typeof(StatusEffectTemplate), "fakeSE");
    }
}
