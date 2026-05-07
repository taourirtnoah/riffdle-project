using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.Domain;

public class UserPlaylist
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerUserName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; }
    public int Likes { get; set; }

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}
