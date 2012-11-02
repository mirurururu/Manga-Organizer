Manga-Organizer
===============

As a mild digital hoarder, I found trying to keep track of 500+ folders was becoming ridiculous. So instead of an imperfect folder hierarchy, I've been fiddling with writing this program over the last month or so, and I think it's in an acceptable state for release. 

Basically, given folder titles of "[Artist] Title" (E-Hentai's format), it can automatically scan them into a database. After that you can search through the list by artist, title, tags and type. 

If this sounds useful to you, give it a shot: <a href="https://github.com/downloads/Nagru/Manga-Organizer/Manga%20Organizer.exe">Download</a>

For an example of it running see: <a href="https://github.com/Nagru/Manga-Organizer/blob/master/preview.jpg?raw=true" target="_blank">Preview</a>

(You'll need the .NET library to run it, but if you're running Vista/7 it should've come bundled with it. Otherwise, download <a href="https://www.microsoft.com/en-us/download/details.aspx?id=17851">.Net</a> for Windows or <a href="http://www.go-mono.com/mono-downloads/download.html">Mono</a> for Linux/OSX.)


================


Notes:
- no install required
- entry data can be automatically loaded from a g.e-hentai gallery url (exhentai links aren't supported yet)
- searches are inclusive, with terms seperated by a comma
- copy & pasted tags from EH will automatically re-format themselves
- drag or paste a formatted name into the Artist textbox, and it should automatically split it into Artist & Title for you
- browse images with your default image viewer (menu item: Open) or with the built in one (click on the cover image)
- the latter browser displays two images simultaneously and should be traversed from right to left
- to un-ignore an item in the scan operation, select it and press the ignore button again
- if you set the root folder (Menu -> Change Root) to the root of your manga folder, the folderbrowserdialog will try to auto-find the corect folder path when new items are added.