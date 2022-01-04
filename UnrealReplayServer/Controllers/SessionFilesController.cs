using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UnrealReplayServer.Data;
using UnrealReplayServer.Databases.Models;

namespace UnrealReplayServer.Controllers
{
    public class SessionFilesController : Controller
    {
        private readonly UnrealReplayServerContext _context;

        public SessionFilesController(UnrealReplayServerContext context)
        {
            _context = context;
        }

        // GET: SessionFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.SessionFile.ToListAsync());
        }

        // GET: SessionFiles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionFile = await _context.SessionFile
                .FirstOrDefaultAsync(m => m.Filename == id);
            if (sessionFile == null)
            {
                return NotFound();
            }

            return View(sessionFile);
        }

        // GET: SessionFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SessionFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Filename,Data,StartTimeMs,EndTimeMs,ChunkIndex")] SessionFile sessionFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sessionFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sessionFile);
        }

        // GET: SessionFiles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionFile = await _context.SessionFile.FindAsync(id);
            if (sessionFile == null)
            {
                return NotFound();
            }
            return View(sessionFile);
        }

        // POST: SessionFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Filename,Data,StartTimeMs,EndTimeMs,ChunkIndex")] SessionFile sessionFile)
        {
            if (id != sessionFile.Filename)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessionFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionFileExists(sessionFile.Filename))
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
            return View(sessionFile);
        }

        // GET: SessionFiles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionFile = await _context.SessionFile
                .FirstOrDefaultAsync(m => m.Filename == id);
            if (sessionFile == null)
            {
                return NotFound();
            }

            return View(sessionFile);
        }

        // POST: SessionFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sessionFile = await _context.SessionFile.FindAsync(id);
            _context.SessionFile.Remove(sessionFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionFileExists(string id)
        {
            return _context.SessionFile.Any(e => e.Filename == id);
        }
    }
}
