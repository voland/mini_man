using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
/* using System.Collections.Generic; */
/* using CsvHelper; */
/* using CsvHelper.Configuration.Attributes; */
/* using System.Globalization; */
/* using System.Text; */
/* using System.Net; */

namespace mini_lib {
    public class Mini {

        private string contrast_tag = "contrast";
        private string nightcontrast_tag = "nightcontrast";
        private string time_tag = "time";
        private string standby_tag = "standby";
        private string pin_tag = "pin";
        private string net_tag = "net";
        private string page_tag = "page";

        private string create_request(string tag, string content) {
            return string.Format("<{0}>{1}</{0}>", tag, content);
        }

        private string ip;

        public Mini() {
            ip = "192.168.8.1";
        }

        public Mini(string ip) {
            this.ip = ip;
        }

        TcpClient client = null;

        public static object TransferLocker = new object();

        private static int cmdport = 2323;

        private void SendData() {
            try {
                lock (TransferLocker) {
                    client = new TcpClient(ip, cmdport);
                    Stream netStream = client.GetStream();
                    using (BinaryWriter sw = new BinaryWriter(netStream)) {

                    }
                }
            } catch { } finally {
                CloseCommandConn();
            }
        }

        private void CloseCommandConn() {
            if (client != null) {
                client.Close();
                client = null;
            }
        }

        private async Task SendStringAsync(string request) {
            await Task.Run(() => {
                lock (TransferLocker) {
                    try {
                        client = new TcpClient(ip, cmdport);
                        Stream netStream = client.GetStream();
                        using (BinaryWriter sw = new BinaryWriter(netStream)) {
                            sw.Write(request);
                        }
                    } catch { } finally {
                        CloseCommandConn();
                    }
                    System.Threading.Thread.Sleep(500);
                }
            });
        }

        public async Task SendPresAsync(Pres pres) {
            await Task.Run(() => {
                lock (TransferLocker) {
                    try {
                        client = new TcpClient(ip, cmdport);
                        Stream netStream = client.GetStream();
                        using (BinaryWriter sw = new BinaryWriter(netStream)) {
                            pres.Serialise(sw);
                        }
                    } catch { } finally {
                        CloseCommandConn();
                    }
                    System.Threading.Thread.Sleep(500);
                }
            });
        }

        public async Task SendContrastAsync(int k) {
            await SendStringAsync(create_request(contrast_tag, k.ToString()));
        }

        public async Task SendNightContrastAsync(int k) {
            await SendStringAsync(create_request(nightcontrast_tag, k.ToString()));
        }

        public async Task SendStandbyTimeAsync(int disH, int disM, int enaH, int enaM) {
            await SendStringAsync(create_request(standby_tag, string.Format("{0};{1};{2};{3}", disH, disM, enaH, enaM)));
        }

        public async Task SendPinAsync(string pin) {
            if (pin.Length == 4) {
                await SendStringAsync(create_request(pin_tag, pin));
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
