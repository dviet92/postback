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
    public class networksController : Controller
    {

        private sshserver3Entities db = new sshserver3Entities();

        // GET: networks
        public ActionResult Index()
        {
            return View(db.networks.ToList());
        }

        // GET: networks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            network network = db.networks.Find(id);
            if (network == null)
            {
                return HttpNotFound();
            }
            return View(network);
        }

        // GET: networks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: networks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,networkName,networkPb,s_rate")] network network)
        {
            if (ModelState.IsValid)
            {
                db.networks.Add(network);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(network);
        }

        // GET: networks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            network network = db.networks.Find(id);
            if (network == null)
            {
                return HttpNotFound();
            }
            return View(network);
        }

        // POST: networks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,networkName,networkPb,s_rate")] network network)
        {
            if (ModelState.IsValid)
            {
                db.Entry(network).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(network);
        }

        // GET: networks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            network network = db.networks.Find(id);
            if (network == null)
            {
                return HttpNotFound();
            }
            return View(network);
        }

        // POST: networks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            network network = db.networks.Find(id);
            db.networks.Remove(network);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult redirect(string url)
        {
            string rawURL = Request.RawUrl;
            string url1 = rawURL.Replace("/networks/redirect?url=", "");
            return new RedirectResult(url1);
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
