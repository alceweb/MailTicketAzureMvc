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
            ViewBag.Title = "Gestione spettacoli";
            ViewBag.Message = "Eventi";
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).ThenBy(r=>r.idEvento);
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
            var verifica = db.Associazionis.Include("IdEvento,IdSpettacolo,IdSpettMail,IdMan,IdUtente,Spettacolo").Where(u => u.IdUtente == utente & u.IdMan == man & u.Spettacolo == spe);
            var service = new ServizioPrevendita.ServizioPrevendita();
            if (verifica.Count() == 0)
            {
                var eventi = service.RecuperaEventiMailticket().OrderBy(r => r.Spettacolo).Where(r => r.idMan == man & r.Spettacolo == spe);
                ViewBag.Eventi = eventi;
                ViewBag.Eventis = eventi;
                foreach (var item in ViewBag.Eventis)
                {
                    if (ModelState.IsValid)
                    {
                        var eventoOk = service.RecuperaEventiMailticket().Where(r => r.idEvento == item.idEvento & r.idMan == item.idMan);
                        if (eventoOk.Count() == 0)
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
            else
            {
                    return RedirectToAction("AssegnaManKo");

            }
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
            var verifica = db.Associazionis.Include("IdEvento,IdSpettacolo,IdSpettMail,IdMan,IdUtente,Spettacolo").Where(u => u.IdUtente == utente & u.IdMan == man);
            if (verifica.Count() == 0)
            {
                var service = new ServizioPrevendita.ServizioPrevendita();
                var eventi = service.RecuperaEventiMailticket().OrderBy(r => r.Spettacolo).Where(r=>r.idMan == Request["Man"]);
                    ViewBag.Eventi = eventi;
                    ViewBag.Eventis = eventi;
                    foreach (var item in ViewBag.Eventis)
                    {
                        if (ModelState.IsValid)
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
            return RedirectToAction("AssegnaManOk");
            }
            else
            {
                return RedirectToAction("AssegnaManKo");

            }
        }
        public ActionResult AssegnaManKo()
        {
            return View();
        }
        public ActionResult AssegnaManOk()
        {
            return View();
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