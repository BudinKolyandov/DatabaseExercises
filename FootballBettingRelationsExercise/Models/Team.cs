using System.Collections.Generic;

namespace Models
{

    public class Team
    {
        public Team()
        {
            this.Players = new List<Player>();
        }

        public int TeamId { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string Initials { get; set; }

        public decimal Budget { get; set; }

        public int PrimaryColorId { get; set; }
        public Color PrimaryColor { get; set; }


        public int SecondaryColorId { get; set; }
        public Color SecondaryColor { get; set; }

        public int TownId { get; set; }
        public Town Town { get; set; }

        public ICollection<Player> Players { get; set; }

    }
}
