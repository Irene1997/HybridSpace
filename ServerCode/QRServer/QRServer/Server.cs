using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QRServer
{
    class Server
    {
        private Client[] clientTable;
        private int[][] teamTable;
        private int[] teamSizes;

        private const int serverPort = 666;
        private int scoreOnScan = 1;
        private int scoreOnScanned = 0;

        public Server()
        {
            //Ask maximum amount of players for this game
            Console.WriteLine("Maxiumum number of players?");
            clientTable = new Client[Int32.Parse(Console.ReadLine())];

            //Ask amount of teams for this game
            Console.WriteLine("Amount of teams?");
            teamTable = new int[Int32.Parse(Console.ReadLine())][];
            teamSizes = new int[teamTable.Length];

            //For each team, get a table of which teams they can scan (input format: numbers divided by spaces)
            for (int i = 0; i < teamTable.Length; i++)
            {
                Console.WriteLine("Team " + i + " can scan?");
                string[] input = Console.ReadLine().Split(" ");
                teamTable[i] = new int[input.Length];
                teamSizes[i] = 0;

                for (int j = 0; j < input.Length; j++)
                {
                    int t = Int32.Parse(input[j]);
                    if (t >= teamTable.Length) { throw new System.ArgumentException("Team number cannot be larger than maximum amount of teams"); }

                    teamTable[i][j] = t;
                }
            }

            TcpListener listener = new TcpListener(IPAddress.Any, serverPort);

            //Thread t = new Thread(() => Listen(client));

            Thread listenThread = new Thread(() => ListenForRequests(listener));
            listenThread.Start();

            Input();
        }

        #region internal stuff

        /// <summary>
        /// When a new client connects, this puts that client in the players table 
        /// </summary>
        /// <param name="input">input gained from the message handler</param>
        /// <returns></returns>
        private void ClientConnect(string[] input, StreamReader read, StreamWriter write)
        {
            int id = Int32.Parse(input[1]);
            string name = input[2];
            int port = Int32.Parse(input[3]);

            if (id >= clientTable.Length)
            {
                Client tClient = new Client(id, -1, name, 0, read, write, port);
                SendMessage(tClient, tClient.ClientID + " IC Invalid_ID");
                return;
            }

            Client client;

            if (clientTable[id] == null)
            {
                NewClientConnect(id, name, read, write, port);
            }
            else
            {
                client = clientTable[id];
                ClientReconnect(client, read, write);
            }

            Console.WriteLine("Client " + id + " connected on port " + port);
          

        }

        /// <summary>
        /// Connect a new player to the game, and assign them a team etc.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="read"></param>
        /// <param name="write"></param>
        /// <param name="port"></param>
        public void NewClientConnect(int id, string name, StreamReader read, StreamWriter write, int port)
        {
            int team = GetSmallestTeam();

            teamSizes[team]++; //Increase the team size of the team this player joins

            Client client = new Client(id, team, name, 0, read, write, port);
            clientTable[id] = client;

            Thread t = new Thread(() => Listen(client));
            t.Start();

            SendMessage(client, client.ClientID + " SC " + client.Team);

        }


        /// <summary>
        /// Method of a client reconnecting to the game
        /// </summary>
        /// <param name="client"></param>
        public void ClientReconnect(Client client, StreamReader reader, StreamWriter writer)
        {
            client.reader = reader;
            client.writer = writer;

            client.Connected = true;
            SendMessage(client, client.ClientID + " RC"); //Tell client it sucessfully reconnected

            Thread t = new Thread(() => Listen(client));
            t.Start();

            //Send the backlog of messages to the client
            while (true)
            {
                string message = client.DequeueBackLog();
                if(message == "empty") { break; }
                SendMessage(client, message);
            }

        }

        /// <summary>
        /// Takes the client out of the players table if they stop playing
        /// </summary>
        /// <param name="client">Which client stops playing</param>
        private void ClientStops(Client client)
        {
            clientTable[client.ClientID] = null;

            //Send message to client it has stopped playing succesfully
            SendMessage(client, client.ClientID + " SD");
            ClientDisconnect(client);

            //throw new System.NotImplementedException("Client Disconnection is not implemented yet");
        }

        private void ClientDisconnect(Client client)
        {
            client.Connected = false;
            Console.WriteLine("Client " + client.ClientID + " disconnected");
        }

        /// <summary>
        /// Handles messages coming in from clients
        /// </summary>
        /// <param name="message">The message from the client</param>
        private void MessageHandling(string message)
        {
            string[] input = message.Split(" ");
            Client c;
            switch (input[0])
            {
                case "S":
                    Scan(input);
                    break;

                case "D":
                    c = clientTable[Int32.Parse(input[1])];
                    if (c == null) { break; } //If the client trying to disconnect does not exist, exit
                    ClientDisconnect(c);
                    break;

                case "A":
                    c = clientTable[Int32.Parse(input[1])];
                    if(c==null) { break; }
                    Console.WriteLine("Client " + c.ClientID + " asked for Acknowledgement");
                    SendMessage(c, c.ClientID + " ACK");
                    break;

                default:
                    Console.WriteLine("Got a message that was garbage!"); break;
                    //throw new System.ArgumentException("Message has invalid type");
            }

            //throw new System.NotImplementedException("Message Handling is not implemented yet");
        }

        /// <summary>
        /// Handles a client scanning another client
        /// </summary>
        /// <param name="scanner">The client scanning</param>
        /// <param name="target">The client being scanned</param>
        private void Scan(Client scanner, Client target)
        {
            if (scanner.Team == target.Team)
            {
                //Send message to scanner that they tried to scan someone from their own team;
                SendMessage(scanner, scanner.ClientID + " OS");
                return;
            }

            //foreach (int i in teamTable[scanner.Team])
            for (int i = 0; i < teamTable[scanner.Team].Length; i++)
            {
                if (target.Team == teamTable[scanner.Team][i])
                {
                    teamSizes[target.Team]--;
                    teamSizes[scanner.Team]++;

                    target.Team = scanner.Team;

                    //Send message to Target that they have been scanned and what their new team is
                    target.Score += scoreOnScanned;
                    scanner.Score += scoreOnScan;
                    SendMessage(target, target.ClientID + " GS " + scanner.ClientID + " " + scanner.Name + " " + scanner.Team + " " + target.Score);
                    //Send message to Scanner that they succesfully scanned someone and converted them to their team
                    SendMessage(scanner, scanner.ClientID + " SS " + target.ClientID + " " + target.Name + " " + scanner.Score);

                    return;
                }
            }

            //Send message to Target that someone tried to scan them
            SendMessage(target, target.ClientID + " TS " + scanner.ClientID + " " + scanner.Name + " " + scanner.Team);
            //Send message to Scanner that they tried to scan someone they can't scan
            SendMessage(scanner, scanner.ClientID + " CS " + target.Team);
        }

        /// <summary>
        /// Overload of Scan that takes a Message as input
        /// </summary>
        /// <param name="input">Input message</param>
        private void Scan(string[] input)
        {
            int scannerID = Int32.Parse(input[1]);
            int targetID = Int32.Parse(input[2]);

            if(scannerID >= clientTable.Length) { return; } //If the scanner client is out of bounds, do nothing
            Client scanner = (clientTable[Int32.Parse(input[1])]);

            if (targetID >= clientTable.Length)
            {
                SendMessage(scanner, scanner.ClientID + " IS");
                return;
            }
            Client target = (clientTable[Int32.Parse(input[2])]);

            if (scanner == null) { return; } //If the scanner client does not exist, do nothing
            if (target == null)
            {
                //Send message to scanner saying the target doesn't exist
                SendMessage(scanner, scanner.ClientID + " IS");
                return;
            }

            Scan(scanner, target);
        }

        /// <summary>
        /// Sends string messages to clients
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void SendMessage(Client client, string message)
        {
            if (client == null) //Client does not exist or has stopped playing
            { return; } 

            if (client.Connected) { client.writer.WriteLine(message); } //If the client is connected, send the message to the client
            else { client.EnqueueMessage(message); } //If the client is not connected, log the message for later
        }

        /// <summary>
        /// Calculates which team currently has the fewest members
        /// </summary>
        /// <returns>The id of the team with the fewest members</returns>
        private int GetSmallestTeam()
        {
            int smallest = Int32.MaxValue;
            int result = -1;

            for (int i = 0; i < teamSizes.Length; i++)
            {
                if (teamSizes[i] < smallest) { smallest = teamSizes[i]; result = i; }
            }

            if (result < 0) { throw new ArgumentOutOfRangeException("Okay but how the fuck did you end up with no teams exaclty?"); }
            return result;
        }

        #endregion

        #region networking stuff

        /// <summary>
        /// Accepts new Players into the game
        /// </summary>
        /// <param name="listener"></param>
        private void ListenForRequests(TcpListener listener)
        {
            //listener = new TcpListener(IPAddress.Parse("127.0.0.1"),port );
            listener.Start();

            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    StreamReader clientIn = new StreamReader(client.GetStream());
                    StreamWriter clientOut = new StreamWriter(client.GetStream());
                    clientOut.AutoFlush = true;

                    string[] firstLine = clientIn.ReadLine().Split(" ");

                    ClientConnect(firstLine, clientIn, clientOut);
                }
            }

            catch (SocketException e)
            {
                Console.WriteLine("Yay socket exception");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Method that handles messages from clients
        /// </summary>
        /// <param name="client"></param>
        public void Listen(Client client)
        {
            Console.WriteLine("Listening to client " + client.ClientID);
            try
            {
                while (true)
                {
                    MessageHandling(client.reader.ReadLine());
                }
            }
            catch
            {
                ClientDisconnect(client);
            } //No connection, that's fine
        }

        #endregion

        private void Input()
        {
            while(true)
            {
                string input = Console.ReadLine();

                int id;
                try
                {
                    id = Int32.Parse(input.Split(" ")[0]);
                }
                catch
                {
                    Console.WriteLine("Invalid message format");
                    continue;
                }
                
                SendMessage(clientTable[id], input);
            }
        }
    }
}