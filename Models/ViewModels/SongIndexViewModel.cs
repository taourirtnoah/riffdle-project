using Riffdle.Models.Domain;

namespace Riffdle.Models.ViewModels;

public class SongIndexViewModel
{
    public List<Song> Songs { get; set; } = new();
}
