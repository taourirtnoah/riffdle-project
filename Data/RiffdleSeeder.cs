using System.Collections.Generic;
using System.Linq;
using Riffdle.Models.Domain;

namespace Riffdle.Data;

public static class RiffdleSeeder
{
    public static void Seed(RiffdleDbContext context)
    {
        var genres = EnsureGenres(context);
        var bands = EnsureBands(context, genres);
        var albums = EnsureAlbums(context, bands);
        var songs = EnsureSongs(context, albums);
        EnsurePlaylists(context, songs);
    }

    private static List<Genre> EnsureGenres(RiffdleDbContext context)
    {
        var genres = context.Genres.ToList();
        if (genres.Count > 0)
        {
            return genres;
        }

        genres = new List<Genre>
        {
            new() { Name = "Thrash Metal", Description = "Fast and aggressive riffs built on precision rhythm guitar." },
            new() { Name = "Heavy Metal", Description = "Classic high-energy metal with anthem choruses and twin-guitar leads." },
            new() { Name = "Progressive Metal", Description = "Technical arrangements, shifting time signatures, and layered atmospheres." },
            new() { Name = "Death Metal", Description = "Low-tuned intensity, blast beats, and crushing rhythmic structures." },
            new() { Name = "Symphonic Metal", Description = "Orchestral textures and cinematic vocal arrangements merged with heavy riffs." }
        };

        context.AddRange(genres);
        context.SaveChanges();
        return genres;
    }

    private static List<Band> EnsureBands(RiffdleDbContext context, List<Genre> genres)
    {
        var bands = context.Bands.ToList();
        if (bands.Count > 0)
        {
            return bands;
        }

        bands = new List<Band>
        {
            new() { Name = "Metallica", FormedYear = 1981, Country = "United States", Genre = genres.First(g => g.Name == "Thrash Metal"), Description = "Thrash pioneers known for sharp riff architecture and arena-scale songwriting." },
            new() { Name = "Iron Maiden", FormedYear = 1975, Country = "United Kingdom", Genre = genres.First(g => g.Name == "Heavy Metal"), Description = "New Wave of British Heavy Metal legends with narrative epics and galloping rhythm sections." },
            new() { Name = "Opeth", FormedYear = 1990, Country = "Sweden", Genre = genres.First(g => g.Name == "Progressive Metal"), Description = "Progressive storytellers blending acoustic melancholy with extreme metal weight." },
            new() { Name = "Gojira", FormedYear = 1996, Country = "France", Genre = genres.First(g => g.Name == "Death Metal"), Description = "Rhythm-driven modern heavyweights focused on groove, dynamics, and environmental themes." },
            new() { Name = "Judas Priest", FormedYear = 1969, Country = "United Kingdom", Genre = genres.First(g => g.Name == "Heavy Metal"), Description = "Foundational heavy metal icons with twin-lead guitar attacks and steel-plated hooks." },
            new() { Name = "Nightwish", FormedYear = 1996, Country = "Finland", Genre = genres.First(g => g.Name == "Symphonic Metal"), Description = "Symphonic metal trailblazers known for cinematic arrangements and dramatic vocal melodies." }
        };

        context.AddRange(bands);
        context.SaveChanges();
        return bands;
    }

    private static List<Album> EnsureAlbums(RiffdleDbContext context, List<Band> bands)
    {
        var albums = context.Albums.ToList();
        if (albums.Count > 0)
        {
            return albums;
        }

        albums = new List<Album>
        {
            new() { Title = "Ride the Lightning", ReleaseYear = 1984, Band = bands[0] },
            new() { Title = "Master of Puppets", ReleaseYear = 1986, Band = bands[0] },
            new() { Title = "...And Justice for All", ReleaseYear = 1988, Band = bands[0] },
            new() { Title = "The Number of the Beast", ReleaseYear = 1982, Band = bands[1] },
            new() { Title = "Powerslave", ReleaseYear = 1984, Band = bands[1] },
            new() { Title = "Seventh Son of a Seventh Son", ReleaseYear = 1988, Band = bands[1] },
            new() { Title = "Blackwater Park", ReleaseYear = 2001, Band = bands[2] },
            new() { Title = "Ghost Reveries", ReleaseYear = 2005, Band = bands[2] },
            new() { Title = "Watershed", ReleaseYear = 2008, Band = bands[2] },
            new() { Title = "From Mars to Sirius", ReleaseYear = 2005, Band = bands[3] },
            new() { Title = "The Way of All Flesh", ReleaseYear = 2008, Band = bands[3] },
            new() { Title = "Magma", ReleaseYear = 2016, Band = bands[3] },
            new() { Title = "British Steel", ReleaseYear = 1980, Band = bands[4] },
            new() { Title = "Screaming for Vengeance", ReleaseYear = 1982, Band = bands[4] },
            new() { Title = "Painkiller", ReleaseYear = 1990, Band = bands[4] },
            new() { Title = "Oceanborn", ReleaseYear = 1998, Band = bands[5] },
            new() { Title = "Once", ReleaseYear = 2004, Band = bands[5] },
            new() { Title = "Dark Passion Play", ReleaseYear = 2007, Band = bands[5] }
        };

        context.AddRange(albums);
        context.SaveChanges();
        return albums;
    }

