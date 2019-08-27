using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sshsv.Models;

namespace sshsv.Controllers
{
    public class sshesController : Controller
    {
        private sshserver3Entities db = new sshserver3Entities();

        // GET: sshes
        public ActionResult Index()
        {
            return View(IPAddressRange.listsshes.ToList());
        }

        // GET: sshes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ssh ssh = db.sshes.Find(id);
            if (ssh == null)
            {
                return HttpNotFound();
            }
            return View(ssh);
        }

        // GET: sshes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: sshes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ssh1,username,password,country,city,used,live")] ssh ssh)
        {
            if (ModelState.IsValid)
            {
                db.sshes.Add(ssh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ssh);
        }

        // GET: sshes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ssh ssh = db.sshes.Find(id);
            if (ssh == null)
            {
                return HttpNotFound();
            }
            return View(ssh);
        }

        // POST: sshes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ssh1,username,password,country,city,used,live")] ssh ssh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ssh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ssh);
        }

        // GET: sshes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ssh ssh = db.sshes.Find(id);
            if (ssh == null)
            {
                return HttpNotFound();
            }
            return View(ssh);
        }

        // POST: sshes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ssh ssh = db.sshes.Find(id);
            db.sshes.Remove(ssh);
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
