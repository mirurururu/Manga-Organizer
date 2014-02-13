begin transaction

-- Create Schema
create database MangaOrganizer
go

use MangaOrganizer
go


-- Create Table: Artist
-------------------------------------------
create table Artist
(
	ArtistID	int				not null	identity(0,1)
	,Name		nvarchar(100)	not null
)
on [primary]
go

alter table Artist add constraint
pk_artist primary key clustered (ArtistID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: Tag
-------------------------------------------
create table Tag
(
	TagID		int				not null	identity(0,1)
	,Tag		nvarchar(100)	not null
)
on [primary]
go

alter table Tag add constraint
pk_tag primary key clustered (TagID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: Type
create table Type
(
	TypeID		int				not null	identity(0,1)
	,Type		int				not null
)
on [primary]
go

alter table Type add constraint
pk_type primary key clustered (TypeID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: MangaArtist
-------------------------------------------
create table MangaArtist
(
	MangaArtistID	int			not null	identity(0,1)
	,MangaID		int			not null
	,ArtistID		int			not null
)
on [primary]
go

alter table MangaArtist add constraint
pk_mangaArtist primary key clustered (MangaArtistID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: MangaTag
-------------------------------------------
create table MangaTag
(
	MangaTagID		int			not null	identity(0,1)
	,MangaID		int			not null
	,TagID			int			not null
)
on [primary]
go

alter table MangaTag add constraint
pk_mangaTag primary key clustered (MangaTagID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Table: Manga
-------------------------------------------
create table Manga
(
	MangaID			int				not null	identity(0,1)
	,TypeID			int				null
	,Title			nvarchar(250)	not null
	,Pages			int				not null
	,Rating			decimal(1,1)	not null
	,Description	nvarchar(4000)	null
	,Location		nvarchar(500)	null
	,GalleryURL		nvarchar(40)	null
	,PublishedDate	date			null
	,CreatedDBTime	datetime		not null
	,AuditDBTime	datetime		not null
)
on [primary]
go

alter table Manga add constraint
pk_manga primary key clustered (MangaID)
with(statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]


-- Create Foreign Key: mangaArtist.mangaID -> manga.mangaID
alter table [MangaArtist] add constraint [fk_mangaID]
foreign_key ([MangaID])
references [Manga] ([MangaID])
on update no action
on delete no action

-- Create Foreign Key: mangaArtist.artistID -> artist.artistID
alter table [MangaArtist] add constraint [fk_artistID]
foreign_key ([ArtistID])
references [Artist] ([ArtistID])
on update no action
on delete no action

-- Create Foreign Key: mangaTag.mangaID -> manga.mangaID
alter table [MangaTag] add constraint [fk_mangaID]
foreign_key ([MangaID])
references [Manga] ([MangaID])
on update no action
on delete no action

-- Create Foreign Key: mangaTag.tagID -> tag.tagID
alter table [MangaTag] add constraint [fk_tagID]
foreign_key ([TagID])
references [Tag] ([TagID])
on update no action
on delete no action

-- Create Foreign Key: manga.typeID -> type.typeID
alter table [Manga] add constraint [fk_typeID]
foreign_key ([TypeID])
references [Type] ([TypeID])
on update no action
on delete no action

commit transaction