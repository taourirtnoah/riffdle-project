using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Riffdle.Data;
using Riffdle.Models.Domain;

#nullable disable

namespace Riffdle.Data.Migrations
{
    [DbContext(typeof(RiffdleDbContext))]
    partial class RiffdleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Genres");
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Description).IsRequired();
            });

            modelBuilder.Entity<UserPlaylist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("UserPlaylists");
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.OwnerUserName).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsPublic).IsRequired();
                entity.Property(e => e.Likes).IsRequired();
            });

            modelBuilder.Entity<Band>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Bands");
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Country).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.HasOne(e => e.Genre)
                    .WithMany(e => e.Bands)
                    .HasForeignKey(e => e.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Albums");
                entity.Property(e => e.Title).IsRequired();
                entity.HasOne(e => e.Band)
                    .WithMany(e => e.Albums)
                    .HasForeignKey(e => e.BandId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Songs");
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.OpeningLyric).IsRequired();
                entity.Property(e => e.AudioSnippetUrl).IsRequired();
                entity.Property(e => e.AlbumCoverUrl).IsRequired();
                entity.HasOne(e => e.Album)
                    .WithMany(e => e.Songs)
                    .HasForeignKey(e => e.AlbumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PlaylistSong>(entity =>
            {
                entity.HasKey(e => new { e.PlaylistId, e.SongId });
                entity.ToTable("PlaylistSongs");
                entity.HasOne(e => e.Playlist)
                    .WithMany(e => e.PlaylistSongs)
                    .HasForeignKey(e => e.PlaylistId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Song)
                    .WithMany(e => e.PlaylistSongs)
                    .HasForeignKey(e => e.SongId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QuizRound>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("QuizRounds");
                entity.HasOne(e => e.Song)
                    .WithMany(e => e.QuizRounds)
                    .HasForeignKey(e => e.SongId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Hint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Hints");
                entity.Property(e => e.Content).IsRequired();
                entity.HasOne(e => e.QuizRound)
                    .WithMany(e => e.Hints)
                    .HasForeignKey(e => e.QuizRoundId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}