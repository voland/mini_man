using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
/* using CsvHelper; */
/* using CsvHelper.Configuration.Attributes; */
/* using System.Globalization; */
/* using System.Text; */
/* using System.Net; */

namespace mini_lib {

    public enum eWifiMode {
        sta,
        ap
    }

    public class sConfig {
        public string ip { set; get; }
        const int SSID_KEY_LEN = 64;
        public byte contrast;
        public byte nightContrast;
        public byte nightOffHour;
        public byte nightOffMin;
        public byte nightOnHour;
        public byte nightOnMin;
        private char[] _pin = new char[5];
        public byte kalibracjaTemp;
        public byte maxPower;
        public uint width;
        public byte GroupSize; //Group size mode czyli czy ma odtwarzac slajdy po kolei czy tez ma wyswietlac w zaleznosci od przycisku
        public byte beginPage;
        public byte OnlyFirstLine;
        public byte rs485_mode;
        public char[] _ssid = new char[SSID_KEY_LEN];
        public char[] _key = new char[SSID_KEY_LEN];
        public eWifiMode wifi_mode;

        string Pin {
            get {
                return new String(_pin);
            }
            set {
                char[] tab = value.ToCharArray();
                _pin[0] = tab[0];
                _pin[1] = tab[1];
                _pin[2] = tab[2];
                _pin[3] = tab[3];
                _pin[4] = (char)0;
            }
        }

        public sConfig(string ip) {
            this.ip = ip;
        }

        public string ssid {
            get {
                return new String(_ssid);
            }
        }

        public string key {
            get {
                return new String(_key);
            }
        }

        public static sConfig GetFromB64code(byte[] b64code, string ip) {
            sConfig c = new sConfig(ip);
            if (b64code.Length >= 20) {
                string b64str = System.Text.Encoding.UTF8.GetString(b64code);
                b64str = b64str.Remove(0, b64str.IndexOf(':') + 1);
                byte[] data = System.Convert.FromBase64String(b64str);
                int i = 0;
                c.contrast = data[i++];
                c.nightContrast = data[i++];
                c.nightOffHour = data[i++];
                c.nightOffMin = data[i++];
                c.nightOnHour = data[i++];
                c.nightOnMin = data[i++];
                c._pin[0] = (char)data[i++];
                c._pin[1] = (char)data[i++];
                c._pin[2] = (char)data[i++];
                c._pin[3] = (char)data[i++];
                c._pin[4] = (char)data[i++];
                c.kalibracjaTemp = data[i++];
                c.maxPower = data[i++];
                c.width = data[i++];
                c.width += ((uint)data[i++]) << 8;
                c.width += ((uint)data[i++]) << 16;
                c.width += ((uint)data[i++]) << 24;
                c.GroupSize = data[i++];
                c.beginPage = data[i++];
                c.OnlyFirstLine = data[i++];
                c.rs485_mode = data[i++];
                for (int j = 0; (j < SSID_KEY_LEN) & (i < data.Length); j++) {
                    c._ssid[j] = (char)data[i++];
                }
                for (int j = 0; (j < SSID_KEY_LEN) & (i < data.Length); j++) {
                    c._key[j] = (char)data[i++];
                }
                if (i < data.Length) {
                    c.wifi_mode = (eWifiMode)data[i];
                }
            }
            return c;
        }

        public override string ToString() {
            string retval = $"ip {ip}\n";
            retval += $"contrast = {contrast}\n";
            retval += $"nightContrast = {nightContrast}\n";
            return retval;
        }
    };

    public class MiniListener : IDisposable {
        const int port = 6051;
        bool proces_running = false;
        ThreadStart _ts;
        Thread _t;
        Action<sConfig> _callback_mini_received;
        UdpClient myUdpClient;

        private void AnnounceRecUdpProces() {
            try {
                myUdpClient = new UdpClient(port);
                IPEndPoint myEndPoint = new IPEndPoint(IPAddress.Any, port);
                while (proces_running) {
                    byte[] data = myUdpClient.Receive(ref myEndPoint);
                    if (_callback_mini_received != null)
                        _callback_mini_received(sConfig.GetFromB64code(data, myEndPoint.Address.ToString()));
                }
            } catch {
                Console.WriteLine("Exception while UdpReceivement");
            }
        }

        public MiniListener(Action<sConfig> CallbackMiniRecevied) {
            _callback_mini_received = CallbackMiniRecevied;
            _ts = new ThreadStart(AnnounceRecUdpProces);
            _t = new Thread(_ts);
            proces_running = true;
            _t.Start();
        }

        public void Dispose() {
            proces_running = false;
            if (_t != null) {
                if (_t.IsAlive) {
                    _t.Abort();
                }
                _t = null;
            }
        }
    }


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
        private string wificonf_tag = "wico";
        private string begin_of_transmition = "<data>";
        private string end_of_transmition = "</data>";

        public sConfig config = new sConfig("");

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
                            byte[] data = new byte[bot.Length + array.Length + eot.Length];
                            bot.CopyTo(data, 0);
                            array.CopyTo(data, bot.Length);
                            eot.CopyTo(data, bot.Length + array.Length);
                            netStream.Write(data, 0, data.Length);
                            System.Threading.Thread.Sleep(200);
                            netStream.Write(data, 0, data.Length);
                        }
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    } finally {
                        CloseCommandConn();
                        System.Threading.Thread.Sleep(100);
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
                                byte[] data = new byte[bot.Length + array.Length + eot.Length];
                                bot.CopyTo(data, 0);
                                array.CopyTo(data, bot.Length);
                                eot.CopyTo(data, bot.Length + array.Length);
                                netStream.Write(data, 0, data.Length);
                                System.Threading.Thread.Sleep(200);
                            }
                        }
                    } catch (Exception e) { Console.WriteLine(e.Message); } finally {
                        CloseCommandConn();
                        System.Threading.Thread.Sleep(100);
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

        public async Task SendWifiConfAsync(string s) {
            if (s.Contains(":")) {
                s = s.Replace(@"\!", "!");
                await SendStringAsync(create_request(wificonf_tag, s));
            }
        }

        public async Task SendWifiConfAsync(string ssid, string pwd, eWifiMode wm) {
            if ((ssid != null) & (pwd != null)) {
                string s = $"{ssid}:{pwd}:{wm.ToString()}";
                await SendStringAsync(create_request(wificonf_tag, s));
            }
        }

        public async Task SendMessageAsync(MessageRgb msg) {
            if (msg != null) {
                string mesedz = convertMessageRgbToString(msg);
                string request = create_request(mesg_tag, mesedz);
                await SendStringAsync(request);
            }
        }

        string convertMessageRgbToString(MessageRgb msg) {
            //{"message":"9","color":0,"showtime":1,"bell":false}
            string bellBulVal = msg.bell ? "true" : "false";

            string result = "{";
            result += $"\"{nameof(msg.message)}\" : \"{msg.message}\" ,";
            result += $"\"{nameof(msg.color)}\" : {msg.color} ,";
            result += $"\"{nameof(msg.showtime)}\" : {msg.showtime} ,";
            result += $"\"{nameof(msg.bell)}\" : {bellBulVal}";

            result += "}";

            return result;
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
