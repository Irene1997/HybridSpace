using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace MultiClientServer
{
    class Connection
    {
        public StreamReader Read;
        public StreamWriter Write;

        // Connection heeft 2 constructoren: deze constructor wordt gebruikt als wij CLIENT worden bij een andere SERVER
        public Connection(int poort) {
            bool verbonden = false;

            while (!verbonden) {
                try {
                    TcpClient client = new TcpClient("localhost", poort);
                    Read = new StreamReader(client.GetStream());
                    Write = new StreamWriter(client.GetStream());
                    Write.AutoFlush = true;

                    // De server kan niet zien van welke poort wij client zijn, dit moeten we apart laten weten
                    Write.WriteLine("Poort: " + Program.mijnPoort);

                    verbonden = true;

                    Console.WriteLine("Verbonden: " + poort);

                    // Start het reader-loopje
                    new Thread(ReaderThread).Start();
                } catch { }
            }
        }

        // Deze constructor wordt gebruikt als wij SERVER zijn en een CLIENT maakt met ons verbinding
        public Connection(StreamReader read, StreamWriter write) {
            Read = read; Write = write;

            // Start het reader-loopje
            new Thread(ReaderThread).Start();
        }

        // LET OP: Nadat er verbinding is gelegd, kun je vergeten wie er client/server is (en dat kun je aan het Connection-object dus ook niet zien!)

        // Deze loop leest wat er binnenkomt en print dit
        public void ReaderThread() {
            try {
                while (true) {
                    string s = Read.ReadLine();

                    switch (s[0]) {
                        case 'U':
                            Update(s);
                            break;
                        case 'B':
                            Bericht(s);
                            break;
                        case 'D':
                            Disconnect(s);
                            break;
                    }
                }
            } catch { } // Verbinding is kennelijk verbroken
        }

        void Update(string s) {
            string[] data = s.Split();

            // "'U' via bestemming (afstand - 1) viavia"
            int via = int.Parse(data[1]);
            int bestemming = int.Parse(data[2]);
            int afstand = int.Parse(data[3]);

            // anderen kan ik met afstand + 1 bereiken via degene die mij het bericht stuurde
            Program.UpdateKortstePad(bestemming, afstand + 1, via);
        }

        void Bericht(string s) {
            string[] data = s.Split(new char[] { ' ' }, 3);

            // "'B' bestemming bericht"
            int bestemming = int.Parse(data[1]);

            // als het bericht voor ons bedoeld is schrijven we het naar de console
            if (bestemming == Program.mijnPoort) {
                Console.WriteLine(data[2]);
            }
            // anders sturen we het bericht door
            else {
                Program.StuurBerichtDoor(s);
            }
        }

        void Disconnect(string s) {
            Program.VerbreekVerbinding(s);
        }
    }
}
