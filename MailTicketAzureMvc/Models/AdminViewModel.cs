using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MailTicketAzureMvc.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Nome ruolo")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }
        [Display(Name = "Codice Fiscale")]
        public string CodiceFiscale { get; set; }
        [Display(Name = "Partita IVA")]
        public string PartitaIva { get; set; }
        [Display(Name = "Insegna")]
        public string Insegna { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}