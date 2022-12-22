using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;
using UnrealReplayServer.Data;
using System.Web;
using UnrealReplayServer.Databases.Models;
using Microsoft.AspNetCore.Identity;

namespace UnrealReplayServer.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly UnrealReplayServerContext _context;

        public SessionsController(UnrealReplayServerContext context)
        {
            _context = context;
        }


      /*  [HttpGet]
        public IActionResult CorpLogin()
        {
            var authProperties = _signInManager
                          .ConfigureExternalAuthenticationProperties("AzureAD",
               Url.Action("SigninOidc", "Account", null, Request.Scheme));

            return Challenge(authProperties, "AzureAD");
        } */

        /*
        [HttpPost("signinoidc")]
        [AllowAnonymous]
        public async Task<IActionResult> SigninOidc([FromForm] object data)
        {
            //this never runs
            //return View(await _context.Session.ToListAsync());
            return RedirectToAction(returnUrl);

        } */

        // GET: Sessions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            System.Diagnostics.Debug.WriteLine("Index");
            return View(await _context.Session.OrderByDescending(s => s.CreationDate).Take(10).ToListAsync());
        }

        // GET: Sessions/Details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.SessionName == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Sessions/Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsLive,SessionName,AppVersion,NetVersion,Changelist,Meta,PlatformFriendlyName,TotalDemoTimeMs,TotalUploadedBytes,TotalChunks,CreationDate,InternalUsers")] Session session)
        {
            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Sessions/Edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IsLive,SessionName,AppVersion,NetVersion,Changelist,Meta,PlatformFriendlyName,TotalDemoTimeMs,TotalUploadedBytes,TotalChunks,CreationDate,InternalUsers")] Session session)
        {
            if (id != session.SessionName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.SessionName))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Sessions/Delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.SessionName == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var session = await _context.Session.FindAsync(id);
            _context.Session.Remove(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(string id)
        {
            return _context.Session.Any(e => e.SessionName == id);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
