using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riffdle.Models.Domain;

public class Album
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }

    [ForeignKey(nameof(Band))]
    public int BandId { get; set; }

    public virtual Band? Band { get; set; }
    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
