namespace Riffdle.Models.Lab1;

public class Band
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int FormedYear { get; set; }
    public bool IsActive { get; set; }
    public string PrimaryStyle { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<Song> Songs { get; set; } = new();
}
