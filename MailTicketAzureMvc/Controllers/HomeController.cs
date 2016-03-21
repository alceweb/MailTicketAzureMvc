using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailTicketAzureMvc.ServizioPrevendita;
using Microsoft.AspNet.Identity;
using System.Net;
using MailTicketAzureMvc.Models;
using System.Data;
using System.Data.Entity;

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
                return RedirectToAction("PvSpettacoli");
            }
        }

        public ActionResult IndexAdmin()
        {
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Lista eventi";
            ViewBag.Message = "Eventi";
                var eventi = service.RecuperaEventiMailticket()
                    .OrderBy(r => r.idMan).ThenBy(r => r.idEvento);
                ViewBag.Eventi = eventi;
                ViewBag.EventiTot = eventi.Count();
            return View();
        }

        [HttpPost]
        public ActionResult IndexAdmin(string cerca)
        {
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Lista eventi";
            ViewBag.Message = "Eventi";
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).ThenBy(r => r.idEvento).Where(r=>r.idMan.Contains(cerca.ToUpper()) | r.Spettacolo.Contains(cerca.ToUpper()));
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }

        public ActionResult IndexPv()
        {
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
            ViewBag.CarrelloOk = "";
            var utente = User.Identity.GetUserId();
            var spettacoli = db.Associazionis.Where(r => r.IdUtente == utente);
            ViewBag.Spettacoli = spettacoli;
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket();
            return View(eventi.ToList());
        }

        public ActionResult PvManifestazioni()
        {
            ViewBag.Title = "Manifestazioni";
            ViewBag.Message = "Nessuna manifestazione assegnata al punto vendita";
            return View();
        }
        public ActionResult AdmEventi(string man, string spe, string eve)
        {
            ViewData["ut"] = db.Users.ToList();
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var service = new ServizioPrevendita.ServizioPrevendita();
            if (eve == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Admin eventi";
            ViewBag.Message = "idEvento " + eve;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).Where(r => r.idEvento == eve & r.idMan == man);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View(ViewBag.Eventi);
        }
        [HttpPost]
        public ActionResult AdmEventi([Bind(Include = "idUtente,Idman,IdEvento,IdSpettMail,Spettacolo")] Associazione associazione)
        {
            var utente = Request["IdUtente"];
            var man = Request["man"];
            var spe = Request["spe"];
            var eve = Request["eve"];
            var verifica = db.Associazionis.Where(u => u.IdUtente == utente & u.IdMan == man & u.Spettacolo == spe & u.IdEvento == eve);
            if (verifica.Count() == 0)
            {
                var service = new ServizioPrevendita.ServizioPrevendita();
                var eventi = service.RecuperaEventiMailticket().Where(r => r.idMan == man & r.Spettacolo == spe & r.idEvento == eve);
                ViewBag.Eventi = eventi;
                ViewBag.Eventis = eventi;
                foreach (var item in ViewBag.Eventis)
                {
                    if (ModelState.IsValid)
                    {
                        associazione.IdEvento = item.idEvento;
                        associazione.IdMan = item.idMan;
                        associazione.IdSpettacolo = item.idSpettacolo;
                        associazione.IdSpettMail = item.idSpettMail;
                        associazione.Spettacolo = item.Spettacolo;
                        db.Associazionis.Add(associazione);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("AssegnaManOk");
            }
            else
            {
                return RedirectToAction("AssegnaManKo");

            }
        }
        public ActionResult AdmSpettacoli(string man, string spe)
        {
            ViewData["ut"] = db.Users.ToList();
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var service = new ServizioPrevendita.ServizioPrevendita();
            if (spe == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Admin spettacoli";
            ViewBag.Message = spe;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).Where(r => r.Spettacolo == spe);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }
        [HttpPost]
        public ActionResult AdmSpettacoli([Bind(Include = "idUtente,Idman,IdEvento,IdSpettMail,Spettacolo")] Associazione associazione)
        {
            var utente = Request["IdUtente"];
            var man = Request["man"];
            var spe = Request["spe"];
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket().Where(r => r.Spettacolo == Request["spe"]);
            ViewBag.Eventi = eventi;
            ViewBag.Eventis = eventi;
            foreach (var item in ViewBag.Eventis)
            {
                if (ModelState.IsValid)
                {
                //verifico se il singolo evento è già assegnato
                    string verificaEv = item.idEvento;
                    string verificaMan = item.idMan;
                    var verifica = db.Associazionis.Where(e => e.IdMan == verificaMan & e.Spettacolo == spe & e.IdEvento == verificaEv).Count();
                    if (verifica == 0)
                    //se non è assegnato inserisco i record
                    {
                    associazione.IdEvento = item.idEvento;
                        associazione.IdMan = item.idMan;
                        associazione.IdSpettacolo = item.idSpettacolo;
                        associazione.IdSpettMail = item.idSpettMail;
                        associazione.Spettacolo = item.Spettacolo;
                        db.Associazionis.Add(associazione);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("AssegnaManOk");
    }

        public ActionResult AdmManifestazioni(string man)
        {
            ViewData["ut"] = db.Users.ToList();
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var service = new ServizioPrevendita.ServizioPrevendita();
            if (man == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            ViewBag.Title = "Admin manifestazioni";
            ViewBag.Message = man;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.Spettacolo).Where(r => r.idMan == man);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }

        [HttpPost]
        public ActionResult AdmManifestazioni([Bind(Include = "idUtente,Idman,IdEvento,IdSpettMail,Spettacolo")] Associazione associazione)
        {
            var utente = Request["IdUtente"];
            var man = Request["man"];
                var service = new ServizioPrevendita.ServizioPrevendita();
                var eventi = service.RecuperaEventiMailticket().Where(r=>r.idMan == Request["Man"]);
                ViewBag.Eventi = eventi;
                ViewBag.Eventis = eventi;
                foreach (var item in ViewBag.Eventis)
                {
                    if (ModelState.IsValid)
                    {
                        //verifico se il singolo evento è già assegnato
                        string verificaEv = item.idEvento;
                        string verificaSpe = item.idEvento;
                        var verifica = db.Associazionis.Where(e => e.IdMan == man & e.Spettacolo == verificaSpe & e.IdEvento == verificaEv).Count();
                        if (verifica == 0)
                        //se non è assegnato inserisco i record
                        {
                            associazione.IdMan = item.idMan;
                            associazione.IdEvento = item.idEvento;
                            associazione.IdSpettacolo = item.idSpettacolo;
                            associazione.IdSpettMail = item.idSpettMail;
                            associazione.Spettacolo = item.Spettacolo;
                            db.Associazionis.Add(associazione);
                            db.SaveChanges();
                        }
                    }
                }
            return RedirectToAction("AssegnaManOk");
        }
        public ActionResult AssegnaMan([Bind(Include = "idUtente,Idman,IdEvento,IdSpettMail")] Associazione associazione)
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
        public ActionResult AssegnaManKo()
        {
            return View();
        }
        public ActionResult AssegnaManOk()
        {
            return View();
        }
        public ActionResult test()
        {
            var utente = User.Identity.GetUserId();
            var spettacoli = db.Associazionis.Where(r => r.IdUtente == utente);
            ViewBag.Spettacoli = spettacoli;
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket();
            return View(eventi.ToList());
        }
    }
}