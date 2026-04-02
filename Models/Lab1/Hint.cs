<<<<<<< HEAD
namespace Riffdle.Models.Lab1;

public class Hint
{
    public int Id { get; set; }
    public int QuizRoundId { get; set; }
    public QuizRound? QuizRound { get; set; }
    public HintType Type { get; set; }
    public int Order { get; set; }
    public string Content { get; set; } = string.Empty;
}
=======
namespace Riffdle.Models.Lab1;

public class Hint
{
    public int Id { get; set; }
    public int QuizRoundId { get; set; }
    public QuizRound? QuizRound { get; set; }
    public HintType Type { get; set; }
    public int Order { get; set; }
    public string Content { get; set; } = string.Empty;
}
>>>>>>> ed8937926970a6d087a5c69af7505e0bfa96e32a
