using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileSpoof
{
    class Client
    {
        StreamReader reader;
        StreamWriter writer;
        int cId;
        public Client(int id, string name, int port)
        {
            bool connected = false;
            cId = id;
            while (!connected)
            {
                try
                {
                    TcpClient client = new TcpClient("localhost", port);
                    reader = new StreamReader(client.GetStream());
                    writer = new StreamWriter(client.GetStream());
                    writer.AutoFlush = true;

                    // De server kan niet zien van welke poort wij client zijn, dit moeten we apart laten weten
                    writer.WriteLine("C " + id + " " + name + " " + Program.port);

                    connected = true;

                    Console.WriteLine("Verbonden: " + port);

                    // Start het reader-loopje
                    new Thread(ReaderThread).Start();
                }
                catch
                {
                    Console.WriteLine("Connection to server lost. Reconnect? (y/n)");
                    string ans = Console.ReadLine();
                    if (ans == "y")
                    {
                        connected = false;
                    }
                }


                Input();
            }
        }

        public void ReaderThread()
        {
            try
            {
                while (true)
                {
                    string s = reader.ReadLine();
                    try
                    {
                        if (Int32.Parse(s.Split(' ')[0]) != cId)
                        {
                            Console.WriteLine("Got a message for client " + s.Split(' ')[0]);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Got a message that was garbage");
                        continue;
                    }

                    Console.WriteLine(s); //Instead of this, do message handling in real app


                }
            }
            catch { } // Verbinding is kennelijk verbroken
        }

        public void Input()
        {
            while (true)
            { writer.WriteLine(Console.ReadLine()); }
        }
    }
}
