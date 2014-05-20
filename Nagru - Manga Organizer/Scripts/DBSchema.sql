
/*
-- Create Table: Artist
-------------------------------------------
create table [Artist]
(
	ArtistID				integer			primary key		autoincrement
	,Name						text				not null			unique
	,Psuedonym			text				null
	,CreatedDBTime	text				not null			default CURRENT_TIMESTAMP
	,AuditDBTime		text				not null			default CURRENT_TIMESTAMP
)


-- Create Table: Tag
-------------------------------------------
create table [Tag]
(
	TagID						integer			primary key		autoincrement
	,Tag						text				not null			unique
	,CreatedDBTime	text				not null			default CURRENT_TIMESTAMP
	,AuditDBTime		text				not null			default CURRENT_TIMESTAMP
)


-- Create Table: Type
-------------------------------------------
create table [Type]
(
	TypeID					integer		primary key		autoincrement
	,Type						text			not null			unique
	,CreatedDBTime	text			not null			default CURRENT_TIMESTAMP
	,AuditDBTime		text			not null			default CURRENT_TIMESTAMP
)


-- Create Table: MangaArtist
-------------------------------------------
create table [MangaArtist]
(
	MangaArtistID		integer		primary key		autoincrement
	,MangaID				integer		not null
	,ArtistID				integer		not null
	,CreatedDBTime	text			not null		default CURRENT_TIMESTAMP
	,AuditDBTime		text			not null		default CURRENT_TIMESTAMP
	,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
	,constraint [fk_artistID] foreign key ([ArtistID]) references [Artist] ([ArtistID])
)


-- Create Table: MangaTag
-------------------------------------------
create table [MangaTag]
(
	MangaTagID			integer		primary key		autoincrement
	,MangaID				integer		not null
	,TagID					integer		not null
	,CreatedDBTime	text			not null		default CURRENT_TIMESTAMP
	,AuditDBTime		text			not null		default CURRENT_TIMESTAMP
	,constraint [fk_mangaID] foreign key ([MangaID]) references [Manga] ([MangaID])
	,constraint [fk_tagID] foreign key ([TagID]) references [Tag] ([TagID])
)


-- Create Table: Manga
-------------------------------------------
create table [Manga]
(
	MangaID					integer			primary key		autoincrement
	,TypeID					integer			null
	,Title					text				not null
	,Pages					integer			not null			default		0
	,Rating					numeric			not null			default		0
	,Description		text				null
	,Location				text				null
	,GalleryURL			text				null
	,PublishedDate	text				null
	,CreatedDBTime	text				not null			default CURRENT_TIMESTAMP
	,AuditDBTime		text				not null			default CURRENT_TIMESTAMP
	,constraint [fk_typeID] foreign key ([TypeID]) references [Type] ([TypeID])
)
*/