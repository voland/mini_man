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
    public class Pres : IDisposable {

        private IEnumerable<Page> records;
        public List<Page> pages = new List<Page>();
        StreamReader sr = null;
        CsvReader csv = null;
        FileStream fs = null;
        private bool disposedValue;

        private void BackConstr(Stream StrCsv) {
            sr = new StreamReader(StrCsv);
            csv = new CsvReader(sr, CultureInfo.InvariantCulture);
            records = csv.GetRecords<Page>();
            if (records != null) {
                int i = 0;
                foreach (Page p in records) {
                    if (p != null) {
                        Console.Write("record {0}: ", i);
                        Console.WriteLine(p.ToString());
                        pages.Add(p.Clone());
                        i++;
                        if (i == Consts.MAXPAGES) break;
                    }
                }
                for (; i < Consts.MAXPAGES; i++) {
                    Page p = new Page();
                    p.idx = i.ToString();
                    p.Linia1 = "";
                    p.Linia2 = "";
                    p.Linia3 = "";
                    pages.Add(p);
                    Console.WriteLine("{0} added empty page.", i);
                }
            }
        }

        public Pres(string FileName) {
            fs = new FileStream(FileName, FileMode.Open);
            BackConstr(fs);
        }

        public Pres(Stream StrCsv) {
            BackConstr(StrCsv);
        }

        public UInt32 GetChecksum() {
            UInt32 ch = 0;
            ch += Consts.checksum(Consts.MAXPAGES);
            foreach (Page p in pages) ch += p.GetChecksum();
            return ch;
        }

        public int GetSize() {
            return 8 + Consts.MAXPAGES * Page.GetSize();
        }

        private Encoding _enc = null;
        private Encoding e {
            get {
                if (_enc == null) {
                    _enc = CodePagesEncodingProvider.Instance.GetEncoding(28592);
                }
                return _enc;
            }
        }

        public void Serialise(BinaryWriter bw) {
            string begin = string.Format("<sPresentation Size=\"{0}\" Checksum=\"{1}\">", GetSize(), GetChecksum());
            string end = "</sPresentation>";
            byte[] begin_bytes = e.GetBytes(begin);
            byte[] end_bytes = e.GetBytes(end);
            bw.Write(begin_bytes);
            bw.Write(Consts.MAXPAGES);
            if (bw != null) {
                foreach (Page p in pages) {
                    p.Serialise(bw);
                }
            }
            bw.Write((int)0);
            bw.Write(end_bytes);
            // If you want check checksum comment headers and uncoment code below
            /* bw.BaseStream.Seek(0, SeekOrigin.Begin); */
            /* UInt32 tc = 0; */
            /* int b; */
            /* while ((b = bw.BaseStream.ReadByte()) != -1) { */
            /*     tc += (UInt32)b; */
            /* } */
            /* Console.WriteLine("calclulated checksum is {0}", tc); */
        }

        public void SaveToBinFile(string filename) {
            using (FileStream fs = new FileStream(filename, FileMode.Create)) {
                using (BinaryWriter bw = new BinaryWriter(fs)) {
                    Serialise(bw);
                }
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (csv != null) csv.Dispose();
                    if (sr != null) sr.Dispose();
                    if (fs != null) fs.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
