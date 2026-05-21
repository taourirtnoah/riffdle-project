using Riffdle.Models.Domain;

namespace Riffdle.Models.ViewModels;

public class PlaylistIndexViewModel
{
    public List<UserPlaylist> Playlists { get; set; } = new();

    public HashSet<int> LikedPlaylistIds { get; set; } = new();
}