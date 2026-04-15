using Riffdle.Models.Domain;

namespace Riffdle.Models.Mock;

public class BandMockRepository
{
    private readonly List<Band> _bands;

    public BandMockRepository(GenreMockRepository genreRepository)
    {
        var genres = genreRepository.GetAll();

        _bands =
        [
            new Band
            {
                Id = 1,
                Name = "Metallica",
                FormedYear = 1981,
                Country = "United States",
                Genre = genres.First(genre => genre.Id == 1),
                Description = "Thrash pioneers known for sharp riff architecture and arena-scale songwriting."
            },
            new Band
            {
                Id = 2,
                Name = "Iron Maiden",
                FormedYear = 1975,
                Country = "United Kingdom",
                Genre = genres.First(genre => genre.Id == 2),
                Description = "New Wave of British Heavy Metal legends with narrative epics and galloping rhythm sections."
            },
            new Band
            {
                Id = 3,
                Name = "Opeth",
                FormedYear = 1990,
                Country = "Sweden",
                Genre = genres.First(genre => genre.Id == 3),
                Description = "Progressive storytellers blending acoustic melancholy with extreme metal weight."
            },
            new Band
            {
                Id = 4,
                Name = "Gojira",
                FormedYear = 1996,
                Country = "France",
                Genre = genres.First(genre => genre.Id == 4),
                Description = "Rhythm-driven modern heavyweights focused on groove, dynamics, and environmental themes."
            },
            new Band
            {
                Id = 5,
                Name = "Judas Priest",
                FormedYear = 1969,
                Country = "United Kingdom",
                Genre = genres.First(genre => genre.Id == 2),
                Description = "Foundational heavy metal icons with twin-lead guitar attacks and steel-plated hooks."
            },
            new Band
            {
                Id = 6,
                Name = "Nightwish",
                FormedYear = 1996,
                Country = "Finland",
                Genre = genres.First(genre => genre.Id == 5),
                Description = "Symphonic metal trailblazers known for cinematic arrangements and dramatic vocal melodies."
            }
        ];
    }

    public List<Band> GetAll()
    {
        return _bands;
    }

    public Band? GetById(int id)
    {
        return _bands.FirstOrDefault(band => band.Id == id);
    }
}
