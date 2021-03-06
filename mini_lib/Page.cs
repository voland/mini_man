
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
    public class Record {
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

        public override string ToString() {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {6}, {7}, {8}, {9}", idx, czas_trwania, Efekt, liczba_linii, Linia1, Czcionka1, Linia1, Czcionka2, Linia2, Czcionka3, Linia3);
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

        //w tej konwersjii przyjmujemy ze rekord lini3 w triple line i double line hor przechodzi do lini 1 w sterowniku
        public Page GetPage() {
            Page p = new Page();
            int temp;
            if (Int32.TryParse(idx, out temp)) p.idx = temp; else p.idx = 0;
            p.czas_trwania = 0;
            if (Int32.TryParse(czas_trwania, out temp)) {
                if (temp < Consts.MAXTIME)
                    p.czas_trwania = temp;
            }
            p.Efekt = 0;
            if (Int32.TryParse(Efekt, out temp)) {
                if (temp < Consts.EFFECTCNT) {
                    p.Efekt = temp;
                }
            }
            p.liczba_linii = Consts.SINLE_LINE;
            if (Int32.TryParse(liczba_linii, out temp)) {
                if (temp <= 4) {
                    p.liczba_linii = temp - 1;
                }
            }
            p.font = Consts.GetFontCode(p.liczba_linii, _Czcionka1, _Czcionka2, _Czcionka3);
            if (p.text == null) p.text = new byte[Consts.MAXCHARS];
            //clear buffer
            for (int k = 0; k < p.text.Length; k++) {
                p.text[k] = 0;
            }
            byte[] tab;
            int i = 0;
            switch (p.liczba_linii) {
                case Consts.SINLE_LINE:
                    tab = Enc.e.GetBytes(Linia1);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 1));) p.text[i++] = tab[j++];
                    break;
                case Consts.DOUBLE_LINE:
                    tab = Enc.e.GetBytes(Linia1);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 2));) p.text[i++] = tab[j++];
                    i++;
                    tab = Enc.e.GetBytes(Linia2);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 1));) p.text[i++] = tab[j++];
                    break;
                case Consts.DOUBLE_LINE_HORIZONTAL:
                    tab = Enc.e.GetBytes(Linia3);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 2));) p.text[i++] = tab[j++];
                    i++;
                    tab = Enc.e.GetBytes(Linia1);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 1));) p.text[i++] = tab[j++];
                    break;
                case Consts.TRIPLE_LINE:
                    tab = Enc.e.GetBytes(Linia3);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 3));) p.text[i++] = tab[j++];
                    i++;
                    tab = Enc.e.GetBytes(Linia1);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 2));) p.text[i++] = tab[j++];
                    i++;
                    tab = Enc.e.GetBytes(Linia2);
                    for (int j = 0; j < (tab.Length) & (i < (Consts.MAXCHARS - 1));) p.text[i++] = tab[j++];
                    break;
            }
            return p;
        }
    }

    public class Page {
        public int idx { get; set; }
        public int czas_trwania { get; set; }
        public int Efekt { get; set; }
        public int liczba_linii { get; set; }
        public byte[] text { get; set; }
        public int font { get; set; }

        public UInt32 GetChecksum() {
            UInt32 ch = 0;
            ch += Consts.checksum(liczba_linii);
            ch += Consts.checksum(Efekt);
            ch += Consts.checksum(czas_trwania);
            if (text != null) ch += Consts.checksum(text);
            ch += Consts.checksum(font);
            return ch;
        }

        static public int GetSize() {
            return 16 + Consts.MAXCHARS;
        }

        //returns checksum
        public void Serialise(BinaryWriter bw) {
            if (bw != null) {
                bw.Write(liczba_linii);
                bw.Write(Efekt);
                bw.Write(czas_trwania);
                bw.Write(text);
                bw.Write(font);
            }
        }
    }
}
