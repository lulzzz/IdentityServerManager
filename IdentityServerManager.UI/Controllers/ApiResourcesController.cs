﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerManager.UI.Data;
using IdentityServerManager.UI.Models;
using AutoMapper;
using IdentityServerManager.UI.Infrastructure;
using System.Data.SqlClient;

namespace IdentityServerManager.UI.Controllers
{
    public class ApiResourcesController : Controller
    {
        private readonly ConfigurationDbContext _context;

        public ApiResourcesController(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string SuccessMessage = null)
        {
            ViewData["SuccessMessage"] = SuccessMessage;
            var apiResource = await _context.ApiResources.ToListAsync();
            return View(Mapper.Map<IEnumerable<ApiResource>, IEnumerable<ApiResourceViewModel>>(apiResource));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiResource = await _context.ApiResources
                .SingleOrDefaultAsync(m => m.Id == id);
            if (apiResource == null)
            {
                return NotFound();
            }
            return await Task.FromResult(PartialView("_details", apiResource.MapTo<ApiResourceViewModel>()));
        }

        public IActionResult Create()
        {
            return View(new ApiResourceViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiResourceViewModel apiResourceVM)
        {
            _context.Add(apiResourceVM.MapTo<ApiResource>());
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiResource = await _context.ApiResources.SingleOrDefaultAsync(m => m.Id == id);
            if (apiResource == null)
            {
                return NotFound();
            }
            return View(apiResource.MapTo<ApiResourceViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] ApiResourceViewModel apiResourceVM)
        {
            _context.Update(apiResourceVM.MapTo<ApiResource>());
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apiResource = await _context.ApiResources.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApiResources.Remove(apiResource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { SuccessMessage = "Api Resource successfully deleted." });
        }

    }
}
