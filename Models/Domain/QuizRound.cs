namespace Riffdle.Models.Domain;

public class QuizRound
{
    public int Id { get; set; }
    public int SongId { get; set; }
    public Song? Song { get; set; }
    public List<Hint> Hints { get; set; } = new();
}
