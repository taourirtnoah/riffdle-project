<<<<<<< HEAD
namespace Riffdle.Models.Lab1;

public class QuizRound
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }
    public int SongId { get; set; }
    public Song? Song { get; set; }
    public int RoundNumber { get; set; }
    public int PointsForCorrectGuess { get; set; }
    public int MaxHintsAvailable { get; set; }
    public List<Hint> Hints { get; set; } = new();
}
=======
namespace Riffdle.Models.Lab1;

public class QuizRound
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }
    public int SongId { get; set; }
    public Song? Song { get; set; }
    public int RoundNumber { get; set; }
    public int PointsForCorrectGuess { get; set; }
    public int MaxHintsAvailable { get; set; }
    public List<Hint> Hints { get; set; } = new();
}
>>>>>>> ed8937926970a6d087a5c69af7505e0bfa96e32a
