using System.Collections.Generic;

namespace Models
{
    public class PlayerStatistic
    {
        public PlayerStatistic()
        {
            this.Games = new List<Game>();
            this.Players = new List<Player>();
        }


        public int GameId { get; set; }
        public Game Game { get; set; }


        public int PlayerId { get; set; }
        public Player Player { get; set; }


        public int ScoredGoals { get; set; }

        public int Assists { get; set; }

        public double MinutesPlayed { get; set; }

        public ICollection<Game> Games { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
