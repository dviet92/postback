using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sshsv.Models;
using System.Net;
using System.IO;
using System.Data.Entity;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using sshsv.Filters;
using System.Text.RegularExpressions;
using MaxMind.GeoIP2.Responses;
using MaxMind.GeoIP2;
using System.Web.Hosting;

namespace sshsv.Controllers
{

    public class ConversionController : Controller
    {
        // GET: Conversion
        private sshserver3Entities db = new sshserver3Entities();
        private urlDB1Entities dburl1 = new urlDB1Entities();
        private urlDB1Entities1 dburl2 = new urlDB1Entities1();
        private urlDB1Entities2 dburl3 = new urlDB1Entities2();
        private urlDBEntities dburl4 = new urlDBEntities();
        private WebClient webclient = new WebClient();

        private static DatabaseReader reader = new DatabaseReader(HostingEnvironment.MapPath("/App_Data/GeoLite2-City.mmdb"));
        private bool AndroidPostBack(string clickID)
        {
            if (clickID == null) return false;
            if (clickID.ToLower().StartsWith("android")|| clickID.ToLower().StartsWith("iossss"))
            {
                string result = "";
                try
                {
                    string server = clickID.ToLower().StartsWith("android") ? "android" : "ios";
                    result = webclient.DownloadString($"http://boot{server}.devmoba.com/Home/postback?clickID=" + clickID + "&transactionIP=" + Request.UserHostAddress);
                }
                catch (Exception)
                {

                    
                }
                if (result!="OK")
                {
                    lock (ClickIDQueue.clickQueue)
                    { 
                        ClickIDQueue.clickQueue.Add(new ClickIDQueue(clickID, Request.UserHostAddress, clickID.ToLower().StartsWith("android")));
                    }
                }
                return true;
            }
            return false;
        }
        public void postback4(string networkID, string offerID, string aff_source, string aff_subid1, string aff_subid2, string aff_subid3, string session_ip, string payout, string device_brand, string device_model, string device_os)
        {
            if (AndroidPostBack(aff_subid1)) return;          
        }
        public void postback3(string networkID, string offerID, string aff_source, string aff_subid1, string aff_subid2, string aff_subid3, string session_ip, string payout, string device_brand, string device_model, string device_os)
        {
            if (AndroidPostBack(aff_subid1)) return;             
        }
        public void postback2(string networkID, string offerID, string aff_source, string aff_subid1, string aff_subid2, string aff_subid3, string session_ip, string payout, string device_brand, string device_model, string device_os)
        {
            if (AndroidPostBack(aff_subid1)) return;
          
       
        }
        public void postback1(string networkID, string offerID, string aff_source, string aff_subid1, string aff_subid2, string aff_subid3, string session_ip, string payout, string device_brand, string device_model, string device_os)
        {
            if (AndroidPostBack(aff_subid1)) return;                    
        }
        public void postback(string networkID, string offerID, string aff_source, string aff_subid1, string aff_subid2, string aff_subid3, string session_ip, string payout, string device_brand, string device_model, string device_os)
        {
            if (AndroidPostBack(aff_subid1)) return;            
        }
        private string TimeAgo(DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }
        [AuthorizeIPAddress]
        public ActionResult Index()
        {

            ViewBag.TotalCons = conversionglobal.totalconversion.ToString();
            ViewBag.LastUpdate = TimeAgo(conversionglobal.LastUpdate);
     
 

            /*
            foreach (var _network in db.networks)
            {
                var conversions = conversiontotal.Where(x => x.networkID == _network.ID);
                if (conversions.Count() > 0)
                {
                    var offerlist = conversions.Select(x => x.offerID).Distinct();
                    foreach (var item in offerlist)
                    {

                        conversiontotal _conversion = new conversiontotal();
                        _conversion.offerID = item;
                        _conversion.network = _network.networkName;
                        var lastconversion = conversiontotal.Where(x => x.offerID == item).OrderByDescending(x => x.time_install).First();
                        _conversion.lastconversion = (DateTime)lastconversion.time_install;

                        _conversion.conversion_count = conversions.Count(y => y.offerID == item);
                        _conversion.conversion_pb_count = conversions.Count(y => y.offerID == item && (DateTime.Now.Hour == ((DateTime)y.time_install).Hour));
                        _conversion.Country = "";

                        if (_conversion.offerID == null || _conversion.offerID == "") _conversion.offerID = "UNKNOWN";
                        if (_conversion.network == null || _conversion.network == "") _conversion.network = "UNKNOWN";
                        var lasttr = lastconversion.transactionID.Split(new string[] { "_" }, StringSplitOptions.None);
                        if (lasttr.Count()>=5)
                        {
                            switch (lasttr[3])
                            {
                                case "sv1":
                                    var _url = urlsv1.FirstOrDefault(x => x.ID == Convert.ToInt32(lasttr[4]));
                                    if (_url != null)
                                    {
                                        _conversion.urlid = "http://url.devmoba.com/url/Edit/" + _url.ID;
                                        _conversion.offerName = _url.name;
                                    }

                                    break;
                                case "sv2":
                                    var _url1 = urlsv2.FirstOrDefault(x => x.ID == Convert.ToInt32(lasttr[4]));
                                    if (_url1 != null)
                                    {
                                        _conversion.urlid = "http://url1.devmoba.com/url/Edit/" + _url1.ID;
                                        _conversion.offerName = _url1.name;
                                    }
                                    break;
                                case "sv3":
                                    var _url2 = urlsv3.FirstOrDefault(x => x.ID == Convert.ToInt32(lasttr[4]));
                                    if (_url2 != null)
                                    {
                                        _conversion.urlid = "http://url2.devmoba.com/url/Edit/" + _url2.ID;
                                        _conversion.offerName = _url2.name;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        //if (_conversion.Country == null|| _conversion.Country=="") _conversion.Country = "UNKNOWN";
                        listconversion.Add(_conversion);


                    }
                }
            }
            int totalconversion = 0;

            foreach (var _cons in listconversion)
            {
                totalconversion += _cons.conversion_count;
            }
            ViewBag.TotalCons = totalconversion.ToString();
            listconversion = listconversion.OrderByDescending(x => x.lastconversion).ToList();
            if (listconversion.Count == 0) return View(listconversion);
          
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("offerlist", String.Join("|", listconversion.Select(x => x.offerID)));

                byte[] responsebytes = wb.UploadValues("http://url.devmoba.com/Home/getOfferNameByOfferID", "POST", reqparm);
                string response = Encoding.UTF8.GetString(responsebytes);
                var offer = response.Split(new string[] { "{:}" }, StringSplitOptions.None);
                for (int i = 0; i < offer.Length; i++)
                {
                    var offerdetail = offer[i].Split(new string[] { "|" }, StringSplitOptions.None);
                    listconversion[i].offerName = offerdetail[0];
                    listconversion[i].urlid = offerdetail[1];
                    listconversion[i].totalClick = offerdetail[2];
                    listconversion[i].unusedClick = offerdetail[3];
                }
                responsebytes = wb.UploadValues("http://url1.devmoba.com/Home/getOfferNameByOfferID", "POST", reqparm);
                 response = Encoding.UTF8.GetString(responsebytes);
                 offer = response.Split(new string[] { "{:}" }, StringSplitOptions.None);
                for (int i = 0; i < offer.Length; i++)
                {
                    if (listconversion[i].offerName=="")
                    { 
                    var offerdetail = offer[i].Split(new string[] { "|" }, StringSplitOptions.None);
                    listconversion[i].offerName = offerdetail[0];
                    listconversion[i].urlid = offerdetail[1];
                    listconversion[i].totalClick = offerdetail[2];
                    listconversion[i].unusedClick = offerdetail[3];
                    }
                }
            }
            */
            return View(conversionglobal.listconversion);
        }

