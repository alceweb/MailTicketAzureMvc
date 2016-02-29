using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailTicketAzureMvc.Models
{
    public class Evento
    {
        public string Name { get;set;}
        public Evento()
        {
            var EventoMail = new ServizioPrevendita.EventoMail();
            Name = EventoMail.DescEvento;
        }
    }
}