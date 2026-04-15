namespace Riffdle.Models.Domain;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public Band Band { get; set; } = new();
    public List<Song> Songs { get; set; } = new();
}
