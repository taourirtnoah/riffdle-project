using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;

namespace Riffdle.Models.Mock;

public class GenreMockRepository
{
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public GenreMockRepository(IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public List<Genre> GetAll()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Genres
            .AsNoTracking()
            .Include(genre => genre.Bands)
            .OrderBy(genre => genre.Name)
            .ToList();
    }

    public Genre? GetById(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Genres
            .AsNoTracking()
            .Include(genre => genre.Bands)
            .FirstOrDefault(genre => genre.Id == id);
    }
}
