using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riffdle.Models.Domain;

public class QuizRound
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Song))]
    public int SongId { get; set; }

    public virtual Song? Song { get; set; }
    public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();
}
