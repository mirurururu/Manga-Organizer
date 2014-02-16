
-- Create Table: Artist
-------------------------------------------
create table [Artist]
(
	ArtistID	integer			primary key		autoincrement
	,Name		nvarchar(100)	not null		unique
)


-- Create Table: Tag
-------------------------------------------
create table [Tag]
(
	TagID		integer			primary key		autoincrement
	,Tag		nvarchar(100)	not null		unique
)


-- Create Table: Type
create table [Type]
(
	TypeID		integer			primary key		autoincrement
	,Type		nvarchar(100)	not null		unique
)


-- Create Table: MangaArtist
-------------------------------------------
create table [MangaArtist]
(
	MangaArtistID	integer		primary key		autoincrement
	,MangaID		int			not null
	,ArtistID		int			not null
	,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
	,constraint [fk_artistID] foreign key ([ArtistID]) references [Artist] ([ArtistID])
)


-- Create Table: MangaTag
-------------------------------------------
create table [MangaTag]
(
	MangaTagID		integer		primary key		autoincrement
	,MangaID		int			not null
	,TagID			int			not null
	,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
	,constraint [fk_tagID] foreign key ([TagID]) references [Tag] ([TagID])
)


-- Create Table: Manga
-------------------------------------------
create table [Manga]
(
	MangaID			integer			primary key		autoincrement
	,TypeID			int				null
	,Title			nvarchar(250)	not null		unique
	,Pages			int				not null
	,Rating			decimal(1,1)	not null
	,Description	nvarchar(4000)	null
	,Location		nvarchar(260)	null			-- MAX_PATH Windows API value
	,GalleryURL		nvarchar(40)	null
	,PublishedDate	date			null
	,CreatedDBTime	datetime		not null		default CURRENT_DATE
	,AuditDBTime	datetime		not null		default CURRENT_DATE
	,constraint [fk_typeID] foreign key ([TypeID]) references [Type] ([TypeID])
)
