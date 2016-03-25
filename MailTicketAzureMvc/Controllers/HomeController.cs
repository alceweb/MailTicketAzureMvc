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
                return RedirectToAction("IndexPv");
            }
        }

        public ActionResult IndexAdmin()
        {
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Title = "Lista eventi";
            ViewBag.Message = "Eventi";
            //Estraggo tutti gli eventi dal WS
            var eventi = service.RecuperaEventiMailticket().OrderBy(r => r.idMan).ThenBy(r => r.idEvento);
            ViewBag.EventiTot = eventi.Count();
            ViewBag.Eventi = eventi;
            foreach (var item in ViewBag.Eventi)
            {
                string man = item.idMan;
                string eve = item.idEvento;
                var pv = db.Associazionis.Where(u => u.IdMan == man & u.IdEvento == eve);
                ViewBag.Pv = pv;
            }
            var puntiVendita = db.Associazionis;
            ViewBag.PuntiVendita = puntiVendita;
            return View(puntiVendita.ToList());
        }

        [HttpPost]
        public ActionResult IndexAdmin(string cerca)
        {
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Title = "Lista eventi";
            ViewBag.Message = "Eventi";
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).ThenBy(r => r.idEvento).Where(r=>r.idMan.Contains(cerca.ToUpper()) | r.Spettacolo.Contains(cerca.ToUpper()));
            ViewBag.Eventi = eventi;
            var pv = db.Associazionis;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }

        public ActionResult IndexPv()
        {
            var utente = User.Identity.GetUserId();
            ViewBag.Title = "Ricerca spettacoli";
            ViewBag.Message = "Spettacoli";
            var spettacoli = db.Associazionis.Where(r => r.IdUtente == utente);
            ViewBag.Spettacoli = spettacoli;
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket();
            return View(eventi.ToList());
        }

        [HttpPost]
        public ActionResult IndexPv(string cerca)
        {
            var utente = User.Identity.GetUserId();
            if (cerca.Length != 0)
            {
            var spettacoli = db.Associazionis.Where(r => r.IdUtente == utente & r.Spettacolo.Contains(cerca.ToUpper()));
            ViewBag.Spettacoli = spettacoli;
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket();
            return View(eventi.ToList());
            }
            else
            {
                var spettacoli = db.Associazionis.Where(r => r.IdUtente == utente);
                ViewBag.Spettacoli = spettacoli;
                var service = new ServizioPrevendita.ServizioPrevendita();
                var eventi = service.RecuperaEventiMailticket();
                return View(eventi.ToList());
            }
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
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Title = "Admin eventi";
            ViewBag.Message = "idEvento " + eve;
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            if (eve == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).Where(r => r.idEvento == eve & r.idMan == man);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View(ViewBag.Eventi);
        }
        [HttpPost]
        public ActionResult AdmEventi([Bind(Include = "idUtente,Idman,IdEvento,IdSpettMail,Spettacolo")] Associazione associazione)
        {
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var utente = Request["IdUtente"];
            var man = Request["man"];
            var spe = Request["spe"];
            var eve = Request["eve"];
            ViewBag.Title = "Admin eventi";
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket().Where(r => r.idMan == man & r.Spettacolo == spe & r.idEvento == eve);
            ViewBag.Eventi = eventi;
            if (!string.IsNullOrEmpty(utente))
            {
                ViewBag.Eventis = eventi;
                foreach (var item in ViewBag.Eventis)
                {
                    if (ModelState.IsValid)
                    {
                        //verifico se il singolo evento è già assegnato
                        var verifica = db.Associazionis.Where(u => u.IdUtente == utente & u.IdMan == man & u.Spettacolo == spe & u.IdEvento == eve);
                        if (verifica.Count() == 0)
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
            }
            else
            {
                ViewBag.Ko = "non hai selezionato l'insegna!";
                return View();
            }
            return RedirectToAction("AssegnaEveOk");
        }
        public ActionResult AdmSpettacoli(string man, string spe)
        {
            ViewBag.Title = "Admin spettacoli";
            ViewBag.Message = spe;
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var service = new ServizioPrevendita.ServizioPrevendita();
            if (spe == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
            var eventi = service.RecuperaEventiMailticket()
                .OrderBy(r => r.idMan).Where(r => r.Spettacolo == spe);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            return View();
        }
        [HttpPost]
        public ActionResult AdmSpettacoli([Bind(Include = "idUtente,Idman,IdEvento,IdSpettMail,Spettacolo")] Associazione associazione)
        {
            ViewBag.Title = "Admin spettacoli";
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var utente = Request["IdUtente"];
            var man = Request["man"];
            var spe = Request["spe"];
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket().Where(r => r.Spettacolo == Request["spe"]);
            ViewBag.Eventi = eventi;
            if (!string.IsNullOrEmpty(utente))
            {
                ViewBag.Eventis = eventi;
                foreach (var item in ViewBag.Eventis)
                {
                    if (ModelState.IsValid)
                    {
                    //verifico se il singolo evento è già assegnato
                        string verificaEv = item.idEvento;
                        string verificaMan = item.idMan;
                        var verifica = db.Associazionis.Where(e => e.IdUtente == utente & e.IdMan == verificaMan & e.Spettacolo == spe & e.IdEvento == verificaEv).Count();
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

             }
            else
            {
                ViewBag.Ko = "non hai selezionato l'insegna!";
                return View();
            }

            return RedirectToAction("AssegnaSpeOk");
    }

        public ActionResult AdmManifestazioni(string man)
        {
            ViewBag.Title = "Admin manifestazioni";
            ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var utente = Request["IdUtente"];
            var service = new ServizioPrevendita.ServizioPrevendita();
            if (man == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Service = service.RecuperaEventiMailticket().Length;
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
            ViewBag.Title = "Admin manifestazioni";
            ViewBag.Message = man;
           ViewBag.IdUtente = new SelectList(db.Users, "Id", "Insegna");
            var eventi = service.RecuperaEventiMailticket().Where(r => r.idMan == man);
            ViewBag.Eventi = eventi;
            ViewBag.EventiTot = eventi.Count();
            if (!string.IsNullOrEmpty(utente))
            {
                ViewBag.Eventis = eventi;
                foreach (var item in ViewBag.Eventi)
                {
                    if (ModelState.IsValid)
                    {
                        //verifico se il singolo evento è già assegnato
                        string verificaEv = item.idEvento;
                        string verificaSpe = item.Spettacolo;
                        var verifica = db.Associazionis.Where(e => e.IdUtente == utente & e.IdMan == man & e.Spettacolo == verificaSpe & e.IdEvento == verificaEv).Count();
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
            }
            else
            {
                ViewBag.Ko = "non hai selezionato l'insegna!";
                return View();
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
        public ActionResult AssegnaSpeOk()
        {
            return View();
        }
        public ActionResult AssegnaEveOk()
        {
            return View();
        }
        public ActionResult test()
        {
            var spettacoli = db.Associazionis.OrderBy(s => s.Spettacolo);
            ViewBag.Spettacoli = spettacoli;
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket();
            return View(eventi.ToList());
        }

        [HttpPost]
        public ActionResult test(string save, string cancel)
        {
            if (!string.IsNullOrEmpty(save))
            {
                ViewBag.Message = "Customer saved successfully!";
            }
            if (!string.IsNullOrEmpty(cancel))
            {
                ViewBag.Message = "The operation was cancelled!";
            }
            var utente = User.Identity.GetUserId();
            var spettacoli = db.Associazionis.Where(r => r.IdUtente == utente);
            ViewBag.Spettacoli = spettacoli;
            return View();
        }
    }
}