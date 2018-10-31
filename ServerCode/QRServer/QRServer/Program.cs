using System;

namespace QRServer
{
    class Program
    {
        static private Client[] clientTable;
        static private int[][] teamTable;
        static private int[] teamSizes;

        static void Main(string[] args)
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
                    if(t>=teamTable.Length) { throw new System.ArgumentException("Team number cannot be larger than maximum amount of teams"); }

                    teamTable[i][j] = t;
                }
            }
        }
        
        /// <summary>
        /// When a new client connects, this puts that client in the players table 
        /// </summary>
        /// <param name="input">input gained from the message handler</param>
        /// <returns></returns>
        private void ClientConnect(string[] input)
        {
            int id = Int32.Parse(input[1]);
            string name = input[2];
            int team = GetSmallestTeam();
            teamSizes[team]++;

            Client client = new Client(id, team, name);
            clientTable[id] = client;

            //Send message to Client that it has successfully connected, and which team the client belongs to
            SendMessage(client, client.ClientID + " SC " + client.Team);

            //throw new System.NotImplementedException("Client Connection is not implemented yet");

        }

        /// <summary>
        /// Takes the client out of the players table
        /// </summary>
        /// <param name="client">Which client is disconnecting</param>
        private void ClientDisconnect(Client client)
        {
            clientTable[client.ClientID] = null;

            //Send message to client it has disconnected succesfullys
            SendMessage(client, client.ClientID + " SD");

            //throw new System.NotImplementedException("Client Disconnection is not implemented yet");

        }

        /// <summary>
        /// Handles messages coming in from clients
        /// </summary>
        /// <param name="message">The message from the client</param>
        private void MessageHandling(string message)
        {
            string[] input = message.Split(" ");

            switch(input[0])
            {
                case "S":
                    Scan(input);
                    break;

                case "C":
                    ClientConnect(input);
                    break;

                case "D":
                    Client c = clientTable[Int32.Parse(input[1])];
                    if(c == null) { break; } //If the client trying to disconnect does not exist, exit
                    ClientDisconnect(c);
                    break;

                default:
                    throw new System.ArgumentException("Message has invalid type");
            }

            throw new System.NotImplementedException("Message Handling is not implemented yet");
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
                    SendMessage(target, target.ClientID + " GS " + scanner.ClientID + " " + scanner.Name + " " + scanner.Team + " " + 0);
                    //Send message to Scanner that they succesfully scanned someone and converted them to their team
                    SendMessage(scanner, scanner.ClientID + " SS " + target.ClientID + " " + target.Name + " " + 1);

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
            Client scanner = (clientTable[Int32.Parse(input[1])]);
            Client target = (clientTable[Int32.Parse(input[2])]);

            if(scanner == null) { return; } //If the scanner client does not exist, do nothing
            if(target == null)
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
            throw new System.NotImplementedException("Message Sending is not implemented yet");
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
                if(teamSizes[i]<smallest) { smallest = teamSizes[i]; result = i; }
            }

            if(result<0) { throw new ArgumentOutOfRangeException("Okay but how the fuck did you end up with no teams exaclty?"); }
            return result;
        }
    }
}
