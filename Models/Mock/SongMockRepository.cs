using Riffdle.Models.Domain;

namespace Riffdle.Models.Mock;

public class SongMockRepository
{
    private readonly List<Song> _songs;

    public SongMockRepository(AlbumMockRepository albumRepository)
    {
        var albums = albumRepository.GetAll();
        var albumsByTitle = albums.ToDictionary(album => album.Title, album => album);

        _songs = [];
        var songId = 1;

        AddAlbumSongs("Ride the Lightning",
            ("Fight Fire with Fire", 284, "Flashpoint heat tears through the dawn."),
            ("Ride the Lightning", 397, "Cold steel justice rises in the storm."),
            ("For Whom the Bell Tolls", 311, "A distant bell cuts through the dust."),
            ("Creeping Death", 396, "Desert winds carry a warning in blood."));

        AddAlbumSongs("Master of Puppets",
            ("Battery", 312, "A spark ignites and the room starts shaking."),
            ("Master of Puppets", 515, "Strings tighten while control slips away."),
            ("Welcome Home (Sanitarium)", 386, "Silent walls hum beneath pale lights."),
            ("Disposable Heroes", 496, "Boots pound forward into endless fire."));

        AddAlbumSongs("...And Justice for All",
            ("Blackened", 401, "The sky cracks open over concrete hearts."),
            ("One", 447, "A lonely pulse counts the passing hours."),
            ("Harvester of Sorrow", 345, "Low thunder follows every step."),
            ("Dyers Eve", 313, "Rage breaks free in a final rush."));

        AddAlbumSongs("The Number of the Beast",
            ("Children of the Damned", 274, "Moonlight trembles on cathedral glass."),
            ("The Prisoner", 363, "Iron doors slam and a voice refuses."),
            ("Run to the Hills", 234, "Riders storm the ridge before sunrise."),
            ("Hallowed Be Thy Name", 431, "The final march begins in chains."));

        AddAlbumSongs("Powerslave",
            ("Aces High", 271, "Engines roar over a burning horizon."),
            ("2 Minutes to Midnight", 360, "Hands move closer on a restless clock."),
            ("Powerslave", 407, "Sand and stone guard a sleeping king."),
            ("Rime of the Ancient Mariner", 811, "Waves whisper warnings to the doomed."));

        AddAlbumSongs("Seventh Son of a Seventh Son",
            ("Moonchild", 339, "Dark prophecy circles the night air."),
            ("Infinite Dreams", 373, "Sleepless eyes trace shifting worlds."),
            ("Can I Play with Madness", 211, "The mirror laughs at every question."),
            ("The Evil That Men Do", 274, "A sharpened truth cuts through silence."));

        AddAlbumSongs("Blackwater Park",
            ("The Leper Affinity", 623, "Shadows twist through the cedar rain."),
            ("Bleak", 557, "A cold horizon blurs into memory."),
            ("Harvest", 399, "Quiet fields hide a fading ache."),
            ("Blackwater Park", 739, "Ancient roots hold forgotten names."));

        AddAlbumSongs("Ghost Reveries",
            ("Ghost of Perdition", 630, "A whisper returns from the hollow."),
            ("The Baying of the Hounds", 641, "Distant howls roll over dead ground."),
            ("Atonement", 352, "A slow tide pulls the weight away."),
            ("The Grand Conjuration", 641, "Cinders spin in ritual circles."));

        AddAlbumSongs("Watershed",
            ("Coil", 220, "Morning light drips through silent rooms."),
            ("Heir Apparent", 531, "Heavy footsteps climb the marble stairs."),
            ("The Lotus Eater", 529, "Neon petals close around the pulse."),
            ("Burden", 440, "A final chord hangs over empty chairs."));

        AddAlbumSongs("From Mars to Sirius",
            ("Ocean Planet", 333, "Tides crash beneath iron clouds."),
            ("Backbone", 258, "The spine stiffens against the pressure."),
            ("Flying Whales", 443, "Colossal shapes move below the waves."),
            ("The Heaviest Matter of the Universe", 470, "Gravity bends around each strike."));

        AddAlbumSongs("The Way of All Flesh",
            ("Oroborus", 323, "The circle closes and begins again."),
            ("Toxic Garbage Island", 259, "Black smoke climbs over poisoned water."),
            ("Vacuity", 183, "An empty room echoes with static."),
            ("The Art of Dying", 594, "Breath slows before the break of light."));

        AddAlbumSongs("Magma",
            ("The Shooting Star", 342, "A bright line cuts across the dusk."),
            ("Silvera", 212, "Steel rhythms spark along the wire."),
            ("Stranded", 269, "No road remains beneath the dust."),
            ("Low Lands", 293, "Low drums call through distant fog."));

        AddAlbumSongs("British Steel",
            ("Rapid Fire", 248, "Sirens split the midnight streets."),
            ("Metal Gods", 240, "Marching echoes shake the floor."),
            ("Breaking the Law", 155, "Neon signs flicker over restless minds."),
            ("Living After Midnight", 210, "City lights blur at full speed."));

        AddAlbumSongs("Screaming for Vengeance",
            ("The Hellion", 41, "Signals surge before the first strike."),
            ("Electric Eye", 219, "A cold lens watches every move."),
            ("Riding on the Wind", 186, "Wheels burn bright through the night."),
            ("You've Got Another Thing Comin'", 308, "Confidence hits like a drumline."));

        AddAlbumSongs("Painkiller",
            ("Painkiller", 366, "Chrome wings tear through the sky."),
            ("Hell Patrol", 223, "Engines grind over shattered roads."),
            ("Night Crawler", 345, "Silent steps move just beyond the light."),
            ("A Touch of Evil", 334, "A hidden spark wakes old desire."));

        AddAlbumSongs("Oceanborn",
            ("Stargazers", 268, "Stars bend over frozen lakes."),
            ("Gethsemane", 321, "Choirs rise under winter clouds."),
            ("The Pharaoh Sails to Orion", 382, "Golden sails cross a crimson sea."),
            ("Sleeping Sun", 240, "Pale dawn rests over silent snow."));

        AddAlbumSongs("Once",
            ("Dark Chest of Wonders", 268, "An old gate opens to thunder."),
            ("Wish I Had an Angel", 242, "Bright sparks dance on cathedral stone."),
            ("Nemo", 276, "Lost maps drift over black water."),
            ("Ghost Love Score", 601, "An orchestra rises like a stormfront."));

        AddAlbumSongs("Dark Passion Play",
            ("The Poet and the Pendulum", 839, "A final script burns on the stage."),
            ("Bye Bye Beautiful", 254, "Curtains fall beneath bitter lights."),
            ("Amaranth", 231, "A scarlet bloom breaks through the frost."),
            ("7 Days to the Wolves", 455, "Footsteps gather on the ridge."));

        foreach (var album in albums)
        {
            album.Songs = _songs
                .Where(song => song.Album.Id == album.Id)
                .OrderBy(song => song.Id)
                .ToList();
        }

        return;

        void AddAlbumSongs(string albumTitle, params (string title, int duration, string lyric)[] entries)
        {
            if (!albumsByTitle.TryGetValue(albumTitle, out var album))
            {
                return;
            }

            foreach (var (title, duration, lyric) in entries)
            {
                _songs.Add(new Song
                {
                    Id = songId++,
                    Title = title,
                    DurationSeconds = duration,
                    Album = album,
                    OpeningLyric = lyric,
                    IsDailyQuizSong = albumTitle == "Master of Puppets" &&
                                      title == "Master of Puppets"
                });
            }
        }
    }

    public List<Song> GetAll()
    {
        return _songs;
    }

    public Song? GetById(int id)
    {
        return _songs.FirstOrDefault(song => song.Id == id);
    }

    public Song? GetDailyQuizSong()
    {
        return _songs.FirstOrDefault(song => song.IsDailyQuizSong);
    }
}
