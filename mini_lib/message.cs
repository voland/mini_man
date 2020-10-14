using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
/* using CsvHelper.Configuration.Attributes; */
/* using System.Globalization; */
/* using System.Text; */
/* using System.Net; */

namespace mini_lib {
    public class MessageRgb {
        public string message { set; get; }
        public UInt32 color { set; get; }
        public int showtime { set; get; }
        public bool bell { set; get; }

        public MessageRgb(string message, UInt32 color, int showtime, bool bell) {
            this.message = message;
            this.color = color;
            this.showtime = showtime;
            this.bell = bell;
        }

		public override string ToString(){
			return JsonSerializer.Serialize(this);
		}
    }
}