        [AuthorizeIPAddress]
        public ActionResult Index1()
        {
            List<conversiontotal> listconversion = new List<conversiontotal>();
            DateTime Yesterday = DateTime.Now.Date - TimeSpan.FromDays(1);
            var conversiontotal = db.conversions.Where(x => DbFunctions.DiffDays(x.time_install, DateTime.Now) == 0).ToList();
            foreach (var _network in db.networks)
            {
                var conversions = conversiontotal.Where(x => x.networkID == _network.ID);
                if (conversions.Count() > 0)
                {
                    var offerlist = conversions.Select(x => x.offerID).Distinct();
                    foreach (var item in offerlist)
                    {

                        conversiontotal _conversion = new conversiontotal();
                        _conversion.offerID = item;
                        _conversion.network = _network.networkName;
                        _conversion.lastconversion = (DateTime)db.conversions.Where(x => x.offerID == item).OrderByDescending(x => x.time_install).First().time_install;

                        _conversion.conversion_count = conversions.Count(y => y.offerID == item);
                        _conversion.conversion_pb_count = conversions.Count(y => y.offerID == item && (DateTime.Now.Hour == ((DateTime)y.time_install).Hour));
                        _conversion.Country = "";

                        if (_conversion.offerID == null || _conversion.offerID == "") _conversion.offerID = "UNKNOWN";
                        if (_conversion.network == null || _conversion.network == "") _conversion.network = "UNKNOWN";
                        //if (_conversion.Country == null|| _conversion.Country=="") _conversion.Country = "UNKNOWN";
                        listconversion.Add(_conversion);


                    }
                }
            }
            int totalconversion = 0;

            foreach (var _cons in listconversion)
            {
                totalconversion += _cons.conversion_count;
            }
            ViewBag.TotalCons = totalconversion.ToString();
            listconversion = listconversion.OrderByDescending(x => x.lastconversion).ToList();
            if (listconversion.Count == 0) return View(listconversion);

           

            return View(listconversion);
        }

