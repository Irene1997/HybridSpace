using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
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
        public bool Connected { get; set; }
        private Queue<String> backLog = new Queue<string>();

        public StreamReader reader;
        public StreamWriter writer;


        public Client(int id, int team, string name, int score, StreamReader read, StreamWriter write, int port)
        { 
            ClientID = id;
            Team = team;
            Name = name;
            Score = score;
            reader = read;
            writer = write;
            Connected = true;
        }

        public void EnqueueMessage(string message)
        {
            backLog.Enqueue(message);
        }

        public string DequeueBackLog()
        {
            if (backLog.Count > 0) { return backLog.Dequeue(); }
            else { return "empty"; }
        }
    }
}
