using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailTicketAzureMvc.ServizioPrevendita;
using Microsoft.AspNet.Identity;
using System.Net;
using MailTicketAzureMvc.Models;

namespace MailTicketAzureMvc.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {

                return RedirectToAction("IndexAdmin");
            }
            else {
                return RedirectToAction("PvManifestazioni");
            }
        }

        public ActionResult IndexAdmin()
        {
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Home amministratore";
            ViewBag.Message = "Eventi a disposizione per la prevendita";
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).ThenBy(r=>r.idEvento);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "Istruzioni per l'uso";
            ViewBag.Message = "A cosa serve e come si usa il portale";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Contattaci";
            ViewBag.Message = "Se hai domande o non riesci a collegarti contattaci pure";

            return View();
        }
        public ActionResult PvSpettacoli()
        {
            var utente = User.Identity.GetUserId();
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Home amministratore";
            ViewBag.Message = "Spettacoli";
            var groupedMan = service.RecuperaEventiMailticket().GroupBy(r => r.idMan);
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).Where(r=>r.idEvento == "1");
            ViewBag.Eventi = eventi;
            return View();
        }
        public ActionResult PvManifestazioni()
        {
            ViewBag.Title = "Manifestazioni";
            ViewBag.Message = "Nessuna manifestazione assegnata al punto vendita";
            return View();
        }
        public ActionResult AdmEventi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Admin eventi";
            ViewBag.Message = "idEvento " + id;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).Where(r => r.idEvento == id);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View(ViewBag.Eventi);
        }
        public ActionResult AdmSpettacoli(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Admin spettacoli";
            ViewBag.Message = id;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).Where(r => r.Spettacolo == id);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }
        public ActionResult AdmManifestazioni(string id)
        {
            ViewData["ut"] = db.Users.ToList();
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var service = new ServizioPrevendita.ServizioPrevendita();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Admin manifestazioni";
            ViewBag.Message = id;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.Spettacolo).Where(r => r.idMan == id);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }

        [HttpPost]
        public ActionResult AdmManifestazioni([Bind(Include = "idUtente,Idman")] Associazione associazione)
        {
            if (ModelState.IsValid)
            {
                db.Associazionis.Add(associazione);
                db.SaveChanges();
                return RedirectToAction("indexAdmin", "home");
            }
            ViewBag.Title = "fallito";
            return RedirectToAction("Contact", "Home");
        }
    }
}