using System;
using System.IO;
using System.Net.Sockets;
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

        private void SendString(string request) {
            try {
                lock (TransferLocker) {
                    client = new TcpClient(ip, cmdport);
                    Stream netStream = client.GetStream();
                    using (BinaryWriter sw = new BinaryWriter(netStream)) {
                        sw.Write(request);
                    }
                }
            } catch { } finally {
                CloseCommandConn();
            }
        }

        public void SendPres(Pres pres) {
            try {
                lock (TransferLocker) {
                    client = new TcpClient(ip, cmdport);
                    Stream netStream = client.GetStream();
                    using (BinaryWriter sw = new BinaryWriter(netStream)) {
                        pres.Serialise(sw);
                    }
                }
            } catch { } finally {
                CloseCommandConn();
            }
        }

        public void SendContrast(int k) {
            SendString(create_request(contrast_tag, k.ToString()));
        }

        public void SendNightContrast(int k) {
            SendString(create_request(nightcontrast_tag, k.ToString()));
        }

        public void SendStandbyTime(int disH, int disM, int enaH, int enaM) {
            SendString(create_request(nightcontrast_tag, string.Format("{0};{1};{2};{3}", disH, disM, enaH, enaM)));
        }

        public void SendPin(string pin) {
            if (pin.Length == 4) {
                SendString(create_request(pin_tag, pin));
            }
        }

        public void SendTime() {
            int year, month, dom, hour, min, sec;
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            dom = DateTime.Now.Day;
            hour = DateTime.Now.Hour;
            min = DateTime.Now.Minute;
            sec = DateTime.Now.Second;
            string time_representation = string.Format("{0};{1};{2};{3};{4};{5}", year, month, dom, hour, min, sec);
            SendString(create_request(time_tag, time_representation));
        }

        public void SendNetConf(string ip, string ma, string gw) {
            string net_conf_representation = string.Format("{0};{1};{2}", ip, ma, gw);
            SendString(create_request(net_tag, net_conf_representation));
        }
    }
}
