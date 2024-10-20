using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{
    internal static class Extensions
    {
        public static string SelfId(this EntityDefinition entityDefinition, string suffix = "Def")
        {
            var tName = entityDefinition.GetType().Name;
            return tName.Substring(0, tName.Length - suffix.Length);
        }
    }
}
