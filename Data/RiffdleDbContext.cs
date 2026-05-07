using Microsoft.EntityFrameworkCore;
using Riffdle.Models.Domain;

namespace Riffdle.Data;

public class RiffdleDbContext : DbContext
{
    public RiffdleDbContext(DbContextOptions<RiffdleDbContext> options)
        : base(options)
    {
    }

    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Band> Bands => Set<Band>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Hint> Hints => Set<Hint>();
    public DbSet<PlaylistSong> PlaylistSongs => Set<PlaylistSong>();
    public DbSet<QuizRound> QuizRounds => Set<QuizRound>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<UserPlaylist> UserPlaylists => Set<UserPlaylist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite primary key for the join table between playlists and songs
        modelBuilder.Entity<PlaylistSong>()
            .HasKey(ps => new { ps.PlaylistId, ps.SongId });
    }
}