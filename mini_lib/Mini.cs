using System;
using System.IO;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace mini_lib {
    public class Mini {

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
        }

        public void SendNightContrast(int k) {
        }

        public void SendStandbyTime(int disH, int disM, int enaH, int enaM) {
        }

        public void SendPin(string pin) {
            if (pin.Length == 4) {
            }
        }

        public void SendTime() {
        }
    }
}
