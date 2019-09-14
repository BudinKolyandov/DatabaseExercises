using System;
using System.Collections.Generic;

namespace Models
{
    public class User
    {
        public User()
        {
            this.Bets = new List<Bet>();
        }

        public int UserId { get; set; }

        public string Username { get; set; }

        public Guid Password { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public ICollection<Bet> Bets { get; set; }


    }
}