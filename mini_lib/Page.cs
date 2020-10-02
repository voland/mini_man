
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
    public class Page {
        [Index(0)]
        public string idx { get; set; }
        [Index(1)]
        public string czas_trwania { get; set; }
        [Index(2)]
        public string Efekt { get; set; }
        [Index(3)]
        public string liczba_linii { get; set; }
        [Index(4)]
        public string Linia1 { get; set; }
        [Index(5)]
        public string Czcionka1 { get; set; }
        [Index(6)]
        public string Linia2 { get; set; }
        [Index(7)]
        public string Czcionka2 { get; set; }
        [Index(8)]
        public string Linia3 { get; set; }
        [Index(9)]
        public string Czcionka3 { get; set; }

        public Page Clone() {
            Page p = new Page();
            p.idx = this.idx;
            p.czas_trwania = this.czas_trwania;
            p.Efekt = this.Efekt;
            p.liczba_linii = this.liczba_linii;
            p.Linia1 = this.Linia1;
            p.Czcionka1 = this.Czcionka1;
            p.Linia2 = this.Linia2;
            p.Czcionka2 = this.Czcionka2;
            p.Linia3 = this.Linia3;
            p.Czcionka3 = this.Czcionka3;
            return p;
        }

        private int _idx {
            get {
                int retval;
                if (Int32.TryParse(idx, out retval)) return retval;
                return 0;
            }
        }

        private int _czas_trwania {
            get {
                int retval;
                if (Int32.TryParse(czas_trwania, out retval)) {
                    if (retval > Consts.MAXTIME) retval = Consts.MAXTIME;
                    return retval;
                }
                return 0;
            }
        }

        private int _Efekt {
            get {
                int retval;
                if (Int32.TryParse(Efekt, out retval)) return retval;
                return 0;
            }
        }

        private int _liczba_linii {
            get {
                int retval;
                if (Int32.TryParse(liczba_linii, out retval)) return retval - 1;
                return 0;
            }
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

        private int _Czcionka1 {
            get {
                int retval;
                if (Int32.TryParse(Czcionka1, out retval)) return retval - 1;
                return 0;
            }
        }

        private int _Czcionka2 {
            get {
                int retval;
                if (Int32.TryParse(Czcionka2, out retval)) return retval - 1;
                return 0;
            }
        }

        private int _Czcionka3 {
            get {
                int retval;
                if (Int32.TryParse(Czcionka3, out retval)) return retval - 1;
                return 0;
            }
        }

        private byte[] l1 = null;
        private byte[] _Linia1 {
            get {
                if (l1 == null) {
                    int lc = Consts.line1len(_liczba_linii);
                    l1 = new byte[lc];
                    if (Linia1 != null) {
                        byte[] tab = e.GetBytes(Linia1);
                        for (int i = 0; i < Math.Min(tab.Length, lc); i++) l1[i] = tab[i];
                    }
                }
                return l1;
            }
        }

        private byte[] l2 = null;
        private byte[] _Linia2 {
            get {
                if (l2 == null) {
                    int lc = Consts.line2len(_liczba_linii);
                    l2 = new byte[lc];
                    if (Linia2 != null) {
                        byte[] tab = e.GetBytes(Linia2);
                        for (int i = 0; i < Math.Min(tab.Length, lc); i++) l2[i] = tab[i];
                    }
                }
                return l2;
            }
        }

        private byte[] l3 = null;
        private byte[] _Linia3 {
            get {
                if (l3 == null) {
                    int lc = Consts.line3len(_liczba_linii);
                    l3 = new byte[lc];
                    if (Linia3 != null) {
                        byte[] tab = e.GetBytes(Linia3);
                        for (int i = 0; i < Math.Min(tab.Length, lc); i++) l3[i] = tab[i];
                    }
                }
                return l3;
            }
        }

        public UInt32 GetChecksum() {
            UInt32 ch = 0;
            ch += Consts.checksum(_liczba_linii);
            ch += Consts.checksum(_Efekt);
            ch += Consts.checksum(_czas_trwania);
            if (_Linia1 != null) ch += Consts.checksum(_Linia1);
            if (_Linia2 != null) ch += Consts.checksum(_Linia2);
            if (_Linia3 != null) ch += Consts.checksum(_Linia3);
            int font_code = Consts.GetFontCode(_liczba_linii, _Czcionka1, _Czcionka2, _Czcionka3);
            ch += Consts.checksum(font_code);
            return ch;
        }

        static public int GetSize() {
            return 16 + Consts.MAXCHARS;
        }

        //returns checksum
        public void Serialise(BinaryWriter bw) {
            if (bw != null) {
                bw.Write(_liczba_linii);
                bw.Write(_Efekt);
                bw.Write(_czas_trwania);
                bw.Write(_Linia1);
                switch (_liczba_linii) {
                    case Consts.DOUBLE_LINE_HORIZONTAL:
                    case Consts.DOUBLE_LINE:
                        bw.Write(_Linia2);
                        break;
                    case Consts.TRIPLE_LINE:
                        bw.Write(_Linia2);
                        bw.Write(_Linia3);
                        break;
                }
                /* Console.WriteLine("total len is {0}, {1}, {2}, {3}", _Linia1.Length, _Linia2.Length, +_Linia3.Length, _Linia1.Length + _Linia2.Length + _Linia3.Length); */
                int font_code = Consts.GetFontCode(_liczba_linii, _Czcionka1, _Czcionka2, _Czcionka3);
                bw.Write(font_code);
            }
        }

        public override string ToString() {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {6}, {7}, {8}, {9}", idx, czas_trwania, Efekt, liczba_linii, Linia1, Czcionka1, Linia1, Czcionka2, Linia2, Czcionka3, Linia3);
        }
    }
}
