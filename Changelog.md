### v. 4.6.57 (November 23, 2014)
  - Added temporary 'fix' for cases of scanning breaking until I can get some details on why
  - Fixed issue when searching through a manga entry with null values

### v. 4.6.55 (November 18, 2014)
  - Added new function to return all manga with missing sources

### v. 4.6.54 (November 06, 2014)
  - bem13: Fixed parsing gallery details when in a culture that uses commas instead of periods for numbers

#### v. 4.6.53 (November 03, 2014)
   - bem13: Can now traverse images with A/D
   - bem13: Maintain window state between sessions

#### v. 4.6.51 (October 19, 2014)
  - Prevented an exception when trying to open encrypted archives

#### v. 4.6.50 (October 12, 2014)
  - Improved the pie chart on the tag statistics screen
  - Updated the scrollbar handling

#### v. 4.6.48 (October 03, 2014)
  - Chose a new compromise with tagging (small speed improvement)
  - Added better error handling for querying the EH API

#### v. 4.6.46 (September 29, 2014)
  - Added the ability to convert folders to .cbz's
  - Re-did the Settings form code-behind
  - Improved the load speed of the page-browser
  - Improved the speed of adding entries by Scanning
  - Slightly improved scanning speed
  - Updated the ListView to have a 'VS designer' mode

#### v. 4.6.30 (September 26, 2014)
  - Fixed an issue where closing the scan form, mid-scanning, would cause an exception
  - Updated a filepath check to be case insensitive

#### v. 4.6.28 (September 23, 2014)
  - Updated the drag-drop tag code to match EHs new format, auto-sort, and ignore duplicates

#### v. 4.6.27 (September 22, 2014)
	- Updated to prevent gibberish EH memberIDs
	- Removed the error messaging code
	- Other house-cleaning

#### v. 4.6.25 (July 30, 2014)
	- Fixed issue with sorting listview columns
	- Ensured column highlighting is properly respected when not using the grid display mode
	- Other small updates

#### v. 4.6.21 (July 25, 2014)
	- Fixed check when looking for upgraded DB versions. '!=' should have been '=='.

#### v. 4.6.20 (July 21, 2014)
	- Improved the 'random manga' button to be a 'shuffle' operation
	- Improved filename retrieving code
	- Improved how manga are saved
	- Updated how HTML is decoded
	- Updated automatic title splitting code
	- Fixed tags not being displayed sorted
	- Moved most settings into the database
	- General code cleanup

#### v. 4.5.04 (June 6, 2014)
	- Fixed sorting by percentage in the tag stats viewer

#### v. 4.5.03 (June 4, 2014)
	- Fixed sorting by dates
	- Improved sorting speed by a couple nanoseconds

#### v. 4.5.02 (June 1, 2014)
	- Minor bugfixes for deleting entries, copying titles, etc

#### v. 4.5.00 (May 22, 2014)
	- Switched data storage from serialization to Sqlite
	- Remapped 'search EH' shortcut to Alt+X
	
#### v. 4.4.36 (April 13, 2014)
	- Added image previews to the built-in imageviewer's page-browser (press 'f')
	
#### v. 4.3.36 (March 30, 2014)
	- Added multi-monitor support to the built-in imageviewer
	- Gallery type selections are now saved for searches
	- Minor improvements to auto-file finding

#### v. 4.3.33 (March 15, 2014)
	- Bugfix for EH search when there are multiple results with the same title
	- Newly added artists will now show up in future auto-completions without needing a reload
	- Minor improvement to mass file scanning

#### v. 4.3.30 (February 11, 2014)
	- Fix for the program crashing when scanning a corrupted archive
	- Added shortcut tooltip to 'Search EH' function

#### v. 4.3.28 (January 24, 2014)
	- Fix for searches not respecting generic parameters
	
#### v. 4.3.27 (January 18, 2014)
	- Stopped EH search from treating no-results as an error (sorry about that)
	- Added option to choose which gallery types EH-Search looks through
	- Small behaviour change to textboxes with a scrollbar
		(when switching from shown to hidden, the code will try to pull the full text into view)

#### v. 4.3.15 (January 4, 2014)
	- Improved search listview resizing
	- Made auto-search formatting visible and improved it
	- Tweaked listview row colours
	- Small code clean-up

#### v. 4.3.12 (January 1, 2014)
	- Improved relative file-paths
	- Improved EH searching (should return *all* results now)
	- EH search Form 'select' button disabled when no item selected
	- Added short-form tag aliases (ie "artist:" == "a:")

#### v. 4.3.00 (December 29, 2013)
	- Added ability to search EH website from within the application
	- Small bugfixes and improvements

#### v. 4.1.89 (December 5, 2013)
	- Fixed tag scrolling sometimes throwing an exception
	
#### v. 4.1.88 (Nov 21, 2013)
	- Improved performance of scanning
	- Fixed file extension remaining in title
	- Changed shortcut keys of menu items 

#### v. 4.1.77 (Nov 08, 2013)
	- Changed tag fetching to only set Artist/Title when the field is blank
	- Also changed it to append, not overwrite, any existing tags on fetch
	
#### v. 4.1.75 (Nov 07, 2013)
	- Added ability to choose external image viewer (check settings)
	- Added ability to reset settings individually
	- Small bugfixes

#### v. 4.1.63 (Nov 03, 2013)
	- Made Date column optional (check settings)
	- Small bugfix for title parsing (pre-fill array with empty strings)
	- Small code improvements

#### v. 4.1.52 (Nov 02, 2013)
	- Added Date column (MM-DD-YY)
	- Improved handling of opening files/folders with external programs
	- Fixed title parsing breaking when first ']' is the last character

#### v. 4.1.40 (Nov 01, 2013)
	- Added (beta) relative file-paths
	- Improved artist/title splitting code again
	- Gave entry saving button more accurate title
	- Fixed drag/drop allowing non-folders/archives when paired with them
	- Fixed Scan not seeing rar/cbr and nested folders

#### v. 4.1.26 (Oct 28, 2013)
	- Updated artist/title formatting behaviour
	
#### v. 4.1.25 (Oct 27, 2013)
	- Fixed FavsOnly bug where it wouldn't draw properly if the list was partially scrolled

#### v. 4.1.24 (Oct 26, 2013)
	- Added ability to select tag suggestions using the mouse
	- Other small usability improvements to tag suggestions
	
#### v. 4.1.13 (Oct 21, 2013)
	- Fixed broken Statistics page "Favs Only" function

#### v. 4.1.12 (Oct 19, 2013)
	- Added merged File/Folder browser
	- Fixed drag/drop text into search bar adding text to artist/title fields
	- Fixed tag field's scrollbar not updating when new term added
	- Fixed image scaling when image is undersized

#### v. 4.0.00 (Oct 11, 2013)
	- Replaced Ionic.Zip with SharpCompress
	- Now has RAR & 7-zip support
	- Improved auto-magic file/folder selection

#### v. 3.9.59 (Oct 08, 2013)
	- Added drag/drop to search bar & auto-formatting of titles
	- Improved tag scrollbar behaviour
	- Statistics code optimization
 
#### v. 3.9.47 (Oct 07, 2013)
	- Added tag auto-completion
	- Improved tag parsing code
	- Improved raw title parsing
	- Switched to .NET4 Client Profile

#### v. 3.8.36 (Oct 04, 2013)
	- Fixed bug when accessing zip non-explicitly

#### v. 3.8.35 (Oct 03, 2013)
	- Added term chaining (eg. tag:bride&vanilla)
	- Better behaviour for overfull scrollbar

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
