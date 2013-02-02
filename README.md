Manga-Organizer
===============

As a mild hoarder I found that a folder hierarchy simply doesn't cut it when you have 500+ folders to manage. No tags, stilted search functionality, no automation; it simply becomes a mess to find that specific manga you downloaded a month ago. So I got fed up with it and wrote a simple application to handle all this, and hopefully it's in an acceptable state to be useful to others as well.

Basically it provides semi-automated metadata for your folders, along with the option to use a built-in 2-page image browser. It can automatically scan your current folders into a directory; which will fill out the artist, title, and page count fields. The same can be done by dragging and dropping a folder over the listview. Given a g.e-hentai gallery url it can auto-populate the remaining fields (exc. description). A tab for notes is also included to keep track of your always-full backlog.

If this sounds useful to you, give it a shot: <a href="http://www.mediafire.com/?i8p7s19kht1s33l">Download</a>

For examples of it running see: <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_Browse.jpg" target="_blank">Browse</a> - <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_View.jpg" target="_blank">View</a> - <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_Notes.jpg" target="_blank">Notes</a>

(You'll need the .NET library to run it, but if you're running Vista/7 it should've come bundled with it. Otherwise, download <a href="https://www.microsoft.com/en-us/download/details.aspx?id=17851">.Net</a> for Windows or <a href="http://www.go-mono.com/mono-downloads/download.html">Mono</a> for Linux/OSX.)


================


Notes:
- no install required
- searches are cumulative, with terms separated by a comma
- entry data can be automatically loaded from a g.e-hentai gallery url
  (if you give it an exhentai link, it'll try to auto-load its g.e equivalent)
- 'auto-magic' also applies to copy/pasted tags, gallery titles pasted into the artist field, and folders dropped onto the listview
- browse images with your default image viewer (menu item: Open) or with the built in one (click on the cover image)
- the latter browser displays two images simultaneously and should be traversed from right to left (Also, can hit 'f' to bring up a list of pages, then double-click on a page to open it)
- to un-ignore an item in the scan operation, select it and press the ignore button again
- if you set the root folder (Menu -> Change Root) to the root of your manga folder, the FolderBrowserDialog will try to auto-find the correct folder path when new items are added.
- load the 'About' entry to automatically check for an updated version
