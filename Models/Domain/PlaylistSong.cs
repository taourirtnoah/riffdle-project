using System.ComponentModel.DataAnnotations.Schema;

namespace Riffdle.Models.Domain;

public class PlaylistSong
{

    [ForeignKey(nameof(Playlist))]
    public int PlaylistId { get; set; }

    public virtual UserPlaylist? Playlist { get; set; }

    [ForeignKey(nameof(Song))]
    public int SongId { get; set; }

    public virtual Song? Song { get; set; }
    public DateTime AddedAt { get; set; }
}
