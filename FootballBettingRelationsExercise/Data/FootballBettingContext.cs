using System;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class FootballBettingContext : DbContext
    {
        protected FootballBettingContext()
        {
        }
        public FootballBettingContext(DbContextOptions options) 
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreatingTeamEntity(modelBuilder);
            CreatingColorEntity(modelBuilder);
            CreatingTownEntity(modelBuilder);
            CreatingCountryEntity(modelBuilder);
            CreatingPlayerEntity(modelBuilder);
            CreatingPositionEntity(modelBuilder);
            CreatingPlayerStatisticEntity(modelBuilder);
            CreatingGameEntity(modelBuilder);
            CreatingBetEntity(modelBuilder);
            CreatingUserEntity(modelBuilder);

        }

        private void CreatingTeamEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingColorEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingTownEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingCountryEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingPlayerEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingPositionEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingPlayerStatisticEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingGameEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingBetEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void CreatingUserEntity(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
