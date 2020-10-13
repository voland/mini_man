using System;
using System.Text;

namespace mini_lib {
    public class Enc {
        static public Encoding _enc = null;
        static public Encoding e {
            get {
                if (_enc == null) {
                    _enc = CodePagesEncodingProvider.Instance.GetEncoding(28592);
                    if (_enc == null)
                        throw new Exception("Can not get encoding try to set \"e\" value externaly like Enc.e = Encoding.GetEncoding(28592);");
                }
                return _enc;
            }
            set {
                _enc = value;
            }
        }
    }
}
