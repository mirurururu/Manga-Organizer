#### v. 3.8.24 (Sep 24, 2013)
	- Fixed bug with trying to overwrite existing image
	- Fixed temp folder sometimes not being removed

#### v. 3.8.22 (Sep 09, 2013)
	- Improved zip file sorting (default was awful)

#### v. 3.8.12 (Sep 06, 2013)
	- .cbz files now treated as .zip files
	- Small bugfix for random manga chooser
	- Small bugfix for page choosing

#### v. 3.8.00 (Sep 04, 2013)
	- Added ability to drag/drop date and page count
	- Added better formatting for large values in title text
	- Fixed auto-fill feature for artist/title
	- Fixed image scaling when choosing new pages
	- Fixed image traversal with zips when choosing new pages
	- Fixed image traversal page value sometimes being one off
	- Small adjustments to tutorial text

#### v. 3.7.63 (Sep 02, 2013)
	- Added tutorial screen for new users
	- Added new Form to manage all settings
	- Added automatic page flipper (no hands mode)
	- Added auto-Artist/Title fill condition when setting location
	- Changed versioning scheme to better reflect new changes
	- Fixed drag/drop of folders marked as "Directory|Archive"
	- Fixed Edit not updating Search result text
	- Double-fix for zip file use

#### v. 3.4.49 (Aug 25, 2013)
	- Fix for zip file browsing
	- Improved entry deletion code

#### v. 3.4.47 (Aug 24, 2013)
	- Added alternating row colors to Scan Form
	- Improved GetURL() efficiency & start-up placement
	- Fixed item removal from Scan listview

#### v. 3.4.44 (Aug 10, 2013)
	- Added pie chart to statistics page
	- Added "&#039;" condition to ReplaceHTML()
	- Improved StarControl efficiency
	- Suppress RichTextBox beeps on reaching start/end

#### v. 3.3.32 (June 30, 2013)
	- Fixed mistake in Scan code when adding new items

#### v. 3.3.31 (June 02, 2013)
	- Fixed exception in GetURL()

#### v. 3.3.21 (May 24, 2013)
	- Fixed alternating item colours not showing when searching
	- Fixed exception in custom imagebrowser

#### v. 3.3.10 (May 23, 2013)
	- Replaced 'Favourite' checkbox with StarControl
	- Changed entry structure to accommodate this
	- Fixed image scaling on wide pictures

#### v. 3.2.00 (May 21, 2013)
	- Added ability to choose PictureBox BackColor
	- Added slight gap between images while browsing
	- Added alternating listview item color
	- Changed listview style to WindowsExplorer
	- Reduced memory usage during image browsing
	- Small image loading improvement

#### v. 3.0.67 (May 20, 2013)
	- Improved listview resizing
	- Fixed 'Random Manga' button from returning the same entry twice in a row
	- Fixed context menu paste not triggering Search event
	- Small code improvements (~500kb less RAM)

#### v. 3.0.55 (May 17, 2013)
	- Improved context menu behaviour
	- Changed picturebox back-colour to same as Notes field
	- Fixed delete operation not updating listview
	- Fixed crash when browsing .zip containing non-images

#### v. 3.0.33 (May 16, 2013)
	- fixed GetUrl() mistake which sent EH metadata to Notes field
	- UI consistency changes

#### v. 3.0.31 (May 15, 2013)
	- Browsing zip files now only extracts pages on a 'need to' basis
	- Scan & Drag/Drop now support zip files
	- Improved Scan efficiency

#### v. 3.0.10 (May 14, 2013)
	- Added tab shortcuts (ctrl + [1-3])
	- Added support for .zip archives
	- Embedded Ionic.Zip.dll

#### v. 2.8.49 (May 13, 2013)
	- Complete Search() re-haul to better reflect EH's
	 (eg. title:celeb_wife date:>01/01/12 -vanilla)
	- Searches now include Description, Date, & Pages fields
	- More LINQ usage
	- Cont. code cleanup

#### v. 2.7.48 (May 12, 2013)
	- Fixed broken "Shell->Open With..." feature
	- Various code cleanup

#### v. 2.7.47 (May 11, 2013)
	- Fixed Scan behaviour
	- Custom imageviewer now displays images side-by-side
	- Minor listview update speed increase
	- Changed entry type from struct to class

#### v. 2.6.45 (May 10, 2013)
	- Fixed WebRequest hangs by setting 'Proxy = null;'
	- Fixed unpolished behaviour with listview focusing
	- Removed aberrant Refocus() call in MnTs_New
	- Improved Scan speed using hashsets

#### v. 2.6.32 (May 09, 2013)
	- Searches now support spaces: replace ' ' with '_'
	- Multiple folders can now be drag-dropped simultaneously
	- Changed Scan code from List<> to Dictionary<> based
	- Removed unnecessary class: Search.cs

#### v. 2.5.12 (May 07, 2013)
	- Fully fixed mixed-font bug
	- Ensure proper listview count in title

#### v. 2.5.10 (May 06, 2013)
	- Added button for random manga selection
	- Fixed display of Japanese Unicode

#### v. 2.4.80 (May 05, 2013)
	- Improved performance of GetFromURL()
	- Removed `Newtonsoft.Json.dll`

