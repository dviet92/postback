using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sshsv.Models
{
    public class conversionglobal
    {
        public static List<conversiontotal> listconversion = new List<conversiontotal>();
        public static int totalconversion = 0;
        public static DateTime LastUpdate = DateTime.Now;
    }
    public class conversiontotal
    {
       
        public string server { get; set; }
        public string offerID { get; set; }
        public string network { get; set; }
        public int conversion_count { get; set; }
        public int conversion_pb_count { get; set; }
        public string Country { get; set; }
        public DateTime lastconversion { get; set; }
        public string offerName { get; set; }
        public string totalClick { get; set; }
        public string unusedClick { get; set; }
        public string urlid { get; set; }
        public string urlpath { get; set; }

    }
    public class offerlistdetail
    {
        public string offerID { get; set; }
        public string network { get; set; }
        public int conversion_cnt { get; set; }
        public DateTime datetime { get; set; }

    }
}