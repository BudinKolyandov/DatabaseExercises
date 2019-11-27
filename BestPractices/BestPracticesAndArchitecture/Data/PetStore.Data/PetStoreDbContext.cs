using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Data
{
    class PetStoreDbContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Toy> Toys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(brand =>
            {
                brand.HasMany(b => b.Food)
                .WithOne(f => f.Brand)
                .HasForeignKey(f => f.BrandId)
                .OnDelete(DeleteBehavior.Restrict);


                brand.HasMany(b => b.Toys)
                .WithOne(t => t.Brand)
                .HasForeignKey(t => t.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Breed>(breed =>
            {
                breed.HasMany(b => b.Pets)
                .WithOne(p => p.Breed)
                .HasForeignKey(p => p.BreedId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(category =>
            {
                category.HasMany(c => c.Pets)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                category.HasMany(c => c.Toys)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                category.HasMany(c => c.Foods)
                .WithOne(f => f.Category)
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Order>(order =>
            {
                order.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                order.HasMany(o => o.Pets)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<FoodOrder>(fo =>
            {
                fo.HasKey(fo => new { fo.FoodId, fo.OrderId });

                fo.HasOne(fo => fo.Order)
                .WithMany(o => o.Foods)
                .HasForeignKey(fo => fo.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

                fo.HasOne(fo => fo.Food)
                .WithMany(f => f.Orders)
                .HasForeignKey(fo => fo.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<ToyOrder>(to =>
            {
                to.HasKey(to => new { to.ToyId, to.OrderId });

                to.HasOne(to => to.Order)
                .WithMany(o => o.Toys)
                .HasForeignKey(to => to.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

                to.HasOne(to => to.Toy)
                .WithMany(t => t.Orders)
                .HasForeignKey(to => to.ToyId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<User>(user =>
           {
               user.HasIndex(u => u.Email)
               .IsUnique();

           });

        }


    }
}
