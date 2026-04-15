using Riffdle.Models.Domain;

namespace Riffdle.Models.Mock;

public class GenreMockRepository
{
    private readonly List<Genre> _genres;

    public GenreMockRepository()
    {
        _genres =
        [
            new Genre { Id = 1, Name = "Thrash Metal", Description = "Fast and aggressive riffs built on precision rhythm guitar." },
            new Genre { Id = 2, Name = "Heavy Metal", Description = "Classic high-energy metal with anthem choruses and twin-guitar leads." },
            new Genre { Id = 3, Name = "Progressive Metal", Description = "Technical arrangements, shifting time signatures, and layered atmospheres." },
            new Genre { Id = 4, Name = "Death Metal", Description = "Low-tuned intensity, blast beats, and crushing rhythmic structures." },
            new Genre { Id = 5, Name = "Symphonic Metal", Description = "Orchestral textures and cinematic vocal arrangements merged with heavy riffs." }
        ];
    }

    public List<Genre> GetAll()
    {
        return _genres;
    }

    public Genre? GetById(int id)
    {
        return _genres.FirstOrDefault(genre => genre.Id == id);
    }
}
