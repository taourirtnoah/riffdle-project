using System.ComponentModel.DataAnnotations;
using Riffdle.Models.Domain;

namespace Riffdle.Models.DTO;

public class HintDTO
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int QuizRoundId { get; set; }

    public HintType Type { get; set; }

    [Range(0, int.MaxValue)]
    public int Order { get; set; }

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}
