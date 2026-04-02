<<<<<<< HEAD
using Riffdle.Models.Lab1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await RunLab1DemoAsync();

app.Run();

static async Task RunLab1DemoAsync()
{
    var bands = new List<Band>
    {
        new() { Id = 1, Name = "Mayhem", Country = "Norway", FormedYear = 1984, IsActive = true, PrimaryStyle = "Black Metal", CreatedAt = new DateTime(1984, 1, 1) },
        new() { Id = 2, Name = "Death", Country = "USA", FormedYear = 1983, IsActive = false, PrimaryStyle = "Death Metal", CreatedAt = new DateTime(1983, 1, 1) },
        new() { Id = 3, Name = "Opeth", Country = "Sweden", FormedYear = 1989, IsActive = true, PrimaryStyle = "Progressive Death Metal", CreatedAt = new DateTime(1989, 1, 1) }
    };

    var songs = new List<Song>
    {
        new() { Id = 1, Title = "Freezing Moon", Genre = MetalGenre.BlackMetal, ReleaseDate = new DateTime(1994, 5, 16), LyricsSnippet = "Everything here is so cold...", AudioSnippetUrl = "/audio/freezing-moon.mp3", AlbumCoverUrl = "/img/de-mysteriis.jpg", DurationSeconds = 386, BandId = 1 },
        new() { Id = 2, Title = "Chainsaw Gutsfuck", Genre = MetalGenre.BlackMetal, ReleaseDate = new DateTime(1987, 12, 7), LyricsSnippet = "No one can stop this...", AudioSnippetUrl = "/audio/chainsaw-gutsfuck.mp3", AlbumCoverUrl = "/img/deathcrush.jpg", DurationSeconds = 241, BandId = 1 },
        new() { Id = 3, Title = "Pagan Fears", Genre = MetalGenre.BlackMetal, ReleaseDate = new DateTime(1994, 5, 16), LyricsSnippet = "The bloody history from the past...", AudioSnippetUrl = "/audio/pagan-fears.mp3", AlbumCoverUrl = "/img/de-mysteriis.jpg", DurationSeconds = 367, BandId = 1 },
        new() { Id = 4, Title = "Pull the Plug", Genre = MetalGenre.DeathMetal, ReleaseDate = new DateTime(1988, 5, 25), LyricsSnippet = "Life ends so fast...", AudioSnippetUrl = "/audio/pull-the-plug.mp3", AlbumCoverUrl = "/img/leprosy.jpg", DurationSeconds = 267, BandId = 2 },
        new() { Id = 5, Title = "Crystal Mountain", Genre = MetalGenre.DeathMetal, ReleaseDate = new DateTime(1995, 8, 22), LyricsSnippet = "Inside crystal mountain evil takes its form...", AudioSnippetUrl = "/audio/crystal-mountain.mp3", AlbumCoverUrl = "/img/symbolic.jpg", DurationSeconds = 309, BandId = 2 },
        new() { Id = 6, Title = "Spirit Crusher", Genre = MetalGenre.DeathMetal, ReleaseDate = new DateTime(1998, 5, 5), LyricsSnippet = "It comes from the depths...", AudioSnippetUrl = "/audio/spirit-crusher.mp3", AlbumCoverUrl = "/img/sound-of-perseverance.jpg", DurationSeconds = 404, BandId = 2 },
        new() { Id = 7, Title = "Ghost of Perdition", Genre = MetalGenre.ProgressiveMetal, ReleaseDate = new DateTime(2005, 8, 30), LyricsSnippet = "Ghost of mother, lingering death...", AudioSnippetUrl = "/audio/ghost-of-perdition.mp3", AlbumCoverUrl = "/img/ghost-reveries.jpg", DurationSeconds = 626, BandId = 3 },
        new() { Id = 8, Title = "Deliverance", Genre = MetalGenre.ProgressiveMetal, ReleaseDate = new DateTime(2002, 11, 4), LyricsSnippet = "Deliverance, thrown back at me...", AudioSnippetUrl = "/audio/deliverance.mp3", AlbumCoverUrl = "/img/deliverance.jpg", DurationSeconds = 816, BandId = 3 },
        new() { Id = 9, Title = "Bleak", Genre = MetalGenre.ProgressiveMetal, ReleaseDate = new DateTime(2001, 3, 6), LyricsSnippet = "Devious movements in your eyes...", AudioSnippetUrl = "/audio/bleak.mp3", AlbumCoverUrl = "/img/blackwater-park.jpg", DurationSeconds = 554, BandId = 3 }
    };

    foreach (var song in songs)
    {
        song.Band = bands.Single(b => b.Id == song.BandId);
        song.Band.Songs.Add(song);
    }

    var quizzes = new List<Quiz>
    {
        new() { Id = 1, Name = "Nordic Darkness", Description = "Guess iconic black metal tracks.", CreatedAt = DateTime.UtcNow, IsRanked = true, MaxScore = 300, TimeLimitSeconds = 180 },
        new() { Id = 2, Name = "Old School Death", Description = "Classic death metal challenge.", CreatedAt = DateTime.UtcNow, IsRanked = true, MaxScore = 300, TimeLimitSeconds = 180 },
        new() { Id = 3, Name = "Progressive Extreme", Description = "Long, complex, progressive extreme metal songs.", CreatedAt = DateTime.UtcNow, IsRanked = false, MaxScore = 300, TimeLimitSeconds = 240 }
    };

    quizzes[0].Rounds.Add(CreateRound(1, quizzes[0], songs[0]));
    quizzes[0].Rounds.Add(CreateRound(2, quizzes[0], songs[1]));
    quizzes[0].Rounds.Add(CreateRound(3, quizzes[0], songs[2]));

    quizzes[1].Rounds.Add(CreateRound(4, quizzes[1], songs[3]));
    quizzes[1].Rounds.Add(CreateRound(5, quizzes[1], songs[4]));
    quizzes[1].Rounds.Add(CreateRound(6, quizzes[1], songs[5]));

    quizzes[2].Rounds.Add(CreateRound(7, quizzes[2], songs[6]));
    quizzes[2].Rounds.Add(CreateRound(8, quizzes[2], songs[7]));
    quizzes[2].Rounds.Add(CreateRound(9, quizzes[2], songs[8]));

    var playlists = new List<UserPlaylist>
    {
        new() { Id = 1, Name = "Warmup Brutality", OwnerUserName = "noah", Description = "Short warmup playlist for quiz training.", CreatedAt = DateTime.UtcNow, IsPublic = true, Likes = 12 },
        new() { Id = 2, Name = "Technical Marathon", OwnerUserName = "noah", Description = "Long songs for expert listeners.", CreatedAt = DateTime.UtcNow, IsPublic = false, Likes = 5 }
    };

    LinkPlaylistSong(playlists[0], songs[1]);
    LinkPlaylistSong(playlists[0], songs[4]);
    LinkPlaylistSong(playlists[0], songs[6]);
    LinkPlaylistSong(playlists[1], songs[6]);
    LinkPlaylistSong(playlists[1], songs[7]);
    LinkPlaylistSong(playlists[1], songs[8]);

    var blackMetalAfter1990 = songs
        .Where(s => s.Genre == MetalGenre.BlackMetal && s.ReleaseDate.Year > 1990)
        .OrderBy(s => s.ReleaseDate)
        .ToList();

    var topBandsBySongCount = bands
        .OrderByDescending(b => b.Songs.Count)
        .Select(b => new { b.Name, SongCount = b.Songs.Count })
        .ToList();

    var publicPlaylistsWithDeathMetal = playlists
        .Where(p => p.IsPublic && p.PlaylistSongs.Any(ps => ps.Song?.Genre == MetalGenre.DeathMetal))
        .OrderByDescending(p => p.Likes)
        .ToList();

    var loadedTrack = await LoadTrackAsync(songs, 5);

    Console.WriteLine($"Lab 1 demo loaded. Quizzes: {quizzes.Count}");
    Console.WriteLine($"Black metal songs after 1990: {blackMetalAfter1990.Count}");
    Console.WriteLine($"Top band by catalog size: {topBandsBySongCount.First().Name}");
    Console.WriteLine($"Public playlists containing death metal: {publicPlaylistsWithDeathMetal.Count}");
    Console.WriteLine($"Async loaded track: {loadedTrack?.Title}");
}

