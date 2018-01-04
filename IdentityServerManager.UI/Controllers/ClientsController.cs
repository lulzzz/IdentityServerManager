﻿using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerManager.UI.Data;
using IdentityServerManager.UI.Infrastructure;
using IdentityServerManager.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServerManager.UI.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ConfigurationDbContext _context;

        public ClientsController(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string SuccessMessage = null)
        {
            ViewData["SuccessMessage"] = SuccessMessage;
            var clients = await _context.Clients.ToListAsync();
            return View(Mapper.Map<IEnumerable<Client>, IEnumerable<ClientMainViewModel>>(clients));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return await Task.FromResult(PartialView("_details", client.MapTo<ClientMainViewModel>()));
        }

        public async Task<IActionResult> Main(int? id)
        {
            ClientMainViewModel clientVM;
            if (id.HasValue)
            {
                var client = await _context.Clients.SingleOrDefaultAsync(m => m.Id == id);
                if (client == null)
                {
                    return NotFound();
                }
                clientVM = client.MapTo<ClientMainViewModel>();
            }
            else
            {
                clientVM = new ClientMainViewModel();
            }          
            clientVM.IdentityProtocolTypes = new List<string> {
                    ProtocolTypes.OpenIdConnect,
                    ProtocolTypes.Saml2p,
                    ProtocolTypes.WsFederation };

            return View(clientVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Main(ClientMainViewModel clientVM)
        {
            if (ModelState.IsValid)
            {
                if (clientVM.Id != 0)
                {
                    _context.Update(clientVM.MapTo<Client>());
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Add(clientVM.MapTo<Client>());
                    clientVM.Id = await _context.SaveChangesAsync();
                }

                if (string.IsNullOrEmpty(clientVM.NextUrl))
                {
                    return RedirectToAction(nameof(Index), new { SuccessMessage = "Client successfully created." });
                }
                else
                {
                    return RedirectToAction(clientVM.NextUrl, new { id = clientVM.Id, SuccessMessage = "Data successfully saved." });
                }
            }
            clientVM.NextUrl = string.Empty;
            return View(clientVM);
        }


        public async Task<IActionResult> Scopes(int? id, string SuccessMessage = null)
        {
            ViewData["SuccessMessage"] = SuccessMessage;
            var client = await _context.Clients.SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            var clientVM = client.MapTo<ClientScopesViewModel>();
            return View(clientVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Scopes(ClientScopesViewModel clientVM)
        {
            if (ModelState.IsValid)
            {
                var client = await _context.Clients.SingleOrDefaultAsync(m => m.Id == clientVM.Id);
                client.AllowedScopes = clientVM.AllowedScopes;
                _context.Update(client);
                await _context.SaveChangesAsync();

                if (string.IsNullOrEmpty(clientVM.NextUrl))
                {
                    return RedirectToAction(nameof(Index), new { SuccessMessage = "Client successfully edited." });
                }
                else
                {
                    return RedirectToAction(clientVM.NextUrl, new { id = clientVM.Id, SuccessMessage = "Data successfully saved." });
                }
            }
            clientVM.NextUrl = string.Empty;
            return View(clientVM);
        }

        public async Task<IActionResult> Claims(int? id, string SuccessMessage = null)
        {
            ViewData["SuccessMessage"] = SuccessMessage;
            var client = await _context.Clients.SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            var clientVM = client.MapTo<ClientClaimsViewModel>();
            return View(clientVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Claims(ClientClaimsViewModel clientVM)
        {
            if (ModelState.IsValid)
            {
                var client = await _context.Clients.SingleOrDefaultAsync(m => m.Id == clientVM.Id);
                client.Claims = clientVM.Claims;
                _context.Update(client);
                await _context.SaveChangesAsync();

                if (string.IsNullOrEmpty(clientVM.NextUrl))
                {
                    return RedirectToAction(nameof(Index), new { SuccessMessage = "Client successfully edited." });
                }
                else
                {
                    return RedirectToAction(clientVM.NextUrl, new { id = clientVM.Id, SuccessMessage = "Data successfully saved." });
                }
            }
            clientVM.NextUrl = string.Empty;
            return View(clientVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.SingleOrDefaultAsync(m => m.Id == id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { SuccessMessage = "Client successfully deleted." });
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
