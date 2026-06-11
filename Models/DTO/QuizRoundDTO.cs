using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class QuizRoundDTO
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int SongId { get; set; }

    public string SongTitle { get; set; } = string.Empty;

    public List<HintDTO> Hints { get; set; } = new();
}
