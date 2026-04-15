using Riffdle.Models.Domain;

namespace Riffdle.Models.ViewModels;

public class GenreDetailsViewModel
{
    public Genre? Genre { get; set; }
    public List<Band> Bands { get; set; } = new();
}
