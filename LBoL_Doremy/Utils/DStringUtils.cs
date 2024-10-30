using System;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.RootTemplates
{
    public static class DStringUtils
    {
        public static string FirstToLower(this string s) => s.Length > 0 ? char.ToLower(s[0]) + s[1..] : s;
    }
}
