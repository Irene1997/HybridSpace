using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiClientServer
{
    class Program
    {
        public const int maxNodes = 20;
        public const int offset = 55500;

        static public int mijnPoort;

        // directe verbindingen 
        static public Connection[] buren = new Connection[maxNodes];

        // afstanden tot bestemmingen
        static private int[] afstanden = new int[maxNodes];

        // kortste pad naar een bestemming
        static private int[] kortstePad = new int[maxNodes];

        // geschatte afstand van een node naar een bestemming
        static private int[,] nDisTabel = new int[maxNodes, maxNodes];

        static void Main(string[] args) {
            // args[0] is ons poortnummer
            if (args.Length > 0) {
                mijnPoort = int.Parse(args[0]);
            }

            Console.Title = "NetChange " + mijnPoort;

            for (int i = 0; i < maxNodes; i++) {
                afstanden[i] = maxNodes;
                for (int j = 0; j < maxNodes; j++) {
                    nDisTabel[i, j] = maxNodes;
                }
            }
            UpdateKortstePad(mijnPoort, 0, mijnPoort);

            // start de server met ons poortnummer
            new Server(mijnPoort);

            // verbind met andere servers
            for (int i = 1; i < args.Length; i++) {
                int poort = int.Parse(args[i]);

                if (poort < mijnPoort) {
                    lock (buren) {
                        buren[poort - offset] = new Connection(poort);
                    }
                }
            }

            // doe iets met gebruiker's input
            Invoer();
        }

        static void Invoer() {
            while (true) {
                string input = Console.ReadLine();

                switch (input[0]) {
                    case 'R':
                        ToonRoutingTabel();
                        break;
                    case 'B':
                        StuurBericht(input);
                        break;
                    case 'C':
                        MaakVerbinding(input);
                        break;
                    case 'D':
                        VerbreekVerbinding(input);
                        break;
                }
            }
        }

        static void ToonRoutingTabel() {
            lock (afstanden) {
                for (int i = 0; i < maxNodes; i++) {
                    if (i == mijnPoort - offset) {
                        Console.WriteLine(mijnPoort + " 0 local");
                    } else {
                        int afstand = afstanden[i];
                        if (afstand < maxNodes) {
                            Console.WriteLine(i + offset + " " + afstand + " " + kortstePad[i]);
                        }
                    }
                }
            }
        }

        static void StuurBericht(string input) {
            // deel de input op in 'B', het poortnummer en het bericht
            string[] data = input.Split(new char[] { ' ' }, 3);

            int poort = int.Parse(data[1]);

            lock (buren) {
                if (afstanden[poort - offset] < maxNodes) {
                    buren[kortstePad[poort - offset] - offset].Write.WriteLine(input);
                }
            }
        }

        static void MaakVerbinding(string input) {
            string[] data = input.Split();

            int poort = int.Parse(data[1]);

            lock (buren) {
                if (buren[poort - offset] == null) {
                    buren[poort - offset] = new Connection(poort);
                }

                UpdateBuur(poort);
                UpdateKortstePad(poort, 1, poort);
            }
        }

        static public void VerbreekVerbinding(string input) {
            string[] data = input.Split();

            int poort = int.Parse(data[1]);

            lock (buren) {
                if (buren[poort - offset] == null) {
                    Console.WriteLine("Poort " + poort + " is niet bekend");
                } else {
                    //send message to disconnect
                    buren[poort - offset].Write.WriteLine("D " + mijnPoort);
                    buren[poort - offset] = null;

                    lock (nDisTabel) {
                        for (int i = 0; i < maxNodes; i++) {
                            if(nDisTabel[poort - offset, i] < maxNodes) {
                                UpdateKortstePad(i + offset, maxNodes - 1, poort);
                            }
                        }
                    }
                    Console.WriteLine("Verbroken: " + poort);
                }
            }
        }


        static public void UpdateKortstePad(int bestemming, int afstand, int via) {
            int pos, bestPos = maxNodes, bestDistance = maxNodes;

            if (afstand < maxNodes) {
                lock (afstanden) {
                    lock (nDisTabel) {
                        // update de nDisTabel
                        nDisTabel[via - offset, bestemming - offset] = afstand - 1;

                        // vindt de snelste route naar de bestemming
                        for (pos = 0; pos < maxNodes; pos++) {
                            int temp = nDisTabel[pos, bestemming - offset];

                            if (temp < bestDistance) {
                                bestDistance = temp;
                                bestPos = pos;
                            }
                        }
                    }
                    lock (kortstePad) {
                        // of afstand is anders of pad is anders
                        if (bestDistance + 1 != afstanden[bestemming - offset] || (bestPos + offset) != kortstePad[bestemming - offset]) {
                            afstanden[bestemming - offset] = bestDistance + 1;
                            kortstePad[bestemming - offset] = bestPos + offset;

                            // laat buren weten dat we een nieuw kortste pad hebben naar bestemming
                            for (int i = 0; i < maxNodes; i++) {
                                if (afstanden[i] == 1 && buren[i] != null) {
                                    buren[i].Write.WriteLine("U " + mijnPoort + " " + bestemming + " " + afstanden[bestemming - offset] + " " + kortstePad[bestemming - offset]);
                                }
                            }
                            // "afstand naar <bestemming> is nu <afstand> via <kortstepad>"
                            Console.WriteLine("afstand naar " + bestemming + " is nu " + afstanden[bestemming - offset] + " via " + kortstePad[bestemming - offset]);
                        }
                    }

                }
            } else {
                // unreachable
                lock (buren) {
                    lock (afstanden) {
                        if(afstanden[bestemming - offset] < maxNodes) {
                            buren[bestemming - offset] = null;
                            afstanden[bestemming - offset] = maxNodes;

                            ////any node that is visited through this node
                            //lock (kortstePad) {
                            //    for (int i = 0; i < maxNodes; i++) {
                            //        if (afstanden[i] < maxNodes && buren[i] != null) {
                            //            buren[i].Write.WriteLine("U " + mijnPoort + " " + bestemming + " " + afstanden[bestemming - offset] + " " + kortstePad[bestemming - offset]);
                            //        }
                            //    }
                            //}

                        }
                        
                    }
                }
                
                //kortstePad[bestemming - offset] = maxNodes;
                Console.WriteLine("Onbereikbaar: " + bestemming);


            }

        }

        static public void UpdateBuur(int poort) {
            for (int i = 0; i < maxNodes; i++) {
                if (afstanden[i] < maxNodes) {
                    buren[poort - offset].Write.WriteLine("U " + mijnPoort + " " + (i + offset) + " " + afstanden[i] + " " + kortstePad[i]);
                }
            }
        }

        static public void StuurBerichtDoor(string s) {
            // deel de input op in 'B', het poortnummer en het bericht
            string[] data = s.Split(new char[] { ' ' }, 3);

            int poort = int.Parse(data[1]);

            lock (buren) {
                if (afstanden[poort - offset] < maxNodes) {
                    // we schrijven naar de console dat we het bericht hebben doorgestuurd
                    // "Bericht voor bestemming doorgestuurd naar kortstepad"
                    int via = kortstePad[poort - offset];
                    Console.WriteLine("Bericht voor " + poort + " doorgestuurd naar " + via);
                    buren[via - offset].Write.WriteLine(s);
                }
            }
        }
    }
}
