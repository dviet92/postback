using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sshsv.Models;
using System.Text;
using System.IO;
using MaxMind.GeoIP2;
using System.Web.Hosting;
using EntityFramework.BulkInsert.Extensions;
using System.Transactions;
using System.Text.RegularExpressions;
using sshsv.Filters;

namespace sshsv.Controllers
{
    [AuthorizeIPAddress]
    public class HomeController : Controller
    {

        private static DatabaseReader reader = new DatabaseReader(HostingEnvironment.ApplicationPhysicalPath + @"\App_Data\GeoLite2-City.mmdb");
        private sshserver3Entities db = new sshserver3Entities();

        public ActionResult Index()
        {
            return View(IPAddressRange.listsshes.ToList());
        }
        public string countrylist()
        {
            var listcountry = IPAddressRange.listsshes.Where(x => x.live == true).Select(x => x.country).Distinct();
            return String.Join("|", listcountry);
        }
        public ActionResult DeleteAll()
        {

            IPAddressRange.listsshes.RemoveAll(x => true);
            return RedirectToAction("Index");
        }
        public ActionResult RefreshAll()
        {
            // db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ssh] WHERE [dbo].[ssh].live=0");

            foreach (var item1 in IPAddressRange.listsshes)
            {
                item1.used = null;
            }
  
            return RedirectToAction("Index");
        }
        public ActionResult DownloadDie()
        {
            List<string> sshlistcontent = new List<string>();
            var listssh = IPAddressRange.listsshes.Where(x => x.live == false).ToArray();
            for (int i = 0; i < listssh.Count(); i++)
            {
                string content = listssh[i].ssh1;
                content += "|";
                content += listssh[i].username;
                content += "|";
                content += listssh[i].password;
                content += "|";
                content += listssh[i].country;
                content += "|";
                content += listssh[i].city;
                sshlistcontent.Add(content);
            }


            var contentType = "text/xml";
            var bytes = Encoding.UTF8.GetBytes(String.Join("\r\n", sshlistcontent.OrderBy(x => x)));
            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = sshlistcontent.Count + "ssh.txt";
            return result;
        }
        public ActionResult DownloadAll()
        {

            List<string> sshlistcontent = new List<string>();

            var listssh = IPAddressRange.listsshes.Where(x => x.live == true).ToList();

            for (int i = 0; i < listssh.Count(); i++)
            {
                string content = listssh[i].ssh1;
                content += "|";
                content += listssh[i].username;
                content += "|";
                content += listssh[i].password;
                content += "|";
                content += listssh[i].country;
                content += "|";
                content += listssh[i].city;
                sshlistcontent.Add(content);
            }


            var contentType = "text/xml";
            var bytes = Encoding.UTF8.GetBytes(String.Join("\r\n", sshlistcontent.OrderBy(x => x)));
            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = sshlistcontent.Count + "ssh.txt";
            return result;
        }
        public ActionResult Delete(string item)
        {
            item = item.Replace("'", "''");
            // db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ssh] WHERE [dbo].[ssh].country = " + "'" + item + "'");
            IPAddressRange.listsshes.RemoveAll(x => x.country == item);
            return RedirectToAction("Index");
        }
        public FileContentResult DownloadA(string item, string all)
        {
            List<string> sshlistcontent = new List<string>();
            ssh[] listssh;
            if (all == null) listssh = IPAddressRange.listsshes.Where(x => x.country.Contains(item)).ToArray();
            else listssh = IPAddressRange.listsshes.Where(x => x.country.Contains(item)).ToArray();
            for (int i = 0; i < listssh.Count(); i++)
            {

                string content = listssh[i].ssh1;
                content += "|";
                content += listssh[i].username;
                content += "|";
                content += listssh[i].password;
                content += "|";
                content += listssh[i].country;
                content += "|";
                content += listssh[i].city;
                sshlistcontent.Add(content);
            }


            var contentType = "text/xml";
            var bytes = Encoding.UTF8.GetBytes(String.Join("\r\n", sshlistcontent.OrderBy(x => x)));
            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = sshlistcontent.Count + item + ".txt";
            return result;
        }
        public FileContentResult Download(string item, string all)
        {
            List<string> sshlistcontent = new List<string>();
            ssh[] listssh;
            if (all == null) listssh = IPAddressRange.listsshes.Where(x => x.country.Contains(item) && x.live == true).ToArray();
            else listssh = IPAddressRange.listsshes.Where(x => x.country.Contains(item)).ToArray();
            for (int i = 0; i < listssh.Count(); i++)
            {

                string content = listssh[i].ssh1;
                content += "|";
                content += listssh[i].username;
                content += "|";
                content += listssh[i].password;
                content += "|";
                content += listssh[i].country;
                content += "|";
                content += listssh[i].city;
                sshlistcontent.Add(content);
            }


            var contentType = "text/xml";
            var bytes = Encoding.UTF8.GetBytes(String.Join("\r\n", sshlistcontent.OrderBy(x => x)));
            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = sshlistcontent.Count + item + ".txt";
            return result;
        }
        public void xoassh(string ID)
        {
            // var _sshdie = db.sshes.FirstOrDefault(x => x.ID.ToString() == ID);
            //  if (_sshdie!=null)
            //   {
            //  _sshdie.live = false;
            //      db.SaveChanges();
            //    }
        }
        [HttpPost]
        public void sshesdie(string sshes)
        {
            var listdie = sshes.Split(new string[] { "|" }, StringSplitOptions.None);
            foreach (var item in listdie)
            {
                var _sshdie = IPAddressRange.listsshes.FirstOrDefault(x => x.ssh1 == item);
                if (_sshdie != null) { IPAddressRange.listsshes.Remove(_sshdie); }
            }
          //  db.SaveChanges();
        }
        public string getssh(string country, string offerID, string IP, string citygeo, string fast)
        {


            if (IP != null && IP != "")
            {
                var sssh = IPAddressRange.listsshes.FirstOrDefault(x => x.ssh1 == IP && x.live == true);
                if (sssh != null) return sssh.ID.ToString() + "|" + sssh.ssh1 + "|" + sssh.username + "|" + sssh.password + "|" + sssh.country + "|" + sssh.city;
            }
            var b = IPAddressRange.listsshes.Where(x => (country == "" || country == null || x.country == country) && x.live == true);
            var a = b.OrderBy(x => x.ID).Skip(new Random().Next(b.Count())).FirstOrDefault();
            if (a == null) return "hetssh";


            return a.ID.ToString() + "|" + a.ssh1 + "|" + a.username + "|" + a.password + "|" + a.country + "|" + a.city;



            List<ssh> Listssh;

            if (offerID != null) Listssh = db.sshes.Where(x => x.country == country && x.live == true && (x.used == null || x.used.Contains("|" + offerID + "|") == false)).ToList();
            else
            {
                Listssh = db.sshes.Where(x => x.country == country && x.live == true).ToList();
            }

            if (Listssh.Count() == 0)
            {
                return "hetssh";
            }

            if (IP != null)
            {
                var _listSSH = Listssh.Where(x => x.ssh1.StartsWith(IP));
                if (_listSSH.Count() != 0) Listssh = _listSSH.ToList();
                else
                {
                    string curcity = citygeo;
                    if (curcity == null || curcity == "")
                    {
                        Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                        Match match = ip.Match(IP);
                        if (match.Success)
                        {
                            try
                            {
                                var city = reader.City(match.Groups[0].Value.ToString());
                                curcity = city.City.Name;
                            }
                            catch (Exception)
                            {


                            }

                        }
                    }
                    if (curcity != "" && curcity != null)
                    {
                        _listSSH = Listssh.Where(x => x.city == curcity);
                        if (_listSSH.Count() != 0) Listssh = _listSSH.ToList();
                    }
                }
            }
            else
            {
                if (citygeo != null && citygeo != "")
                {
                    string curcity = citygeo;

                    if (curcity != "" && curcity != null)
                    {
                        var _listSSH = Listssh.Where(x => x.city == curcity);
                        if (_listSSH.Count() != 0) Listssh = _listSSH.ToList();
                    }
                }
            }
            Random rd = new Random();
            int _rd = rd.Next(0, Listssh.Count());
            ssh randomssh = Listssh.OrderBy(x => x.ssh1).Skip(_rd).FirstOrDefault();
            if (offerID != null)
            {
                if (randomssh.used == null) randomssh.used = "|";
                randomssh.used += offerID + "|";
            }

            db.SaveChanges();
            return randomssh.ID.ToString() + "|" + randomssh.ssh1 + "|" + randomssh.username + "|" + randomssh.password + "|" + randomssh.country + "|" + randomssh.city;

        }
        public ActionResult RemoveDie1(string item)
        {
            string item1 = item.Replace("'", "''");
            IPAddressRange.listsshes.RemoveAll(x => x.country == item1&&x.live==false);
        //    db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ssh] WHERE [dbo].[ssh].country = " + "'" + item1 + "' AND [dbo].[ssh].live=0");
            return RedirectToAction("Index");
        }
        public ActionResult RemoveDie(string item)
        {
            string item1 = item.Replace("'", "''");
            IPAddressRange.listsshes.RemoveAll(x => x.country == item1 && x.live == false);
            //    db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ssh] WHERE [dbo].[ssh].country = " + "'" + item1 + "' AND [dbo].[ssh].live=0");
            return RedirectToAction("Index");
        }
        public ActionResult RemoveDieAll(string item)
        {
            string item1 = item.Replace("'", "''");
            IPAddressRange.listsshes.RemoveAll(x =>  x.live == false);
          //  db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ssh] WHERE [dbo].[ssh].live=0");
            return RedirectToAction("Index");
        }
        public ActionResult Refresh(string item)
        {
            string item1 = item.Replace("'", "''");
            // db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ssh] WHERE [dbo].[ssh].country = " + "'" + item1 + "' AND [dbo].[ssh].live=0" );
            var _usedlist = IPAddressRange.listsshes.Where(x => x.used != null && x.country == item);
            foreach (var item2 in _usedlist)
            {
                item2.used = null;
            }
       //     db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public void uploadssh(string sshes)
        {
            List<ssh> _listssh = new List<ssh>();
            IEnumerable<string> listssh = sshes.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var ssh in listssh)
            {
                List<string> sshdetail = ssh.Split(new string[] { "|" }, StringSplitOptions.None).ToList();
                if (sshdetail.Count >= 3)
                {
                    if (sshdetail.Count == 3)
                    {
                        sshdetail.Add("Unknown");
                        sshdetail.Add("Unknown");
                    }
                    else if (sshdetail.Count == 4) sshdetail.Add("Unknown");
                    string ssh_ = sshdetail[0];

                    string _country = sshdetail[3];
                    string _city = sshdetail[4];
                    MaxMind.GeoIP2.Responses.CityResponse city;
                    try
                    {
                        city = reader.City(sshdetail[0]);
                        _country = city.Country.Name;
                        _city = city.City.Name;
                    }
                    catch (Exception)
                    {


                    }



                    ssh prevssh =IPAddressRange.listsshes.FirstOrDefault(x => x.ssh1 == ssh_);

                    if (prevssh == null)
                    {
                        ssh newssh = new ssh();
                        newssh.ssh1 = sshdetail[0];
                        newssh.username = sshdetail[1];
                        newssh.password = sshdetail[2];
                        newssh.used = null;
                        newssh.live = true;
                        newssh.country = _country;
                        newssh.city = _city;

                        IPAddressRange.listsshes.Add(newssh);
                    }
                    else
                    {
                        
                        if (prevssh != null)
                        {
                            prevssh.username = sshdetail[1];
                            prevssh.password = sshdetail[2];
                            prevssh.country = _country;
                            prevssh.city = _city;
                            prevssh.live = true;
                        }
                    }

                }


            }
 
            /*
            using (var transactionScope = new TransactionScope())
            {
                // some stuff in dbcontext

                db.BulkInsert(_listssh);

                db.SaveChanges();
                transactionScope.Complete();
            }
            */
        }

