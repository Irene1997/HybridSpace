using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSpoof
{
    class Program
    {
        public static int port;
        static void Main(string[] args)
        {
            bool hasName = false;
            Console.WriteLine("What's my ID?");
            int id = Int32.Parse(Console.ReadLine());
            Console.WriteLine("What's my port?");
            port = Int32.Parse(Console.ReadLine());
            Console.WriteLine("What's my name? Name may not contain spaces");
            string name = "default";
            while (!hasName)
            {
                string tName = Console.ReadLine();
                if(tName.Contains(" ")) { Console.WriteLine("Name may not contain spaces"); continue; }
                name = tName;
                hasName = true;
            }
            Console.WriteLine("What's the server port?");
            int serverPort = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Connecting to Server");
            Client client = new Client(id,name,serverPort);
        }
    }
}
