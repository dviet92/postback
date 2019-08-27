using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sshsv.Models;
using sshsv.Filters;

namespace sshsv.Controllers
{
    [AuthorizeIPAddress]
    public class conversionsController : Controller
    {
       
        private sshserver3Entities db = new sshserver3Entities();

        // GET: conversions
        public ActionResult Index()
        {
            var conversions = db.conversions.Include(c => c.network);
            return View(conversions.ToList());
        }

        // GET: conversions/Details/5
       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            conversion conversion = db.conversions.Find(id);
            if (conversion == null)
            {
                return HttpNotFound();
            }
            return View(conversion);
        }

        // GET: conversions/Create
        public ActionResult Create()
        {
            ViewBag.networkID = new SelectList(db.networks, "ID", "networkName");
            return View();
        }

        // POST: conversions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,networkID,offerID,aff_source,aff_subid1,aff_subid2,aff_subid3,session_ip,payout,device_brand,device_model,device_os,time_install,country")] conversion conversion)
        {
            if (ModelState.IsValid)
            {
                db.conversions.Add(conversion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.networkID = new SelectList(db.networks, "ID", "networkName", conversion.networkID);
            return View(conversion);
        }

        // GET: conversions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            conversion conversion = db.conversions.Find(id);
            if (conversion == null)
            {
                return HttpNotFound();
            }
            ViewBag.networkID = new SelectList(db.networks, "ID", "networkName", conversion.networkID);
            return View(conversion);
        }

        // POST: conversions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,networkID,offerID,aff_source,aff_subid1,aff_subid2,aff_subid3,session_ip,payout,device_brand,device_model,device_os,time_install,country")] conversion conversion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conversion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.networkID = new SelectList(db.networks, "ID", "networkName", conversion.networkID);
            return View(conversion);
        }

        // GET: conversions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            conversion conversion = db.conversions.Find(id);
            if (conversion == null)
            {
                return HttpNotFound();
            }
            return View(conversion);
        }

        // POST: conversions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            conversion conversion = db.conversions.Find(id);
            db.conversions.Remove(conversion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
