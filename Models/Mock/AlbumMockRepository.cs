using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;

namespace Riffdle.Models.Mock;

public class AlbumMockRepository
{
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public AlbumMockRepository(IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public List<Album> GetAll()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Albums
            .AsNoTracking()
            .Include(album => album.Band)
            .ThenInclude(band => band!.Genre)
            .Include(album => album.Songs)
            .OrderBy(album => album.ReleaseYear)
            .ToList();
    }

    public Album? GetById(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Albums
            .AsNoTracking()
            .Include(album => album.Band)
            .ThenInclude(band => band!.Genre)
            .Include(album => album.Songs)
            .FirstOrDefault(album => album.Id == id);
    }
}
