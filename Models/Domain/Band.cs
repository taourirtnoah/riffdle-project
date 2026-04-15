namespace Riffdle.Models.Domain;

public class Band
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FormedYear { get; set; }
    public string Country { get; set; } = string.Empty;
    public Genre Genre { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public List<Album> Albums { get; set; } = new();
}
