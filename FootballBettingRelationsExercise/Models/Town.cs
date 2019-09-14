using System.Collections.Generic;

namespace Models
{
    public class Town
    {
        public Town()
        {
            this.Teams = new List<Team>();
            this.HomeGames = new List<Game>();
            this.AwayGames = new List<Game>();
        }

        public int TownId { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public ICollection<Team> Teams { get; set; }

        public ICollection<Game> HomeGames { get; set; }

        public ICollection<Game> AwayGames { get; set; }
    }
}