static QuizRound CreateRound(int roundId, Quiz quiz, Song song)
{
    var round = new QuizRound
    {
        Id = roundId,
        QuizId = quiz.Id,
        Quiz = quiz,
        SongId = song.Id,
        Song = song,
        RoundNumber = quiz.Rounds.Count + 1,
        PointsForCorrectGuess = 100,
        MaxHintsAvailable = 5,
        Hints = new List<Hint>
        {
            new() { Id = roundId * 10 + 1, QuizRoundId = roundId, Type = HintType.Genre, Order = 1, Content = song.Genre.ToString() },
            new() { Id = roundId * 10 + 2, QuizRoundId = roundId, Type = HintType.Lyrics, Order = 2, Content = song.LyricsSnippet },
            new() { Id = roundId * 10 + 3, QuizRoundId = roundId, Type = HintType.AudioSnippet, Order = 3, Content = song.AudioSnippetUrl },
            new() { Id = roundId * 10 + 4, QuizRoundId = roundId, Type = HintType.AlbumCover, Order = 4, Content = song.AlbumCoverUrl },
            new() { Id = roundId * 10 + 5, QuizRoundId = roundId, Type = HintType.BandName, Order = 5, Content = song.Band?.Name ?? string.Empty }
        }
    };

    song.QuizRounds.Add(round);
    return round;
}

