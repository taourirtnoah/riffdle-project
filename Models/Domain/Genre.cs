using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.Domain;

public class Genre
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<Band> Bands { get; set; } = new List<Band>();
}
