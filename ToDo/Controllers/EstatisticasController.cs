using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Controllers
{
    public class EstatisticasController : Controller
    {
        private readonly ToDoContext _context;

        public EstatisticasController(ToDoContext context)
        {
            _context = context;
        }

        // GET: Estatisticas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Estatistica.ToListAsync());
        }

        // GET: Estatisticas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatistica = await _context.Estatistica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estatistica == null)
            {
                return NotFound();
            }

            return View(estatistica);
        }

        // GET: Estatisticas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estatisticas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UtilizadorId,TarefaId,DataAtualizacao")] Estatistica estatistica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estatistica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estatistica);
        }

        // GET: Estatisticas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica == null)
            {
                return NotFound();
            }
            return View(estatistica);
        }

        // POST: Estatisticas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UtilizadorId,TarefaId,DataAtualizacao")] Estatistica estatistica)
        {
            if (id != estatistica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estatistica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstatisticaExists(estatistica.Id))
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
            return View(estatistica);
        }

        // GET: Estatisticas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatistica = await _context.Estatistica
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estatistica == null)
            {
                return NotFound();
            }

            return View(estatistica);
        }

        // POST: Estatisticas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica != null)
            {
                _context.Estatistica.Remove(estatistica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstatisticaExists(int id)
        {
            return _context.Estatistica.Any(e => e.Id == id);
        }
    }
}
