
#### May 05, 2013
	- Improved performance of GetFromURL()
	- Removed `Newtonsoft.Json.dll`

#### May 04, 2013
	- Added support for Sad Panda galleries
	- Fixed Html char codes not being replaced on import
	- Fixed Edit() improperly deselecting the current gallery
	- Removed `HtmlAgilityPack.dll`
	- Embedded 'Newtonsoft.Json.dll`

#### Apr 28, 2013
	- Added support to exclude terms during searches
	- Form now restores last size & position
	- Listview now has focus on mouse-over

#### Apr 01, 2013
	- Improved display time of tag statistics
	- Small bugfixes

#### Mar 31, 2013
	- Added scrollbar when text exceeds TxBx_Tags length
	- Added error message to rejected folders dropped on listview
	- Fixed OnlyFav() not updating Form title correctly
	- Removed test file: `version.txt`
	- Small bugfixes

#### Feb 02, 2013
	- Added 'Favs Only' option to tag statistics
	- Up/Dn buttons now work even when no entry is selected
	- Tried again to fix c/p font colour issue
	- Small bugfixes

#### Feb 01, 2013
	- Tried to fix invisible text bug when copying/pasting in 'Notes' field
	- Fixed exception when trying to scroll to entry when listview is empty
	- Fixed titlebar of tag statistics form
	- Improved Html parsing

#### Jan 29, 2013
	- Added new version check to the 'About' form
	- Added tag statistics form

#### Jan 28, 2013
	- GetURL() now automatically checks clipboard for EH gallery
	- Small fixes to drag-drop EH gallery title

#### Jan 11, 2013
	- Fixed exception when trying to 'cut' empty text

#### Jan 06, 2013
	- Semi-fixed broken ContextMenu

#### Dec 09, 2012
	- Added ability to browse by filepath while using custom imageviewer by pressing 'f'

#### Dec 03, 2012
	- Added Artist auto-complete
	- Fixed Btn_Loc auto-select code
	- Fixed Up/Dn buttons improper font type

#### Nov 25, 2012
	- Added up/down browsing from View tab
	- Fixed bug in OnlyFavs() by making it additive, rather than subtractive

#### Nov 16, 2012
	- Retargeted .NET 4.0 (sorry about the hiccup)

#### Nov 12, 2012
	- Improved scanning by comparing filepaths, not titles

#### Nov 10, 2012
	- Fixed listview's improper anchoring
	- Updated preview images

#### Nov 07, 2012
	- Fixed GetImage() being called twice on new item selection
	- Improvements to UI
	- Small bugfixes

#### Nov 03, 2012
	- Improved processing of URL input
	- Imageviewer now saves page position on exit
	- Imageviewer can now skip to front/back by using Home/End keys

#### Nov 02, 2012
	- Prevented multiple running instances

#### Nov 01, 2012
	- Embedded `HtmlAgilityPack.dll`
    - Added ability to load entry data from EH gallery URL

#### Oct 26, 2012
	- Fixed tab switch delay
	- Fixed crash when sorting .gif files
	- Revisions to image loading
	- Assorted bugfixes

#### Oct 23, 2012
	- Added drag-drop option to add folders

#### Oct 20, 2012
	- Properly re-formats tags copied from EH

#### Oct 18, 2012
	- Improved memory usage
	- Updated Reset method

#### Oct 16, 2012
	- Fixed icon breaking WinXP compatability

#### Oct 15, 2012
    - Small UI adj. pII

#### Oct 14, 2012
	- Removed sorting redundancy
	- Small UI adjust.

#### Oct 12, 2012
	- Redid threading

#### Oct 11, 2012
	- Updated image browsing

#### Oct 09, 2012
	- Updated save method
    - New entries now use DateTime.Date

#### Oct 08, 2012
	- Forgot to change TxBx_Loc enabling Edit button

#### Oct 07, 2012
	- Allow media-key presses while using imageviewer
    - Prevent 'Edit' button showing on new entries
    - Bugfix for deleting entries with no folder

#### Oct 05, 2012
	- Bugfixed calling index value when no item exists
    - Revamped image traversal code

#### Oct 04, 2012
	- Bugfix for image traversal
	- Removed old files
	- Fixed UI layout

#### Oct 03, 2012
	- Added realtime update of PicBx on manual location edits
    - Bugfix for custom context menu
    - Fixed infinite loop in ShowFavs
    - Added optional in-program imageviewer

#### Oct 02, 2012
    - Initial commit to GitHub
	- (Project started August 15, 2012)

