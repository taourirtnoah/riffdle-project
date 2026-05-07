using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;

namespace Riffdle.Models.Mock;

public class BandMockRepository
{
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public BandMockRepository(IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public List<Band> GetAll()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Bands
            .AsNoTracking()
            .Include(band => band.Genre)
            .Include(band => band.Albums)
            .OrderBy(band => band.Name)
            .ToList();
    }

    public Band? GetById(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Bands
            .AsNoTracking()
            .Include(band => band.Genre)
            .Include(band => band.Albums.OrderBy(album => album.ReleaseYear))
            .FirstOrDefault(band => band.Id == id);
    }
}
