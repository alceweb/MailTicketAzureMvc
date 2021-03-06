﻿using MailTicketAzureMvc.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MailTicketAzureMvc.Controllers
{
    public class UsersAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Gestione punti vendita";
            ViewBag.UtentiCount = UserManager.Users.Count();
            return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Nome = user.Nome,
                Telefono = user.Telefono,
                CodiceFiscale = user.CodiceFiscale,
                PartitaIva = user.PartitaIva,
                Insegna = user.Insegna,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,Nome,Telefono,CodiceFiscale,PartitaIva,Insegna")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.Nome = editUser.Nome;
                user.Telefono = editUser.Telefono;
                user.CodiceFiscale = editUser.CodiceFiscale;
                user.PartitaIva = editUser.PartitaIva;
                user.Insegna = editUser.Insegna;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult EventiPv(string uid)
        {
            var utente = UserManager.Users.Where(u=>u.Id == uid);
            ViewBag.Title = "Eventi assegnati";
            ViewBag.Message = utente;
            var spettacoli = db.Associazionis.OrderBy(s=>s.Spettacolo).Where(r => r.IdUtente == uid);
            ViewBag.Spettacoli = spettacoli;
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket();
            return View(eventi.ToList());
        }

        [HttpPost]
        public ActionResult EventiPv(string uid, string cerca)
        {
            var utente = UserManager.Users.Where(u => u.Id == uid);
            ViewBag.Title = "Eventi assegnati";
            ViewBag.Message = utente;
            var service = new ServizioPrevendita.ServizioPrevendita();
            var eventi = service.RecuperaEventiMailticket();
            if (string.IsNullOrEmpty(cerca))
            {
                var spettacoli = db.Associazionis.OrderBy(s=>s.Spettacolo).ThenBy(s=>s.IdEvento).Where(r => r.IdUtente == uid);
                ViewBag.Spettacoli = spettacoli;
            }
            else
            {
                var spettacoli = db.Associazionis.OrderBy(s => s.Spettacolo).ThenBy(s => s.IdEvento).Where(r => r.IdUtente == uid & r.Spettacolo.Contains(cerca.ToUpper()));
                ViewBag.Spettacoli = spettacoli;
            }
            return View(eventi.ToList());
        }

        public ActionResult DeleteAss(int? id, string uid, string data)
        {
            var utente = UserManager.Users.Where(u => u.Id == uid);
            ViewBag.Title = "Rimuovi evento";
            ViewBag.Message = utente;
            Associazione associazione = db.Associazionis.Find(id);
            return View(associazione);
        }

        [HttpPost, ActionName("DeleteAss")]
        public ActionResult DeleteAssConfirmed(int id)
        {
            Associazione associazione = db.Associazionis.Find(id);
            db.Associazionis.Remove(associazione);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
