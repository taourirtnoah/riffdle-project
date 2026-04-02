<<<<<<< HEAD
namespace Riffdle.Models.Lab1;

public class Quiz
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRanked { get; set; }
    public int MaxScore { get; set; }
    public int TimeLimitSeconds { get; set; }
    public List<QuizRound> Rounds { get; set; } = new();
}
=======
namespace Riffdle.Models.Lab1;

public class Quiz
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRanked { get; set; }
    public int MaxScore { get; set; }
    public int TimeLimitSeconds { get; set; }
    public List<QuizRound> Rounds { get; set; } = new();
}
>>>>>>> ed8937926970a6d087a5c69af7505e0bfa96e32a
