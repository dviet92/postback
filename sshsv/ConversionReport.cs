using sshsv.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace sshsv
{
    public class ConversionReport
    {
        public static object synclock = new object();
        public static object synclock1 = new object();
        private static urlDB1Entities db1 = new urlDB1Entities();
        private static urlDB1Entities1 db2 = new urlDB1Entities1();
        public static void saveconversionreport(string server, string urlid)
        {
            return;
            string svurl = "http://url.devmoba.com";

            if (server == "sv2")
            {
                svurl = "http://url1.devmoba.com";                          
            }
            if (server == "sv3")
            {
                svurl = "http://url2.devmoba.com";
            }
            try
            {
                new WebClient().DownloadString(svurl + "//Home/trackconversion?urlid=" + urlid);
            }
            catch (Exception)
            {

               
            }
        }
    }
}