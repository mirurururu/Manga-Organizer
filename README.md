Manga-Organizer
===============

<p align="justify">As a mild hoarder I found that a folder hierarchy simply doesn't cut it when you have hundreds of folders to manage. No tags, stilted search functionality, no automation; it simply becomes a mess to find that specific manga you downloaded a month ago. So I got fed up with it and wrote a simple application to handle all this, and hopefully it's in an acceptable state to be useful to others as well.</p>

<p align="justify">Basically it provides semi-automated metadata for your folders, along with the option to use a built-in 2-page image browser. It can automatically scan your current folders into a directory; which will fill out the artist, title, and page count fields. The same can be done by dragging and dropping a folder over the listview. Given an EH gallery url it can auto-populate the remaining fields (exc. description). A tab for notes is also included to keep track of your always-full backlog.</p>

If this sounds useful to you, please give it a shot: <a href="https://dl.dropboxusercontent.com/u/103899726/Manga%20Organizer.exe">Download</a>

For examples of it running see: <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_Browse.jpg" target="_blank">Browse</a> - <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_View.jpg" target="_blank">View</a> - <a href="https://raw.github.com/Nagru/Manga-Organizer/master/Prv_Notes.jpg" target="_blank">Notes</a>

<i>(You'll need the .NET library, but if you're running Vista/7 it should've come bundled with it. Otherwise, download it here: <a href="https://www.microsoft.com/en-us/download/details.aspx?id=17851">.NET</a>)</i>


================


Notes:
- no install required
- searches function similarly to EH's<br />
  (eg. title:celeb_wife date:>01/01/12 -vanilla)
- entry data can be automatically loaded from an EH gallery url
- 'auto-magic' also applies to copy/pasted tags, gallery titles pasted into the artist field, and folders dropped onto the listview
- browse images with your default image viewer (Menu->Open) or with the built in one (click on the cover image)
- the latter browser displays two images simultaneously and should be traversed from right to left (Also, hit 'f' to bring up a list of pages then double-click on a page to open it)
- to un-ignore an item in the scan operation, select it and press the ignore button again
- if you set the root folder (Menu -> Change Root) to the root of your manga folder, the program will try to auto-find the correct folder path when new items are added.
- open 'Menu->About' to automatically check for an updated version
