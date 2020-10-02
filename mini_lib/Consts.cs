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
    public class Consts {
        const string EFFECT_NAME0 = "Żaden";
        const string EFFECT_NAME1 = "Przewijanie w lewo";
        const string EFFECT_NAME2 = "Przewijanie od góry";
        const string EFFECT_NAME3 = "Kropki";
        const string EFFECT_NAME4 = "Kołowe";
        const string EFFECT_NAME5 = "Kontrast";
        const string EFFECT_NAME6 = "Rotacja";
        const string EFFECT_NAME7 = "Zciskanie";
        const string EFFECT_NAME8 = "Eksplozja";
        const string EFFECT_NAME9 = "Inwersja";
        const string EFFECT_NAME10 = "Paski z prawej";
        const string EFFECT_NAME11 = "Paski z góry";
        const string EFFECT_NAME12 = "Matrix";
        const string EFFECT_NAME13 = "Lawa";
        const string EFFECT_NAME14 = "Fala";
        const string FONTNAME1 = "Arial";
        const string FONTNAME2 = "Courier New";
        const string FONTNAME3 = "Arial Black";
        const string FONTNAME4 = "Comic Sans MS";
        const string FONTNAME5 = "Impact";
        const string FONTNAME6 = "Times New Roman";
        const string FONTNAME7 = "Ubuntu Condensed";
        const string FONTNAME8 = "Chancery";
        const int FONTNORMAL = 0;
        const int FONTFAT = 1;
        const int FONT1 = 0x10;
        const int FONT2 = 0x20;
        const int FONT3 = 0x30;
        const int FONT4 = 0x40;
        const int FONT5 = 0x50;
        const int FONT6 = 0x60;
        const int FONT7 = 0x70;
        const int FONT8 = 0x80;
        static int[] fonts_tab = new int[] { FONT1, FONT2, FONT3, FONT4, FONT5, FONT6, FONT7, FONT8 };
        public const int SINLE_LINE = 0;
        public const int DOUBLE_LINE = 1;
        public const int TRIPLE_LINE = 2;
        public const int DOUBLE_LINE_HORIZONTAL = 3;
        public const int MAXPAGES = 80;
        public const int MAXCHARS = 160;
        public const int MAX_LINE_MODES = 4;
        public const int MAXCHARSDOUBLE = (MAXCHARS / 2);
        public const int MAXCHARSDOUBLEHOR_FB = 16;
        public const int MAXCHARSDOUBLEHOR_2B = (MAXCHARS - MAXCHARSDOUBLEHOR_FB);
        public const int MAXCHARSTRIPLE_FB = 8;
        public const int MAXCHARSTRIPLE_23B = ((MAXCHARS - MAXCHARSTRIPLE_FB) / 2);
        public const int MAXSPEED = 5;
        public const int MAXTIME = 20;

        static public int line1len(int LinesCount) {
            switch (LinesCount) {
                case SINLE_LINE: return MAXCHARS;
                case DOUBLE_LINE: return MAXCHARSDOUBLE;
                case DOUBLE_LINE_HORIZONTAL: return MAXCHARSDOUBLEHOR_FB;
                case TRIPLE_LINE: return MAXCHARSTRIPLE_FB;
                default: return 0;
            }
        }

        static public int line2len(int LinesCount) {
            switch (LinesCount) {
                case SINLE_LINE: return 0;
                case DOUBLE_LINE: return MAXCHARSDOUBLE;
                case DOUBLE_LINE_HORIZONTAL: return MAXCHARSDOUBLEHOR_2B;
                case TRIPLE_LINE: return MAXCHARSTRIPLE_23B;
                default: return 0;
            }
        }

        static public int line3len(int LinesCount) {
            switch (LinesCount) {
                case SINLE_LINE: return 0;
                case DOUBLE_LINE: return 0;
                case DOUBLE_LINE_HORIZONTAL: return 0;
                case TRIPLE_LINE: return MAXCHARSTRIPLE_23B;
                default: return 0;
            }
        }

        static public int GetFontCode(int LinesCount, int f1, int f2, int f3) {
            switch (LinesCount) {
                case SINLE_LINE:
                    f1 %= 8;
                    return Consts.fonts_tab[f1];
                case DOUBLE_LINE:
                    f1--;
                    f2--;
                    f1 %= 1;
                    f2 %= 1;
                    return (f2 << 16) + f1;
                case DOUBLE_LINE_HORIZONTAL:
                    f1 %= 8;
                    f2 %= 8;
                    return (Consts.fonts_tab[f1] << 16) + Consts.fonts_tab[f2];
                case TRIPLE_LINE:
                    f1 %= 8;
                    f2--;
                    f3--;
                    f2 %= 1;
                    f3 %= 1;
                    return Consts.fonts_tab[f1] + (f3 << 1) + f2;
                default: return Consts.FONT1;
            }
        }

        static public UInt32 checksum(int input) {
            UInt32 ii = (UInt32)input;
            UInt32 ch = 0;
            for (int i = 0; i < 32; i += 8) ch += (ii >> i) & 0xff;
            return ch;
        }

        static public UInt32 checksum(byte[] input) {
            UInt32 ch = 0;
            foreach (byte b in input) ch += (UInt32)b;
            return ch;
        }

    }
}
