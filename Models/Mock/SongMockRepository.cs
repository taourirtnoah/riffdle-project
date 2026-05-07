using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using System.Security.Cryptography;

namespace Riffdle.Models.Mock;

public class SongMockRepository
{
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;
    private List<Song>? _songs;
    private Song? _dailyQuizSong;

    public SongMockRepository(IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public List<Song> GetAll()
    {
        return LoadSongs();
    }

    public Song? GetById(int id)
    {
        return LoadSongs().FirstOrDefault(song => song.Id == id);
    }

    public Song? GetDailyQuizSong()
    {
        if (_dailyQuizSong is not null)
        {
            return _dailyQuizSong;
        }

        var songs = LoadSongs();
        if (songs.Count == 0)
        {
            return null;
        }

        _dailyQuizSong = songs[RandomNumberGenerator.GetInt32(songs.Count)];
        _dailyQuizSong.IsDailyQuizSong = true;
        return _dailyQuizSong;
    }

    public Song? ResetDailyQuizSong()
    {
        var songs = LoadSongs();
        if (songs.Count == 0)
        {
            _dailyQuizSong = null;
            return null;
        }

        if (_dailyQuizSong is not null)
        {
            _dailyQuizSong.IsDailyQuizSong = false;
        }

        var candidates = songs
            .Where(song => _dailyQuizSong is null || song.Id != _dailyQuizSong.Id)
            .ToList();

        if (candidates.Count == 0)
        {
            candidates = songs;
        }

        _dailyQuizSong = candidates[RandomNumberGenerator.GetInt32(candidates.Count)];
        _dailyQuizSong.IsDailyQuizSong = true;
        return _dailyQuizSong;
    }

    private List<Song> LoadSongs()
    {
        if (_songs is not null)
        {
            return _songs;
        }

        using var context = _dbContextFactory.CreateDbContext();
        _songs = context.Songs
            .AsNoTracking()
            .Include(song => song.Album)
            .ThenInclude(album => album!.Band)
            .ThenInclude(band => band!.Genre)
            .OrderBy(song => song.Id)
            .ToList();

        return _songs;
    }
}
