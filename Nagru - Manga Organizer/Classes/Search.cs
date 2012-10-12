using System;
using System.Text;

namespace Nagru___Manga_Organizer
{
    /* Provides predicate with ability to accept parameters
       Author: Alex Perepletov (Aug 1, 2006)                     */
    public class Search
    {
        string sItem;

        /* Gets the predicate for use with custom entry   */
        public Predicate<Main.stEntry> Match
        { get { return IsMatch; } }

        /* Initialize with suffix we want to match   */
        public Search(string Item)
        { sItem = Item; }

        /* Returns true if Entries match   */
        private bool IsMatch(Main.stEntry en)
        { return en.Equals(sItem); }
    }
}
