namespace Riffdle.Models.Lab1;

public class PlaylistSong
{
    public int PlaylistId { get; set; }
    public UserPlaylist? Playlist { get; set; }
    public int SongId { get; set; }
    public Song? Song { get; set; }
    public DateTime AddedAt { get; set; }
}
