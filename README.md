Manga-Organizer
===============

As a mild digital hoarder, I found trying to keep track of 500+ folders was becoming ridiculous. So instead of an awkward folder hierarchy, I've been writing this program to handle it for me, and I think it's in an acceptable state for release.

Basically, given folder names of "[Artist] Title" (E-Hentai's format), it can automatically scan them into a database. After that you can search through the list by artist, title, tags and type. 

If this sounds useful to you, give it a shot: <a href="http://www.mediafire.com/?qkccfz45rdoqrxb">Download</a>

For an example of it running see: <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_Browse.jpg" target="_blank">Browse</a> - <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_View.jpg" target="_blank">View</a> - <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_Notes.jpg" target="_blank">Notes</a>

(You'll need the .NET library to run it, but if you're running Vista/7 it should've come bundled with it. Otherwise, download <a href="https://www.microsoft.com/en-us/download/details.aspx?id=17851">.Net</a> for Windows or <a href="http://www.go-mono.com/mono-downloads/download.html">Mono</a> for Linux/OSX.)


================


Notes:
- no install required
- searches are inclusive, with terms separated by a comma
- entry data can be automatically loaded from a g.e-hentai gallery url
  (if you give it an exhentai link, it'll try to load its g.e equivalent automatically)
- 'auto-magic' also applies to copy/pasted tags, full titles, and folders dropped onto the listview
- browse images with your default image viewer (menu item: Open) or with the built in one (click on the cover image)
- the latter browser displays two images simultaneously and should be traversed from right to left
- to un-ignore an item in the scan operation, select it and press the ignore button again
- if you set the root folder (Menu -> Change Root) to the root of your manga folder, the folderbrowserdialog will try to auto-find the correct folder path when new items are added.
