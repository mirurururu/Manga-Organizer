Manga-Organizer
===============

As a mild digital hoarder, I found trying to keep track of 500+ folders was becoming ridiculous. So instead of an imperfect folder hierarchy, I've been fiddling with writing this program over the last month or so, and I think it's in an acceptable state for release. 

Basically, given folder titles of "[Artist] Title" (E-Hentai's format), it can automatically scan them into a database. After that you can search through the list by artist, title, tags and type. 

You'll need the .NET library to run it, but if you're running Vista/7 it should've come bundled with it. If it sounds useful to you, give it a shot. See the "Downloads" button ->

For an example of it running see:
http://imageshack.us/a/img696/4306/92024725.jpg


================


Notes:
- searches are inclusive, with terms seperated by a comma
- drag a formatted name into the Artist textbox, and it should automatically split it into Artist & Title for you
- browse images with your default image viewer (menu item: Open) or with the built in one (click on the cover image)
- the latter browser displays two images simultaneously and should be traversed from right to left
- to un-ignore an item in the scan operation, select it and press the ignore button again
- if you set the root folder (Menu -> Change Root) to the root of your manga folder, the folderbrowserdialog will try to auto-find the corect folder path when new items are added.