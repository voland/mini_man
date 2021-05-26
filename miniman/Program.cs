using System;
using mini_lib;
using System.Threading.Tasks;

namespace miniman {
    class Program {
        static void PrintHelp() {
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
            Console.WriteLine("    contrast        send contrast to device (1-10)"); ;
            Console.WriteLine("    ncontrast       send night contrast to device (1-10)");
            Console.WriteLine("    page            send page number to show      ");
            Console.WriteLine("    time now        set internal cloc to now      ");
            Console.WriteLine("    standby         <dh>,<dm>,<eh>,<em> standby disable hr:mn, enable hr:mn");
            Console.WriteLine("    wificonf        <ssid>:<key>:<mode> sends ssid password and mode ap/sta (AccressPoint or Station)");
            Console.WriteLine("    string          send custom string");
            Console.WriteLine("    message         <msg>   send message");
            Console.WriteLine("    listen                  print available miniscreens");
            Console.WriteLine("");
            Console.WriteLine("argument:");
            Console.WriteLine("    filename csv");
            Console.WriteLine("    contrast value");
            Console.WriteLine("    night contrast value");
            Console.WriteLine("    night contrast value");
            Console.WriteLine("");
            Console.WriteLine("examples:");
            Console.WriteLine("    ./miniman 192.168.4.1 slides slajdy.csv");
            Console.WriteLine("    ./miniman 192.168.4.1 time now         ");
            Console.WriteLine("    ./miniman 192.168.4.1 contrast 8       ");
            Console.WriteLine("    ./miniman 192.168.4.1 slides slajdy.csv");
            Console.WriteLine("    ./miniman 192.168.4.1 standby 0,0,5,0     ");
            Console.WriteLine("    ./miniman 192.168.4.1 wificonf \"My Home:SecretKey:sta\"");
            Console.WriteLine("    ./miniman listen");
            return;
        }

        static void OnHaveFoundMini(sConfig c) {
            Console.WriteLine($"{DateTime.Now} Have found Mini:\n{c.ToString()}");
        }

        static void Main(string[] args) {
            if (args.Length < 3) {
                if (args.Length == 1) {
                    switch (args[0]) {
                        case "listen":
                            Console.WriteLine("Pres any key to stop!");
                            using (MiniListener ml = new MiniListener(OnHaveFoundMini)) {
                                Console.ReadKey();
                            }
                            break;
                        default:
                            PrintHelp();
                            break;
                    }
                } else {
                    PrintHelp();
                }
            } else {
                Mini mini = new Mini(args[0]);
                switch (args[1]) {
                    case "slides":
                        string filename = args[2];
                        Pres mp = new Pres(filename);
                        mini.SendPresAsync(mp);
                        break;
                    case "ncontrast":
                        int night_contrast = -1;
                        if (Int32.TryParse(args[2], out night_contrast)) {
                            if (night_contrast > 0) mini.SendNightContrastAsync(night_contrast);
                        }
                        break;
                    case "contrast":
                        int contrast = -1;
                        if (Int32.TryParse(args[2], out contrast)) {
                            if (contrast > 0) mini.SendContrastAsync(contrast);
                        }
                        break;
                    case "time":
                        mini.SendTimeAsync();
                        break;
                    case "page":
                        int page = -1;
                        if (Int32.TryParse(args[2], out page)) {
                            if (page > 0) mini.SendPageNrAsync(page);
                        }
                        break;
                    case "standby":
                        string[] a = args[2].Split(",");
                        int dh, dm, eh, em;
                        if (Int32.TryParse(a[0], out dh)) {
                            if (Int32.TryParse(a[1], out dm)) {
                                if (Int32.TryParse(a[2], out eh)) {
                                    if (Int32.TryParse(a[3], out em)) {
                                        mini.SendStandbyTimeAsync(dh, dm, eh, em);
                                    }
                                }
                            }
                        }
                        break;
                    case "string":
                        mini.SendCustomStringAsync(args[2]);
                        break;
                    case "message":
                        MessageRgb m = new MessageRgb(args[2], 2, 3, true);
                        mini.SendMessageAsync(m);
                        break;
                    case "wificonf":
                        mini.SendWifiConfAsync(args[2]);
                        break;
                    default:
                        Console.WriteLine("No such command!");
                        PrintHelp();
                        break;
                }
                mini.AwaitAllTasksFinished();
            }
        }
    }
}