    private static List<Song> EnsureSongs(RiffdleDbContext context, List<Album> albums)
    {
        var songs = context.Songs.ToList();
        if (songs.Count > 0)
        {
            return songs;
        }

        songs = new List<Song>();

        void AddAlbumSongs(int albumIndex, params (string title, int duration, string lyric)[] entries)
        {
            var album = albums[albumIndex - 1];
            foreach (var (title, duration, lyric) in entries)
            {
                songs.Add(new Song
                {
                    Title = title,
                    DurationSeconds = duration,
                    Album = album,
                    OpeningLyric = lyric,
                    IsDailyQuizSong = false
                });
            }
        }

        AddAlbumSongs(1,
            ("Fight Fire with Fire", 284, "Flashpoint heat tears through the dawn."),
            ("Ride the Lightning", 397, "Cold steel justice rises in the storm."),
            ("For Whom the Bell Tolls", 311, "A distant bell cuts through the dust."),
            ("Creeping Death", 396, "Desert winds carry a warning in blood."));

        AddAlbumSongs(2,
            ("Battery", 312, "A spark ignites and the room starts shaking."),
            ("Master of Puppets", 515, "Strings tighten while control slips away."),
            ("Welcome Home (Sanitarium)", 386, "Silent walls hum beneath pale lights."),
            ("Disposable Heroes", 496, "Boots pound forward into endless fire."));

        AddAlbumSongs(3,
            ("Blackened", 401, "The sky cracks open over concrete hearts."),
            ("One", 447, "A lonely pulse counts the passing hours."),
            ("Harvester of Sorrow", 345, "Low thunder follows every step."),
            ("Dyers Eve", 313, "Rage breaks free in a final rush."));

        AddAlbumSongs(4,
            ("Children of the Damned", 274, "Moonlight trembles on cathedral glass."),
            ("The Prisoner", 363, "Iron doors slam and a voice refuses."),
            ("Run to the Hills", 234, "Riders storm the ridge before sunrise."),
            ("Hallowed Be Thy Name", 431, "The final march begins in chains."));

        AddAlbumSongs(5,
            ("Aces High", 271, "Engines roar over a burning horizon."),
            ("2 Minutes to Midnight", 360, "Hands move closer on a restless clock."),
            ("Powerslave", 407, "Sand and stone guard a sleeping king."),
            ("Rime of the Ancient Mariner", 811, "Waves whisper warnings to the doomed."));

        AddAlbumSongs(6,
            ("Moonchild", 339, "Dark prophecy circles the night air."),
            ("Infinite Dreams", 373, "Sleepless eyes trace shifting worlds."),
            ("Can I Play with Madness", 211, "The mirror laughs at every question."),
            ("The Evil That Men Do", 274, "A sharpened truth cuts through silence."));

        AddAlbumSongs(7,
            ("The Leper Affinity", 623, "Shadows twist through the cedar rain."),
            ("Bleak", 557, "A cold horizon blurs into memory."),
            ("Harvest", 399, "Quiet fields hide a fading ache."),
            ("Blackwater Park", 739, "Ancient roots hold forgotten names."));

        AddAlbumSongs(8,
            ("Ghost of Perdition", 630, "A whisper returns from the hollow."),
            ("The Baying of the Hounds", 641, "Distant howls roll over dead ground."),
            ("Atonement", 352, "A slow tide pulls the weight away."),
            ("The Grand Conjuration", 641, "Cinders spin in ritual circles."));

        AddAlbumSongs(9,
            ("Coil", 220, "Morning light drips through silent rooms."),
            ("Heir Apparent", 531, "Heavy footsteps climb the marble stairs."),
            ("The Lotus Eater", 529, "Neon petals close around the pulse."),
            ("Burden", 440, "A final chord hangs over empty chairs."));

        AddAlbumSongs(10,
            ("Ocean Planet", 333, "Tides crash beneath iron clouds."),
            ("Backbone", 258, "The spine stiffens against the pressure."),
            ("Flying Whales", 443, "Colossal shapes move below the waves."),
            ("The Heaviest Matter of the Universe", 470, "Gravity bends around each strike."));

        AddAlbumSongs(11,
            ("Oroborus", 323, "The circle closes and begins again."),
            ("Toxic Garbage Island", 259, "Black smoke climbs over poisoned water."),
            ("Vacuity", 183, "An empty room echoes with static."),
            ("The Art of Dying", 594, "Breath slows before the break of light."));

        AddAlbumSongs(12,
            ("The Shooting Star", 342, "A bright line cuts across the dusk."),
            ("Silvera", 212, "Steel rhythms spark along the wire."),
            ("Stranded", 269, "No road remains beneath the dust."),
            ("Low Lands", 293, "Low drums call through distant fog."));

        AddAlbumSongs(13,
            ("Rapid Fire", 248, "Sirens split the midnight streets."),
            ("Metal Gods", 240, "Marching echoes shake the floor."),
            ("Breaking the Law", 155, "Neon signs flicker over restless minds."),
            ("Living After Midnight", 210, "City lights blur at full speed."));

        AddAlbumSongs(14,
            ("The Hellion", 41, "Signals surge before the first strike."),
            ("Electric Eye", 219, "A cold lens watches every move."),
            ("Riding on the Wind", 186, "Wheels burn bright through the night."),
            ("You've Got Another Thing Comin'", 308, "Confidence hits like a drumline."));

        AddAlbumSongs(15,
            ("Painkiller", 366, "Chrome wings tear through the sky."),
            ("Hell Patrol", 223, "Engines grind over shattered roads."),
            ("Night Crawler", 345, "Silent steps move just beyond the light."),
            ("A Touch of Evil", 334, "A hidden spark wakes old desire."));

        AddAlbumSongs(16,
            ("Stargazers", 268, "Stars bend over frozen lakes."),
            ("Gethsemane", 321, "Choirs rise under winter clouds."),
            ("The Pharaoh Sails to Orion", 382, "Golden sails cross a crimson sea."),
            ("Sleeping Sun", 240, "Pale dawn rests over silent snow."));

        AddAlbumSongs(17,
            ("Dark Chest of Wonders", 268, "An old gate opens to thunder."),
            ("Wish I Had an Angel", 242, "Bright sparks dance on cathedral stone."),
            ("Nemo", 276, "Lost maps drift over black water."),
            ("Ghost Love Score", 601, "An orchestra rises like a stormfront."));

        AddAlbumSongs(18,
            ("The Poet and the Pendulum", 839, "A final script burns on the stage."),
            ("Bye Bye Beautiful", 254, "Curtains fall beneath bitter lights."),
            ("Amaranth", 231, "A scarlet bloom breaks through the frost."),
            ("7 Days to the Wolves", 455, "Footsteps gather on the ridge."));

        context.AddRange(songs);
        context.SaveChanges();
        return songs;
    }

    private static void EnsurePlaylists(RiffdleDbContext context, List<Song> songs)
    {
        if (context.UserPlaylists.Any())
        {
            return;
        }

        var featuredPlaylist = new UserPlaylist
        {
            Name = "Starter Setlist",
            OwnerUserName = "riffdle",
            Description = "A short demo playlist used to showcase the new playlist page.",
            CreatedAt = DateTime.UtcNow,
            IsPublic = true,
            Likes = 12
        };

        context.UserPlaylists.Add(featuredPlaylist);
        context.SaveChanges();

        var selectedSongs = songs.Take(4).ToList();
        var playlistSongs = selectedSongs.Select(song => new PlaylistSong
        {
            Playlist = featuredPlaylist,
            Song = song,
            AddedAt = DateTime.UtcNow
        }).ToList();

        context.PlaylistSongs.AddRange(playlistSongs);
        context.SaveChanges();
    }
}