static void LinkPlaylistSong(UserPlaylist playlist, Song song)
{
    var relation = new PlaylistSong
    {
        PlaylistId = playlist.Id,
        Playlist = playlist,
        SongId = song.Id,
        Song = song,
        AddedAt = DateTime.UtcNow
    };

    playlist.PlaylistSongs.Add(relation);
    song.PlaylistSongs.Add(relation);
}

static async Task<Song?> LoadTrackAsync(IEnumerable<Song> songs, int songId)
{
    await Task.Delay(200);
    return songs.FirstOrDefault(s => s.Id == songId);
}
=======
using Riffdle.Models.Lab1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await RunLab1DemoAsync();

app.Run();

static async Task RunLab1DemoAsync()
{
    var bands = new List<Band>
    {
        new() { Id = 1, Name = "Mayhem", Country = "Norway", FormedYear = 1984, IsActive = true, PrimaryStyle = "Black Metal", CreatedAt = new DateTime(1984, 1, 1) },
        new() { Id = 2, Name = "Death", Country = "USA", FormedYear = 1983, IsActive = false, PrimaryStyle = "Death Metal", CreatedAt = new DateTime(1983, 1, 1) },
        new() { Id = 3, Name = "Opeth", Country = "Sweden", FormedYear = 1989, IsActive = true, PrimaryStyle = "Progressive Death Metal", CreatedAt = new DateTime(1989, 1, 1) }
    };

    var songs = new List<Song>
    {
        new() { Id = 1, Title = "Freezing Moon", Genre = MetalGenre.BlackMetal, ReleaseDate = new DateTime(1994, 5, 16), LyricsSnippet = "Everything here is so cold...", AudioSnippetUrl = "/audio/freezing-moon.mp3", AlbumCoverUrl = "/img/de-mysteriis.jpg", DurationSeconds = 386, BandId = 1 },
        new() { Id = 2, Title = "Chainsaw Gutsfuck", Genre = MetalGenre.BlackMetal, ReleaseDate = new DateTime(1987, 12, 7), LyricsSnippet = "No one can stop this...", AudioSnippetUrl = "/audio/chainsaw-gutsfuck.mp3", AlbumCoverUrl = "/img/deathcrush.jpg", DurationSeconds = 241, BandId = 1 },
        new() { Id = 3, Title = "Pagan Fears", Genre = MetalGenre.BlackMetal, ReleaseDate = new DateTime(1994, 5, 16), LyricsSnippet = "The bloody history from the past...", AudioSnippetUrl = "/audio/pagan-fears.mp3", AlbumCoverUrl = "/img/de-mysteriis.jpg", DurationSeconds = 367, BandId = 1 },
        new() { Id = 4, Title = "Pull the Plug", Genre = MetalGenre.DeathMetal, ReleaseDate = new DateTime(1988, 5, 25), LyricsSnippet = "Life ends so fast...", AudioSnippetUrl = "/audio/pull-the-plug.mp3", AlbumCoverUrl = "/img/leprosy.jpg", DurationSeconds = 267, BandId = 2 },
        new() { Id = 5, Title = "Crystal Mountain", Genre = MetalGenre.DeathMetal, ReleaseDate = new DateTime(1995, 8, 22), LyricsSnippet = "Inside crystal mountain evil takes its form...", AudioSnippetUrl = "/audio/crystal-mountain.mp3", AlbumCoverUrl = "/img/symbolic.jpg", DurationSeconds = 309, BandId = 2 },
        new() { Id = 6, Title = "Spirit Crusher", Genre = MetalGenre.DeathMetal, ReleaseDate = new DateTime(1998, 5, 5), LyricsSnippet = "It comes from the depths...", AudioSnippetUrl = "/audio/spirit-crusher.mp3", AlbumCoverUrl = "/img/sound-of-perseverance.jpg", DurationSeconds = 404, BandId = 2 },
        new() { Id = 7, Title = "Ghost of Perdition", Genre = MetalGenre.ProgressiveMetal, ReleaseDate = new DateTime(2005, 8, 30), LyricsSnippet = "Ghost of mother, lingering death...", AudioSnippetUrl = "/audio/ghost-of-perdition.mp3", AlbumCoverUrl = "/img/ghost-reveries.jpg", DurationSeconds = 626, BandId = 3 },
        new() { Id = 8, Title = "Deliverance", Genre = MetalGenre.ProgressiveMetal, ReleaseDate = new DateTime(2002, 11, 4), LyricsSnippet = "Deliverance, thrown back at me...", AudioSnippetUrl = "/audio/deliverance.mp3", AlbumCoverUrl = "/img/deliverance.jpg", DurationSeconds = 816, BandId = 3 },
        new() { Id = 9, Title = "Bleak", Genre = MetalGenre.ProgressiveMetal, ReleaseDate = new DateTime(2001, 3, 6), LyricsSnippet = "Devious movements in your eyes...", AudioSnippetUrl = "/audio/bleak.mp3", AlbumCoverUrl = "/img/blackwater-park.jpg", DurationSeconds = 554, BandId = 3 }
    };

    foreach (var song in songs)
    {
        song.Band = bands.Single(b => b.Id == song.BandId);
        song.Band.Songs.Add(song);
    }

    var quizzes = new List<Quiz>
    {
        new() { Id = 1, Name = "Nordic Darkness", Description = "Guess iconic black metal tracks.", CreatedAt = DateTime.UtcNow, IsRanked = true, MaxScore = 300, TimeLimitSeconds = 180 },
        new() { Id = 2, Name = "Old School Death", Description = "Classic death metal challenge.", CreatedAt = DateTime.UtcNow, IsRanked = true, MaxScore = 300, TimeLimitSeconds = 180 },
        new() { Id = 3, Name = "Progressive Extreme", Description = "Long, complex, progressive extreme metal songs.", CreatedAt = DateTime.UtcNow, IsRanked = false, MaxScore = 300, TimeLimitSeconds = 240 }
    };

    quizzes[0].Rounds.Add(CreateRound(1, quizzes[0], songs[0]));
    quizzes[0].Rounds.Add(CreateRound(2, quizzes[0], songs[1]));
    quizzes[0].Rounds.Add(CreateRound(3, quizzes[0], songs[2]));

    quizzes[1].Rounds.Add(CreateRound(4, quizzes[1], songs[3]));
    quizzes[1].Rounds.Add(CreateRound(5, quizzes[1], songs[4]));
    quizzes[1].Rounds.Add(CreateRound(6, quizzes[1], songs[5]));

    quizzes[2].Rounds.Add(CreateRound(7, quizzes[2], songs[6]));
    quizzes[2].Rounds.Add(CreateRound(8, quizzes[2], songs[7]));
    quizzes[2].Rounds.Add(CreateRound(9, quizzes[2], songs[8]));

    var playlists = new List<UserPlaylist>
    {
        new() { Id = 1, Name = "Warmup Brutality", OwnerUserName = "noah", Description = "Short warmup playlist for quiz training.", CreatedAt = DateTime.UtcNow, IsPublic = true, Likes = 12 },
        new() { Id = 2, Name = "Technical Marathon", OwnerUserName = "noah", Description = "Long songs for expert listeners.", CreatedAt = DateTime.UtcNow, IsPublic = false, Likes = 5 }
    };

    LinkPlaylistSong(playlists[0], songs[1]);
    LinkPlaylistSong(playlists[0], songs[4]);
    LinkPlaylistSong(playlists[0], songs[6]);
    LinkPlaylistSong(playlists[1], songs[6]);
    LinkPlaylistSong(playlists[1], songs[7]);
    LinkPlaylistSong(playlists[1], songs[8]);

    var blackMetalAfter1990 = songs
        .Where(s => s.Genre == MetalGenre.BlackMetal && s.ReleaseDate.Year > 1990)
        .OrderBy(s => s.ReleaseDate)
        .ToList();

    var topBandsBySongCount = bands
        .OrderByDescending(b => b.Songs.Count)
        .Select(b => new { b.Name, SongCount = b.Songs.Count })
        .ToList();

    var publicPlaylistsWithDeathMetal = playlists
        .Where(p => p.IsPublic && p.PlaylistSongs.Any(ps => ps.Song?.Genre == MetalGenre.DeathMetal))
        .OrderByDescending(p => p.Likes)
        .ToList();

    var loadedTrack = await LoadTrackAsync(songs, 5);

    Console.WriteLine($"Lab 1 demo loaded. Quizzes: {quizzes.Count}");
    Console.WriteLine($"Black metal songs after 1990: {blackMetalAfter1990.Count}");
    Console.WriteLine($"Top band by catalog size: {topBandsBySongCount.First().Name}");
    Console.WriteLine($"Public playlists containing death metal: {publicPlaylistsWithDeathMetal.Count}");
    Console.WriteLine($"Async loaded track: {loadedTrack?.Title}");
}