#### v. 2.4.79 (May 04, 2013)
	- Added support for Sad Panda galleries
	- Fixed Html char codes not being replaced on import
	- Fixed Edit() improperly deselecting the current gallery
	- Removed `HtmlAgilityPack.dll`
	- Embedded 'Newtonsoft.Json.dll`

#### v. 2.3.77 (Apr 28, 2013)
	- Added support to exclude terms during searches
	- Form now restores last size & position
	- Listview now has focus on mouse-over

#### v. 2.2.57 (Apr 01, 2013)
	- Improved display time of tag statistics
	- Small bugfixes

#### v. 2.2.46 (Mar 31, 2013)
	- Added scrollbar when text exceeds TxBx_Tags length
	- Added error message to rejected folders dropped on listview
	- Fixed OnlyFav() not updating Form title correctly
	- Removed test file: `version.txt`
	- Small bugfixes

#### v. 2.1.34 (Feb 02, 2013)
	- Added 'Favs Only' option to tag statistics
	- Up/Dn buttons now work even when no entry is selected
	- Tried again to fix c/p font colour issue
	- Small bugfixes

#### v. 2.0.22 (Feb 01, 2013)
	- Tried to fix invisible text bug when copying/pasting in 'Notes' field
	- Fixed exception when trying to scroll to entry when listview is empty
	- Fixed titlebar of tag statistics form
	- Improved Html parsing

#### v. 2.0.00 (Jan 29, 2013)
	- Added new version check to the 'About' form
	- Added tag statistics form

#### v. 1.6.68 (Jan 28, 2013)
	- GetURL() now automatically checks clipboard for EH gallery
	- Small fixes to drag-drop EH gallery title

#### v. 1.6.57 (Jan 11, 2013)
	- Fixed exception when trying to 'cut' empty text

#### v. 1.6.47 (Jan 06, 2013)
	- Semi-fixed broken ContextMenu

#### v. 1.6.46 (Dec 09, 2012)
	- Added ability to browse by filepath in custom imageviewer by pressing 'f'

#### v. 1.5.46 (Dec 03, 2012)
	- Added Artist auto-complete
	- Fixed Btn_Loc auto-select code
	- Fixed Up/Down buttons improper font type

#### v. 1.4.44 (Nov 25, 2012)
	- Added up/down browsing from View tab
	- Fixed bug in OnlyFavs() by making it additive, rather than subtractive

#### v. 1.3.34 (Nov 16, 2012)
	- Re-targeted .NET 4.0 (sorry about the hiccup)

#### v. 1.3.33 (Nov 12, 2012)
	- Improved scanning by comparing filepaths, not titles

#### v. 1.2.33 (Nov 10, 2012)
	- Fixed listview's improper anchoring
	- Updated preview images

#### v. 1.2.22 (Nov 07, 2012)
	- Fixed GetImage() being called twice on new item selection
	- Improvements to UI
	- Small bugfixes

#### v. 1.2.10 (Nov 03, 2012)
	- Improved processing of URL input
	- Imageviewer now saves page position on exit
	- Imageviewer can now skip to front/back by using Home/End keys

#### v. 1.1.00 (Nov 02, 2012)
	- Prevented multiple running instances

#### v. 1.0.00 (Nov 01, 2012)
	- Embedded `HtmlAgilityPack.dll`
    - Added ability to load entry data from EH gallery URL

#### v. 0.9.21 (Oct 26, 2012)
	- Fixed tab switch delay
	- Fixed crash when sorting .gif files
	- Revisions to image loading
	- Assorted bugfixes

#### v. 0.7.99 (Oct 23, 2012)
	- Added drag-drop option to add folders

#### v. 0.6.99 (Oct 20, 2012)
	- Properly re-formats tags copied from EH

#### v. 0.6.89 (Oct 18, 2012)
	- Improved memory usage
	- Updated Reset method

#### v. 0.5.79 (Oct 16, 2012)
	- Fixed new icon breaking WinXP compatibility

#### v. 0.5.69 (Oct 15, 2012)
    - Small UI adjustment pII

#### v. 0.5.68 (Oct 14, 2012)
	- Removed sorting redundancy
	- Small UI adjust.

#### v. 0.5.57 (Oct 12, 2012)
	- Redid threading

#### v. 0.4.57 (Oct 11, 2012)
	- Updated image browsing

#### v. 0.3.57 (Oct 09, 2012)
	- Updated save method
    - New entries now use DateTime.Date

#### v. 0.2.47 (Oct 08, 2012)
	- Forgot to change TxBx_Loc enabling Edit button

#### v. 0.2.46 (Oct 07, 2012)
	- Allow media-key presses while using imageviewer
    - Prevent 'Edit' button showing on new entries
    - Bugfix for deleting entries with no folder

#### v. 0.2.35 (Oct 05, 2012)
	- Revamped image traversal code
	- Bugfixed calling index value when no item exists

#### v. 0.2.24 (Oct 04, 2012)
	- Bugfix for image traversal
	- Removed old files
	- Fixed UI layout

#### v. 0.2.12 (Oct 03, 2012)
	- Added real-time update of PicBx on manual location edits
	- Added optional in-program imageviewer
    - Bugfix for custom context menu
    - Fixed infinite loop in ShowFavs

#### v. 0.0.01 (Oct 02, 2012)
    - Initial commit to GitHub
	- (Project started August 15, 2012)
