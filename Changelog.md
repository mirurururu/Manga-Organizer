#### August 10, 2013
	- Added pie chart to statistics page
	- Added &#039; condition to ReplaceHTML()
	- Improved StarControl efficiency
	- Suppress RichTextBox beeps on reaching start/end

#### June 30, 2013
	- Fixed mistake in Scan code when adding new items

#### June 02, 2013
	- Fixed exception in GetURL()

#### May 24, 2013
	- Fixed alternating item colours not showing when searching
	- Fixed exception in custom imagebrowser

#### May 23, 2013
	- Replaced 'Favourite' checkbox with StarControl
	- Changed entry structure to accommodate this
	- Fixed image scaling on wide pictures

#### May 21, 2013
	- Added ability to choose PictureBox BackColor
	- Added slight gap between images while browsing
	- Added alternating listview item color
	- Chnaged listview style to WindowsExplorer
	- Reduced memory usage during image browsing
	- Small image loading improvement

#### May 20, 2013
	- Improved listview resizing
	- Fixed 'Random Manga' button from returning the same entry twice in a row
	- Fixed context menu paste not triggering Search event
	- Small code improvements (500kb less RAM, yay...)

#### May 17, 2013
	- Improved context menu behaviour
	- Changed picturebox back-colour to same as Notes field
	- Fixed delete operation not updating listview
	- Fixed crash when browsing .zip containing non-images

#### May 16, 2013
	- UI consistency changes
	- fixed GetUrl() mistake which sent EH metadata to Notes field

#### May 15, 2013
	- Browsing zip files now only extracts pages on a 'need to' basis
	- Scan & Drag/Drop now support zip files
	- Improved Scan efficiency

#### May 14, 2013
	- Added tab shortcuts (ctrl + [1-3])
	- Added support for .zip archives
	- Embedded Ionic.Zip.dll

#### May 13, 2013
	- Complete Search() rehaul to better reflect EH's
	 (eg. title:celeb_wife date:>01/01/12 -vanilla)
	- Searches now include Description, Date, & Pages fields
	- More LINQ usage
	- Cont. code cleanup

#### May 12, 2013
	- Fixed broken "Shell->Open With..." feature
	- Various code cleanup

#### May 11, 2013
	- Fixed Scan behaviour
	- Custom imageviewer now displays images side-by-side
	- Minor listview update speed increase
	- Changed entry type from struct to class

#### May 10, 2013
	- Fixed WebRequest hangs by setting 'Proxy = null;'
	- Fixed unpolished behaviour with listview focusing
	- Removed aberrant Refocus() call in MnTs_New
	- Improved Scan speed using hashsets

#### May 09, 2013
	- Searches now support spaces: replace ' ' with '_'
	- Multiple folders can now be drag-dropped simultaneously
	- Changed Scan code from List<> to Dictionary<> based
	- Removed unnecessary class: Search.cs

#### May 07, 2013
	- Fully fixed mixed-font bug
	- Ensure proper listview count in title

#### May 06, 2013
	- Added button for random doujin selection
	- Fixed display of japanese unicode

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
	- Fixed new icon breaking WinXP compatability

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

