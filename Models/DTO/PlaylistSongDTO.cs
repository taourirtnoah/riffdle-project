using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class PlaylistSongDTO
{
    [Range(1, int.MaxValue)]
    public int PlaylistId { get; set; }

    public string PlaylistName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SongId { get; set; }

    public string SongTitle { get; set; } = string.Empty;

    public DateTime AddedAt { get; set; }
}
