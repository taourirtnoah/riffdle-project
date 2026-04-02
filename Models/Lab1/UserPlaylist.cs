<<<<<<< HEAD
namespace Riffdle.Models.Lab1;

public class UserPlaylist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerUserName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; }
    public int Likes { get; set; }
    public List<PlaylistSong> PlaylistSongs { get; set; } = new();
}
=======
namespace Riffdle.Models.Lab1;

public class UserPlaylist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerUserName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; }
    public int Likes { get; set; }
    public List<PlaylistSong> PlaylistSongs { get; set; } = new();
}
>>>>>>> ed8937926970a6d087a5c69af7505e0bfa96e32a
