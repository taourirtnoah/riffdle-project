using Riffdle.Models.Domain;

namespace Riffdle.Models.Mock;

public class AlbumMockRepository
{
    private readonly List<Album> _albums;

    public AlbumMockRepository(BandMockRepository bandRepository)
    {
        var bands = bandRepository.GetAll();

        _albums =
        [
            new Album { Id = 1, Title = "Ride the Lightning", ReleaseYear = 1984, Band = bands.First(band => band.Id == 1) },
            new Album { Id = 2, Title = "Master of Puppets", ReleaseYear = 1986, Band = bands.First(band => band.Id == 1) },
            new Album { Id = 3, Title = "...And Justice for All", ReleaseYear = 1988, Band = bands.First(band => band.Id == 1) },

            new Album { Id = 4, Title = "The Number of the Beast", ReleaseYear = 1982, Band = bands.First(band => band.Id == 2) },
            new Album { Id = 5, Title = "Powerslave", ReleaseYear = 1984, Band = bands.First(band => band.Id == 2) },
            new Album { Id = 6, Title = "Seventh Son of a Seventh Son", ReleaseYear = 1988, Band = bands.First(band => band.Id == 2) },

            new Album { Id = 7, Title = "Blackwater Park", ReleaseYear = 2001, Band = bands.First(band => band.Id == 3) },
            new Album { Id = 8, Title = "Ghost Reveries", ReleaseYear = 2005, Band = bands.First(band => band.Id == 3) },
            new Album { Id = 9, Title = "Watershed", ReleaseYear = 2008, Band = bands.First(band => band.Id == 3) },

            new Album { Id = 10, Title = "From Mars to Sirius", ReleaseYear = 2005, Band = bands.First(band => band.Id == 4) },
            new Album { Id = 11, Title = "The Way of All Flesh", ReleaseYear = 2008, Band = bands.First(band => band.Id == 4) },
            new Album { Id = 12, Title = "Magma", ReleaseYear = 2016, Band = bands.First(band => band.Id == 4) },

            new Album { Id = 13, Title = "British Steel", ReleaseYear = 1980, Band = bands.First(band => band.Id == 5) },
            new Album { Id = 14, Title = "Screaming for Vengeance", ReleaseYear = 1982, Band = bands.First(band => band.Id == 5) },
            new Album { Id = 15, Title = "Painkiller", ReleaseYear = 1990, Band = bands.First(band => band.Id == 5) },

            new Album { Id = 16, Title = "Oceanborn", ReleaseYear = 1998, Band = bands.First(band => band.Id == 6) },
            new Album { Id = 17, Title = "Once", ReleaseYear = 2004, Band = bands.First(band => band.Id == 6) },
            new Album { Id = 18, Title = "Dark Passion Play", ReleaseYear = 2007, Band = bands.First(band => band.Id == 6) }
        ];

        foreach (var band in bands)
        {
            band.Albums = _albums
                .Where(album => album.Band.Id == band.Id)
                .OrderBy(album => album.ReleaseYear)
                .ToList();
        }
    }

    public List<Album> GetAll()
    {
        return _albums;
    }

    public Album? GetById(int id)
    {
        return _albums.FirstOrDefault(album => album.Id == id);
    }
}
