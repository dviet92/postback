using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sshsv.Models;
using System.Net;
using System.IO;
using sshsv.Filters;
using MaxMind.GeoIP2;
using System.Text.RegularExpressions;
using MaxMind.GeoIP2.Responses;
using System.Web.Hosting;
using System.Data.Entity;

namespace sshsv.Controllers
{
    public class postbackController : Controller
    {

        private DatabaseReader reader = new DatabaseReader(HostingEnvironment.MapPath("/App_Data/GeoLite2-City.mmdb"));

        private sshserver3Entities db = new sshserver3Entities();
        private WebClient webclient = new WebClient();

        public void aff_lsr( string oid, string aff_source, string subid,string aff_subid1, string aff_subid2, string aff_subid3, string session_ip, string payout, string device_brand, string device_model, string device_os)
        {
            if (aff_subid1 == null) aff_subid1 = subid;
            if (AndroidPostBack(subid)) return;       
        }
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
                if (result != "OK")
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
        public void cv(string nid, string oid,string offerID, string aff_source, string subid,string aff_subid1, string aff_subid2, string aff_subid3, string session_ip, string payout, string device_brand, string device_model, string device_os)
        {
            if (aff_subid1 == null) aff_subid1 = subid;
            if (AndroidPostBack(aff_subid1)) return;
       
        }
        // GET: postback
        [AuthorizeIPAddress]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeIPAddress]
        // GET: postback/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [AuthorizeIPAddress]
        // GET: postback/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: postback/Create
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
        // GET: postback/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: postback/Edit/5
        [HttpPost]
        [AuthorizeIPAddress]
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

        // GET: postback/Delete/5
        [AuthorizeIPAddress]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: postback/Delete/5
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
