using System;
using System.Text;

namespace Nagru___Manga_Organizer
{
    /* Extends String.Contains to support case options & multiple filters  */
    public static class ExtString
    {
        public static bool Contains(this string sSource, string sCheck,
            StringComparison scComp = StringComparison.OrdinalIgnoreCase)
        {
            string[] sFilters = sCheck.Split('\0');
            for (int i = 0; i < sFilters.Length; i++)
                if ((sSource.IndexOf(sFilters[i], scComp) >= 0))
                    return true;

            return false;
        }
    }
}
