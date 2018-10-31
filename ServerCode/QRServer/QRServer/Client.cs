using System;
using System.Collections.Generic;
using System.Text;

namespace QRServer
{
    /// <summary>
    /// Class representing all info of a player playing the game.
    /// </summary>
    class Client
    {
        public int ClientID { get; }
        public int Team { get; set; }
        public string Name { get; }
        public int Score { get; set; }


        public Client(int id, int team, string name, int score = 0)
        {
            ClientID = id;
            Team = team;
            Name = name;
            Score = score;
        }
    }
}
