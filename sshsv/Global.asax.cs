using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using sshsv.Models;
using sshserver3;
using sshserver3.Models;
using System.Net;
using Amib.Threading;
using System.Text;
using MaxMind.GeoIP2;
using System.Web.Hosting;
using System.Timers;
using Newtonsoft.Json;
using System.Data.Entity;

namespace sshsv
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static DatabaseReader reader = new DatabaseReader(HostingEnvironment.ApplicationPhysicalPath + @"\App_Data\GeoLite2-City.mmdb");
        public static List<IPAddressRange> listrange = new List<IPAddressRange>();
        public System.Timers.Timer timer1 = new System.Timers.Timer(30*60*1000);
        public System.Timers.Timer timer = new System.Timers.Timer( 60 * 1000);
        public System.Timers.Timer timerFirePostBack = new System.Timers.Timer(60*10 * 1000);
        public WebClient webclient = new WebClient();

        protected void Application_Start()
        {
            string geodata = sshsv.Resource.geo;
            string[] listrangecountry = geodata.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < listrangecountry.Count(); i++)
            {
                string[] rangedetail = listrangecountry[i].Split(new string[] { "|" }, StringSplitOptions.None);
                if (rangedetail.Count() == 3)
                {
                    IPAddressRange iprange = new IPAddressRange(IPAddress.Parse(rangedetail[0]), IPAddress.Parse(rangedetail[1]), rangedetail[2]);
                    listrange.Add(iprange);
                }
            }
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            timerFirePostBack.Elapsed += TimerFirePostBack_Elapsed;
            timerFirePostBack.Enabled = true;
        }

        private void TimerFirePostBack_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerFirePostBack.Stop();
            List<ClickIDQueue> listRemove = new List<ClickIDQueue>();
            lock(ClickIDQueue.clickQueue)
            { 
            foreach (var item in ClickIDQueue.clickQueue)
            {
                string ret = "";
                try
                {
                    string server = item.isAndroid ? "android" : "ios";
                    ret = webclient.DownloadString($"http://boot{server}.devmoba.com/Home/postback?clickID=" + item.ClickID + "&transactionIP=" + item.hostIP);
                }
                catch (Exception)
                {

                  
                }
                if (ret == "OK") listRemove.Add(item);
            }
            }
            listRemove.ForEach(x => ClickIDQueue.clickQueue.Remove(x));
            timerFirePostBack.Start();
        }

 
    }
}
