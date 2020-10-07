using System.Text;

namespace mini_lib {
    public class Enc {

        static private Encoding _enc = null;
        static private Encoding e {
            get {
                if (_enc == null) {
                    _enc = CodePagesEncodingProvider.Instance.GetEncoding(28592);
                }
                return _enc;
            }
        }

    }
}
