using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sshsv
{
    public class ClickIDQueue
    {
        public string ClickID { get; set; }
        public string hostIP { get; set; }
        public bool isAndroid { get; set; }
        public ClickIDQueue(string ClickID,string hostIP,bool isAndroid)
        {
            this.ClickID = ClickID;
            this.hostIP = hostIP;
            this.isAndroid = isAndroid;
        }
        public static List<ClickIDQueue> clickQueue = new List<ClickIDQueue>();

    }
}