using System;
using mini_lib;

namespace miniman {
    class Program {
        static void Main(string[] args) {
            if (args.Length != 2) {
                Console.WriteLine(" __  __ _       _ __  __                   ");
                Console.WriteLine("|  \\/  (_)_ __ (_)  \\/  | __ _ _ __        ");
                Console.WriteLine("| |\\/| | | '_ \\| | |\\/| |/ _` | '_ \\       ");
                Console.WriteLine("| |  | | | | | | | |  | | (_| | | | |_ _ _ ");
                Console.WriteLine("|_|  |_|_|_| |_|_|_|  |_|\\__,_|_| |_(_|_|_)");
                Console.WriteLine("");
                Console.WriteLine("Mini Manager GilBT");
                Console.WriteLine("usage: miniman [address ip] [command] [argument] sends command to mini device");
                Console.WriteLine("");
                Console.WriteLine("command:");
                Console.WriteLine("    slides          sends slides to device         ");
                Console.WriteLine("    contrast        sends contrast to device       "); ;
                Console.WriteLine("    night contrast  sends night contrast to device ");
                Console.WriteLine("");
                Console.WriteLine("argument:");
                Console.WriteLine("    filename csv");
                Console.WriteLine("    contrast value");
                Console.WriteLine("    night contrast value");
                return;
            } else {
                switch (args[0]) {
                    case "slides":
                        string filename = args[1];
                        Pres mp = new Pres(filename);
                        mp.SaveToBinFile("out.bin");
                        break;
                    case "nightcontrast":
                    case "contrast":
                        Console.WriteLine("command {0} not supported yet", args[0]);
                        break;
                }
            }
        }
    }
}
