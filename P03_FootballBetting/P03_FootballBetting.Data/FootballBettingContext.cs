using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.Configuration.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(e =>
            {
                e.HasKey(t => t.TeamId);

                e.Property(t => t.Name)
                .HasMaxLength(75)
                .IsRequired(true)
                .IsUnicode(true);

                e.Property(t => t.LogoUrl)
                .HasMaxLength(250)
                .IsRequired(false)
                .IsUnicode(false);

                e.Property(t => t.Initials)
                .HasMaxLength(3)
                .IsRequired(true)
                .IsUnicode(true);

                e.Property(t => t.Budget)
                .IsRequired(true);

                e.HasOne(t => t.PrimaryKitColor)
                .WithMany(c => c.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.Town)
                .WithMany(to => to.Teams)
                .HasForeignKey(t => t.TownId);
            });

            modelBuilder.Entity<Color>(e =>
            {
                e.HasKey(c => c.ColorId);

                e.Property(c => c.Name)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(true);
            });

            modelBuilder.Entity<Town>(e =>
            {
                e.HasKey(t => t.TownId);

                e.Property(t => t.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(true);

                e.HasOne(t => t.Country)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.CountryId);
            });

            modelBuilder.Entity<Country>(e =>
            {
                e.HasKey(c => c.CountryId);

                e.Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(true);
            });

            modelBuilder.Entity<Player>(e =>
            {
                e.HasKey(p => p.PlayerId);

                e.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(true);

                e.Property(p => p.SquadNumber)
                .IsRequired(true);

                e.Property(p => p.IsInjured)
                .IsRequired(true);

                e.HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

                e.HasOne(p => p.Position)
                .WithMany(po => po.Players)
                .HasForeignKey(p => p.PositionId);
            });

            modelBuilder.Entity<Position>(e =>
            {
                e.HasKey(p => p.PositionId);

                e.Property(p => p.Name)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(true);
            });

            modelBuilder.Entity<PlayerStatistic>(e =>
            {
                e.HasKey(ps => new { ps.PlayerId, ps.GameId });

                e.Property(ps => ps.ScoredGoals)
                .IsRequired(true);

                e.Property(ps => ps.Assists)
                .IsRequired(true);

                e.Property(ps => ps.MinutesPlayed)
                .IsRequired();

                e.HasOne(ps => ps.Game)
                .WithMany(g => g.PlayerStatistics)
                .HasForeignKey(ps => ps.GameId);

                e.HasOne(ps => ps.Player)
                .WithMany(p => p.PlayerStatistics)
                .HasForeignKey(ps => ps.PlayerId);
            });

            modelBuilder.Entity<Game>(e =>
            {
                e.HasKey(g => g.GameId);

                e.Property(g => g.HomeTeamGoals)
                .IsRequired(true);

                e.Property(g => g.AwayTeamGoals)
                .IsRequired(true);

                e.Property(g => g.DateTime)
                .IsRequired(true);

                e.Property(g => g.HomeTeamBetRate)
                .IsRequired(true);

                e.Property(g => g.AwayTeamBetRate)
                .IsRequired(true);

                e.Property(g => g.DrawBetRate)
                .IsRequired(true);

                e.Property(g => g.Result)
                .HasMaxLength(10)
                .IsRequired(true);

                e.HasOne(g => g.HomeTeam)
                .WithMany(ht => ht.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(g => g.AwayTeam)
                .WithMany(at => at.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
                
            });

            modelBuilder.Entity<Bet>(e =>
            {
                e.HasKey(b => b.BetId);

                e.Property(b => b.Amount)
                .IsRequired(true);

                e.Property(b => b.Prediction)
                .IsRequired(true);

                e.Property(b => b.DateTime)
                .IsRequired(true);

                e.HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserId);

                e.HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId);

            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserId);

                e.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(false);

                e.Property(u => u.Password)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(false);

                e.Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(false);

                e.Property(u => u.Name)
                .HasMaxLength(100)
                .IsRequired(false)
                .IsUnicode(true);

                e.Property(u => u.Balance)
                .IsRequired(true);

            });

        }
    }
}
