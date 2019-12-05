namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongPerformer> SongsPerformers { get; set; }
        public DbSet<Writer> Writers { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SongPerformer>(sp =>
            {
                sp.HasKey(x => new { x.PerformerId, x.SongId });

                sp.HasOne<Performer>(p => p.Performer)
                .WithMany(p => p.PerformerSongs)
                .HasForeignKey(p => p.PerformerId)
                .OnDelete(DeleteBehavior.Restrict);

                sp.HasOne<Song>(s => s.Song)
                .WithMany(x => x.SongPerformers)
                .HasForeignKey(p => p.SongId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Song>(song =>
            {
                song.HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(x => x.AlbumId)
                .OnDelete(DeleteBehavior.Restrict);

                song.HasOne(s => s.Writer)
                .WithMany(w => w.Songs)
                .HasForeignKey(x => x.WriterId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Album>(album =>
            {
                album.HasOne(a => a.Producer)
                .WithMany(p => p.Albums)
                .HasForeignKey(x => x.ProducerId)
                .OnDelete(DeleteBehavior.Restrict);
            });


        }
    }
}
