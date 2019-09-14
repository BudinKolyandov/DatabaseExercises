using System.Collections.Generic;

namespace Models
{
    public class Color
    {
        public Color()
        {
            this.PrimaryKitTeam = new List<Team>();
            this.SecondaryKitTeam = new List<Team>();
        }


        public int ColorId { get; set; }
        
        public string Name { get; set; }

        public ICollection<Team> PrimaryKitTeam { get; set; }

        public ICollection<Team> SecondaryKitTeam { get; set; }

    }
}