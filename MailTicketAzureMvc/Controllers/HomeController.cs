using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailTicketAzureMvc.ServizioPrevendita;
using Microsoft.AspNet.Identity;
namespace MailTicketAzureMvc.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.Title = "Home amministratore";
                ViewBag.Message = "Gestisci i punti vendita";
            }
            var service = new ServizioPrevendita.ServizioPrevendita();
            ViewBag.Service = service.RecuperaEventiMailticket().Length;
           var  eventi = service.RecuperaEventiMailticket();
            
            ViewBag.Eventi = eventi;
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
    }
}