        [AuthorizeIPAddress]
        public ActionResult List(string offerID, string network, string datestart, string dateend, string timezone)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            if (timezone != null) myZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            DateTime dStart = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            DateTime dEnd = DateTime.Now;
            if (datestart != null) dStart = TimeZoneInfo.ConvertTime(DateTime.ParseExact(datestart, "M-d-yyyy", provider), myZone, TimeZoneInfo.Local);
            if (dateend != null) dEnd = TimeZoneInfo.ConvertTime(DateTime.ParseExact(dateend, "M-d-yyyy", provider), myZone, TimeZoneInfo.Local);


            var conversions = db.conversions.Where(x => (offerID == null || offerID == "UNKNOWN" || x.offerID == offerID) && (network == null || network == "UNKNOWN" || x.network.networkName == network) && (datestart == null || datestart == "" || x.time_install >= dStart) && (dateend == null || dateend == "" || x.time_install <= dEnd)).ToList();
            var ListNetwork = conversions.Select(x => x.network).Distinct().ToList();
            var ListOfferID = conversions.Select(x => x.offerID).Distinct().ToList();
            var dateStartCheck = dStart;
            var dateEndCheck = dEnd;
            var offerListDetail = new List<offerlistdetail>();
            while (dateStartCheck < dateEndCheck)
            {

                foreach (var net in ListNetwork)
                {
                    foreach (var _offer in ListOfferID)
                    {
                        offerlistdetail odetal = new offerlistdetail();
                        odetal.conversion_cnt = conversions.Count(x => x.network.ID == net.ID && x.offerID == _offer && dateStartCheck <= x.time_install && x.time_install < (dateStartCheck + new TimeSpan(1, 0, 0, 0)));
                        odetal.datetime = dateStartCheck;
                        odetal.network = net.networkName;
                        odetal.offerID = _offer;
                        offerListDetail.Add(odetal);
                    }
                }
                dateStartCheck = dateStartCheck + new TimeSpan(1, 0, 0, 0);
            }
            return View(offerListDetail);
        }
        [AuthorizeIPAddress]
        public string getPrams(string param, string offerId, string network, string startDate, string endDate,string timezone)
        {
           
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            if (timezone != null) myZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            DateTime dstart = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            DateTime dend = DateTime.Now;
            if (!DateTime.TryParse(startDate, out dstart)) return "start date invalid";
            if (!DateTime.TryParse(endDate, out dend)) return "end date invalid";
           dstart = TimeZoneInfo.ConvertTime(dstart, myZone, TimeZoneInfo.Local);
            dend = TimeZoneInfo.ConvertTime(dend, myZone, TimeZoneInfo.Local);



        
            var paramlits = db.conversions.Where(x => (offerId == null || offerId == "UNKNOWN" || x.offerID == offerId) && (network == null || network == "UNKNOWN" || x.network.networkName == network) && (dend == null || x.time_install < dend) && (dstart == null || x.time_install > dstart)).ToList();
            switch (param)
            {
                case "aff_subid1":
                    return Newtonsoft.Json.JsonConvert.SerializeObject(paramlits.Select(x => x.transactionID).ToArray());
                    break;
                case "aff_subid2":
                    return Newtonsoft.Json.JsonConvert.SerializeObject(paramlits.Select(x => x.aff_subid2).ToArray());
                    
               
                case "aff_subid3":
                    return Newtonsoft.Json.JsonConvert.SerializeObject(paramlits.Select(x => x.aff_subid3).ToArray());
                case "session_ip":
                    return Newtonsoft.Json.JsonConvert.SerializeObject(paramlits.Select(x => x.session_ip).ToArray());
                
                default:
                    return "";
                    break;
            }
        }
        [AuthorizeIPAddress]
        public ActionResult getdata(string offerID,string network,string country,int day=7)
        {
            var yesterday = DateTime.Now.Date - TimeSpan.FromDays(1);
            var data = db.conversions.Where(x => (offerID == null || offerID == "UNKNOWN" || x.offerID == offerID) && (network == null || network == "UNKNOWN" || x.network.networkName == network) && (country == null || country == "UNKNOWN" || x.country == country) && DbFunctions.DiffDays(x.time_install, DateTime.Now) < day).OrderByDescending(x => x.time_install).ToList();
            
            var dd = data.Select(item => item.network.networkName+"|"+item.offerID + "|" +item.transactionID+"|"+ item.aff_source + "|" + item.aff_subid1 + "|" + item.aff_subid2 + "|" + item.aff_subid3 + "|" + item.session_ip + "|" + item.time_install + "|" + item.country);
            var datatxt = "network|offerID|transactionID|aff_source|aff_subid1|aff_subid2|aff_subid3|session_ip|time_install|country";
            datatxt += "\r\n";
            datatxt += String.Join("\r\n", dd.ToArray());
            return File(Encoding.UTF8.GetBytes(datatxt),
                        "text/plain",
                         string.Format("{0}.txt",DateTime.Now.ToString()));

        }
        public ActionResult Detail(string offerID,string network,string country,int day=2)
        {


            DateTime Yesterday = DateTime.Now.Date - TimeSpan.FromDays(1);

            return View(db.conversions.Where(x=>(offerID==null|| offerID == "UNKNOWN" || x.offerID==offerID)&&(network==null||network== "UNKNOWN"||x.network.networkName==network)&&(country==null || country == "UNKNOWN" || x.country==country)&&DbFunctions.DiffDays(x.time_install, DateTime.Now)<day).OrderByDescending(x=>x.time_install).ToList());
        }
        public ActionResult DetailByURLID(string urlid, string server,string network, string offerID, int day = 2)
        {

            var conversionstotal = db.conversions.Where(x => (offerID == null || offerID == "UNKNOWN" || x.offerID == offerID) && (network == null || network == "UNKNOWN" || x.network.networkName == network)  && DbFunctions.DiffDays(x.time_install, DateTime.Now) < day).OrderByDescending(x => x.time_install).ToList();
            foreach (var conversion in conversionstotal)
            {
                var _spl = conversion.transactionID.Split(new string[] { "_" }, StringSplitOptions.None);

                if (_spl.Count() >= 5)
                {
                    conversion.server = _spl[3];
                    conversion.urlid = _spl[4];

                }
                else
                {
                    conversion.server = "unknown";
                    conversion.urlid = "unknown";
                }
            }

            return View(conversionstotal.Where(x=>x.server==server&&x.urlid==urlid).OrderByDescending(x => x.time_install));
        }
        [AuthorizeIPAddress]
        // GET: Conversion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [AuthorizeIPAddress]
        // GET: Conversion/Create
        public ActionResult Create()
        {
            return View();
        }
        [AuthorizeIPAddress]
        // POST: Conversion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [AuthorizeIPAddress]
        // GET: Conversion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        [AuthorizeIPAddress]
        // POST: Conversion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [AuthorizeIPAddress]
        // GET: Conversion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Conversion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
