using System;
using System.Text;

namespace mini_lib {
    public class Enc {

        static public Encoding _enc = null;
        static public Encoding e {
            get {
                if (_enc == null) {
                    _enc = Encoding.GetEncoding(28592);
                    if (_enc == null)
                        _enc = CodePagesEncodingProvider.Instance.GetEncoding(28592);
                }
                return _enc;
            }
        }

    }
}
