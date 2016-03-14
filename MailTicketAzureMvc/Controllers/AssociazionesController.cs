using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MailTicketAzureMvc.Models;

namespace MailTicketAzureMvc.Controllers
{
    public class AssociazionesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Associaziones
        public ActionResult Index()
        {
            var utente = User.Identity.GetUserId();
            var spettacoli = db.Associazionis.Where(u => u.IdUtente == utente).ToList();
            return View(db.Associazionis.Where(u=>u.IdUtente== utente).ToList());
        }

        // GET: Associaziones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Associazione associazione = db.Associazionis.Find(id);
            if (associazione == null)
            {
                return HttpNotFound();
            }
            return View(associazione);
        }

        // GET: Associaziones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Associaziones/Create
        // Per proteggere da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per ulteriori dettagli, vedere http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Associazione,IdUtente,IdMan,IdSpettacolo,IdEvento,IdSpettMail")] Associazione associazione)
        {
            if (ModelState.IsValid)
            {
                db.Associazionis.Add(associazione);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(associazione);
        }

        // GET: Associaziones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Associazione associazione = db.Associazionis.Find(id);
            if (associazione == null)
            {
                return HttpNotFound();
            }
            return View(associazione);
        }

        // POST: Associaziones/Edit/5
        // Per proteggere da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per ulteriori dettagli, vedere http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Associazione,IdUtente,IdMan,IdSpettacolo,IdEvento,IdSpettMail")] Associazione associazione)
        {
            if (ModelState.IsValid)
            {
                db.Entry(associazione).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(associazione);
        }

        // GET: Associaziones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Associazione associazione = db.Associazionis.Find(id);
            if (associazione == null)
            {
                return HttpNotFound();
            }
            return View(associazione);
        }

        // POST: Associaziones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Associazione associazione = db.Associazionis.Find(id);
            db.Associazionis.Remove(associazione);
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
