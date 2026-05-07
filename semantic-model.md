# Semantic Model

## Entities

### Genre
- `Id` - primary key
- `Name`
- `Description`
- `Bands` - one genre can have many bands

### Band
- `Id` - primary key
- `Name`
- `FormedYear`
- `Country`
- `Description`
- `GenreId` - foreign key to `Genre`
- `Genre` - many bands belong to one genre
- `Albums` - one band can have many albums

### Album
- `Id` - primary key
- `Title`
- `ReleaseYear`
- `BandId` - foreign key to `Band`
- `Band` - many albums belong to one band
- `Songs` - one album can have many songs

### Song
- `Id` - primary key
- `Title`
- `DurationSeconds`
- `AlbumId` - foreign key to `Album`
- `Album` - many songs belong to one album
- `OpeningLyric`
- `IsDailyQuizSong`
- `AudioSnippetUrl`
- `AlbumCoverUrl`
- `PlaylistSongs` - junction rows for playlists
- `QuizRounds` - one song can appear in quiz rounds

### QuizRound
- `Id` - primary key
- `SongId` - foreign key to `Song`
- `Song` - one quiz round references one song
- `Hints` - one quiz round can have many hints

### Hint
- `Id` - primary key
- `QuizRoundId` - foreign key to `QuizRound`
- `QuizRound` - many hints belong to one quiz round
- `Type`
- `Order`
- `Content`

### UserPlaylist
- `Id` - primary key
- `Name`
- `OwnerUserName`
- `Description`
- `CreatedAt`
- `IsPublic`
- `Likes`
- `PlaylistSongs` - one playlist can have many playlist-song rows

### PlaylistSong
- `PlaylistId` - foreign key to `UserPlaylist`
- `Playlist` - many rows belong to one playlist
- `SongId` - foreign key to `Song`
- `Song` - many rows can point to one song
- `AddedAt`

## Relationship Summary

- `Genre` 1 -> N `Band`
- `Band` 1 -> N `Album`
- `Album` 1 -> N `Song`
- `Song` 1 -> N `QuizRound`
- `QuizRound` 1 -> N `Hint`
- `UserPlaylist` 1 -> N `PlaylistSong`
- `Song` 1 -> N `PlaylistSong`

## Notes

- The current UI still reads from mock repositories, but the EF-ready model matches the domain structure used by the app.
- `Song` currently uses `Album` as the source of band and genre data in the views and quiz logic.