        public string livesshes()
        {
            List<string> sshlistcontent = new List<string>();
            var listssh = IPAddressRange.listsshes.Where(x => x.live == true).ToArray();
            for (int i = 0; i < listssh.Count(); i++)
            {
                string content = listssh[i].ID.ToString();
                content += "|";
                content += listssh[i].ssh1;
                content += "|";
                content += listssh[i].username;
                content += "|";
                content += listssh[i].password;
                content += "|";
                content += listssh[i].country;
                content += "|";
                content += listssh[i].city;
                sshlistcontent.Add(content);
            }
            return String.Join("\r\n", sshlistcontent);
        }
        public string diesshes()
        {
            List<string> sshlistcontent = new List<string>();
            var listssh = IPAddressRange.listsshes.Where(x => x.live == false).ToArray();
            for (int i = 0; i < listssh.Count(); i++)
            {
                string content = listssh[i].ID.ToString();
                content += "|";
                content += listssh[i].ssh1;
                content += "|";
                content += listssh[i].username;
                content += "|";
                content += listssh[i].password;
                content += "|";
                content += listssh[i].country;
                content += "|";
                content += listssh[i].city;
                sshlistcontent.Add(content);
            }
            return String.Join("\r\n", sshlistcontent);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase[] files)
        {
            List<ssh> _listssh = new List<ssh>();
            string content = "";
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var contentLength = file.ContentLength;
                    var contentType = file.ContentType;

                    // Get file data
                    byte[] data = new byte[] { };
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        data = binaryReader.ReadBytes(file.ContentLength);
                        content += System.Text.Encoding.UTF8.GetString(data);
                        content += "\r\n";
                    }
                }
            }
            string sshes = content;
            IEnumerable<string> listssh = sshes.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            var _listsshdb = IPAddressRange.listsshes.Select(x => x.ssh1);
            string listip = String.Join("|", _listsshdb.ToArray());

            foreach (var ssh in listssh)
            {
                List<string> sshdetail = ssh.Split(new string[] { "|" }, StringSplitOptions.None).ToList();
                if (sshdetail.Count >= 3)
                {
                    if (sshdetail.Count == 3)
                    {
                        sshdetail.Add("Unknown");
                        sshdetail.Add("Unknown");
                    }
                    else if (sshdetail.Count == 4) sshdetail.Add("Unknown");
                    string ssh_ = sshdetail[0];

                    string _country = sshdetail[3];
                    string _city = sshdetail[4];
                    MaxMind.GeoIP2.Responses.CityResponse city;
                    try
                    {
                        city = reader.City(sshdetail[0]);
                        _country = city.Country.Name;
                        _city = city.City.Name;
                    }
                    catch (Exception)
                    {


                    }





                    if (!listip.Contains(ssh_))
                    {
                        ssh newssh = new ssh();
                        newssh.ssh1 = sshdetail[0];
                        newssh.username = sshdetail[1];
                        newssh.password = sshdetail[2];
                        newssh.used = null;
                        newssh.live = true;
                        newssh.country = _country;
                        newssh.city = _city;
                        listip += ssh_;
                        listip += "|";
                        _listssh.Add(newssh);
                    }
                    else
                    {
                        ssh prevssh = db.sshes.Where(x => x.ssh1 == ssh_).FirstOrDefault();
                        if (prevssh != null)
                        {
                            prevssh.username = sshdetail[1];
                            prevssh.password = sshdetail[2];
                            prevssh.country = _country;
                            prevssh.city = _city;
                            prevssh.live = true;
                        }
                    }

                }


            }
            using (var transactionScope = new TransactionScope())
            {
                // some stuff in dbcontext

                db.BulkInsert(_listssh);

                db.SaveChanges();
                transactionScope.Complete();
            }
            return RedirectToAction("Index");
        }
    }
}