static QuizRound CreateRound(int roundId, Quiz quiz, Song song)
{
    var round = new QuizRound
    {
        Id = roundId,
        QuizId = quiz.Id,
        Quiz = quiz,
        SongId = song.Id,
        Song = song,
        RoundNumber = quiz.Rounds.Count + 1,
        PointsForCorrectGuess = 100,
        MaxHintsAvailable = 5,
        Hints = new List<Hint>
        {
            new() { Id = roundId * 10 + 1, QuizRoundId = roundId, Type = HintType.Genre, Order = 1, Content = song.Genre.ToString() },
            new() { Id = roundId * 10 + 2, QuizRoundId = roundId, Type = HintType.Lyrics, Order = 2, Content = song.LyricsSnippet },
            new() { Id = roundId * 10 + 3, QuizRoundId = roundId, Type = HintType.AudioSnippet, Order = 3, Content = song.AudioSnippetUrl },
            new() { Id = roundId * 10 + 4, QuizRoundId = roundId, Type = HintType.AlbumCover, Order = 4, Content = song.AlbumCoverUrl },
            new() { Id = roundId * 10 + 5, QuizRoundId = roundId, Type = HintType.BandName, Order = 5, Content = song.Band?.Name ?? string.Empty }
        }
    };

    song.QuizRounds.Add(round);
    return round;
}

static void LinkPlaylistSong(UserPlaylist playlist, Song song)
{
    var relation = new PlaylistSong
    {
        PlaylistId = playlist.Id,
        Playlist = playlist,
        SongId = song.Id,
        Song = song,
        AddedAt = DateTime.UtcNow
    };

    playlist.PlaylistSongs.Add(relation);
    song.PlaylistSongs.Add(relation);
}

static async Task<Song?> LoadTrackAsync(IEnumerable<Song> songs, int songId)
{
    await Task.Delay(200);
    return songs.FirstOrDefault(s => s.Id == songId);
}
>>>>>>> ed8937926970a6d087a5c69af7505e0bfa96e32a
