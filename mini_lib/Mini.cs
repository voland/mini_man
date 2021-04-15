using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
/* using CsvHelper; */
/* using CsvHelper.Configuration.Attributes; */
/* using System.Globalization; */
/* using System.Text; */
/* using System.Net; */

namespace mini_lib {
    public class Mini {

        private string contrast_tag = "cont";
        private string nightcontrast_tag = "ncon";
        private string time_tag = "time";
        private string standby_tag = "stby";
        private string custom_data_tag = "cuda";
        private string pin_tag = "pin";
        private string net_tag = "net";
        private string page_tag = "page";
        private string mesg_tag = "mesg";
        private string netconf_tag = "neco";
        private string begin_of_transmition = "<data>";
        private string end_of_transmition = "</data>";

        private byte[] bot {
            get {
                return Enc.e.GetBytes(begin_of_transmition);
            }
        }

        private byte[] eot {
            get {
                return Enc.e.GetBytes(end_of_transmition);
            }
        }

        private string create_request(string tag, string content) {
            return string.Format("<{0}>{1}</{0}>", tag, content);
        }

        private string ip;

        public Mini() {
            ip = "192.168.4.1";
        }

        public Mini(string ip) {
            this.ip = ip;
        }

        TcpClient client = null;

        public static object TransferLocker = new object();

        private static int cmdport = 2323;

        private void CloseCommandConn() {
            if (client != null) {
                client.Close();
                client = null;
            }
        }

        public List<Task> StartedTasks = new List<Task>();

        public void AwaitAllTasksFinished() {
            foreach (Task t in StartedTasks) {
                if (t != null) {
                    while (t.Status == TaskStatus.Running) ;
                }
            }
        }

        private async Task SendStringAsync(string request) {
            Task t = Task.Run(() => {
                lock (TransferLocker) {
                    try {
                        Console.WriteLine("creating clitent tcp for {0}:{1} request is {2}", ip, cmdport, request);
                        client = new TcpClient(ip, cmdport);
                        using (Stream netStream = client.GetStream()) {
                            byte[] array = Enc.e.GetBytes(request);
                            netStream.Write(bot, 0, bot.Length);
                            netStream.Write(array, 0, array.Length);
                            netStream.Write(eot, 0, eot.Length);
                            System.Threading.Thread.Sleep(500);
                        }
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    } finally {
                        CloseCommandConn();
                    }
                }
            });
            StartedTasks.Add(t);
            await t;
        }

        public async Task SendPresAsync(Pres pres) {
            Task t = Task.Run(() => {
                lock (TransferLocker) {
                    try {
                        using (MemoryStream ms = new MemoryStream()) {
                            using (BinaryWriter bw = new BinaryWriter(ms)) {
                                pres.Serialise(bw);
                                client = new TcpClient(ip, cmdport);
                                Stream netStream = client.GetStream();
                                ms.Seek(0, SeekOrigin.Begin);
                                byte[] array = new byte[ms.Length];
                                ms.Read(array, 0, (int)Math.Min(array.Length, ms.Length));
                                netStream.Write(bot, 0, bot.Length);
                                netStream.Write(array, 0, array.Length);
                                netStream.Write(eot, 0, eot.Length);
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                    } catch (Exception e) { Console.WriteLine(e.Message); } finally {
                        CloseCommandConn();
                    }
                }
            });
            StartedTasks.Add(t);
            await t;
        }

        public async Task SendContrastAsync(int k) {
            await SendStringAsync(create_request(contrast_tag, k.ToString()));
        }

        public async Task SendNightContrastAsync(int k) {
            await SendStringAsync(create_request(nightcontrast_tag, k.ToString()));
        }

        public async Task SendCustomStringAsync(string cd) {
            await SendStringAsync(create_request(custom_data_tag, cd));
        }

        public async Task SendStandbyTimeAsync(int disH, int disM, int enaH, int enaM) {
            await SendStringAsync(create_request(standby_tag, string.Format("{0};{1};{2};{3}", disH, disM, enaH, enaM)));
        }

        public async Task SendPinAsync(string pin) {
            if (pin.Length == 4) {
                await SendStringAsync(create_request(pin_tag, pin));
            }
        }

        public async Task SendNetConfAsync(string ssid, string pwd) {
            if ((ssid != null) & (pwd != null)) {
                string s = $"{ssid}:{pwd}";
                await SendStringAsync(create_request(net_tag, s));
            }
        }

        public async Task SendMessageAsync(MessageRgb msg) {
            if (msg != null) {
                await SendStringAsync(create_request(mesg_tag, msg.ToString()));
            }
        }

        public async Task SendTimeAsync() {
            int year, month, dom, hour, min, sec;
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            dom = DateTime.Now.Day;
            hour = DateTime.Now.Hour;
            min = DateTime.Now.Minute;
            sec = DateTime.Now.Second;
            string time_representation = string.Format("{0};{1};{2};{3};{4};{5}", year, month, dom, hour, min, sec);
            await SendStringAsync(create_request(time_tag, time_representation));
        }

        public async Task SendNetConfAsync(string ip, string ma, string gw) {
            string net_conf_representation = string.Format("{0};{1};{2}", ip, ma, gw);
            await SendStringAsync(create_request(net_tag, net_conf_representation));
        }

        public async Task SendPageNrAsync(int PageNr) {
            await SendStringAsync(create_request(page_tag, PageNr.ToString()));
        }
    }
}
