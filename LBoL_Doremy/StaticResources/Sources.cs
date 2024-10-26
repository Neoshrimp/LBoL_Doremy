using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoL_Doremy.StaticResources
{
    internal class Sources
    {

        private static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());
        private static DirectorySource imgsDirSource = new DirectorySource(LBoL_Doremy.PInfo.GUID, "imgs");

        internal static DirectorySource playerImgsSource = new DirectorySource(LBoL_Doremy.PInfo.GUID, "playerImgs");
        internal static IResourceSource imgsSource = imgsDirSource;

        internal static DirectorySource extraImgs = new DirectorySource(LBoL_Doremy.PInfo.GUID, "extraImgs");

    }
}
