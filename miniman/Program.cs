using System;
using mini_lib;
using System.Threading.Tasks;

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
                Console.WriteLine("    slides          send slides to device         ");
                Console.WriteLine("    contrast        send contrast to device       "); ;
                Console.WriteLine("    ncontrast       send night contrast to device ");
                Console.WriteLine("    slidenr         send sline number to show     ");
                Console.WriteLine("    time now        set internal cloc to now      ");
                Console.WriteLine("");
                Console.WriteLine("argument:");
                Console.WriteLine("    filename csv");
                Console.WriteLine("    contrast value");
                Console.WriteLine("    night contrast value");
                return;
            } else {
                Mini mini = new Mini();
                switch (args[0]) {
                    case "slides":
                        string filename = args[1];
                        Pres mp = new Pres(filename);
						mini.SendPresAsync(mp);
                        break;
                    case "ncontrast":
                        int night_contrast = -1;
                        if (Int32.TryParse(args[1], out night_contrast)) {
                            if (night_contrast > 0) mini.SendNightContrastAsync(night_contrast);
                        }
                        break;
                    case "contrast":
                        int contrast = -1;
                        if (Int32.TryParse(args[1], out contrast)) {
                            if (contrast > 0) mini.SendContrastAsync(contrast);
                        }
                        break;
                    case "time":
						mini.SendTimeAsync();
                        break;
                }
				mini.AwaitAllTasksFinished();
            }
        }
    }
}
