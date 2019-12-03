namespace Cinema.Data
{
    using Cinema.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class CinemaContext : DbContext
    {
        public CinemaContext()  { }

        public CinemaContext(DbContextOptions options)
            : base(options)   { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Customer>(customer =>
            {
                customer.HasMany(x => x.Tickets)
                .WithOne(t => t.Customer)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            model.Entity<Hall>(hall =>
            {
                hall.HasMany(x => x.Seats)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);

                hall.HasMany(x => x.Projections)
                .WithOne(p => p.Hall)
                .HasForeignKey(p => p.HallId)
                .OnDelete(DeleteBehavior.Restrict);
            });


            model.Entity<Movie>(movie =>
            {
                movie.HasMany(x => x.Projections)
                .WithOne(p => p.Movie)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            model.Entity<Projection>(projection =>
            {
                projection.HasMany(x => x.Tickets)
                .WithOne(t => t.Projection)
                .HasForeignKey(t => t.ProjectionId)
                .OnDelete(DeleteBehavior.Restrict);
            });




        }
    }
}