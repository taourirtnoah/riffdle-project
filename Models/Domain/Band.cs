using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riffdle.Models.Domain;

public class Band
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FormedYear { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [ForeignKey(nameof(Genre))]
    public int GenreId { get; set; }

    public virtual Genre? Genre { get; set; }
    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}
