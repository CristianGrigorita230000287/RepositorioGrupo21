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
    public class TarefasController : Controller
    {
        private readonly ToDoContext _context;

        public TarefasController(ToDoContext context)
        {
            _context = context;
        }

        // GET: Tarefas
        public async Task<IActionResult> Index()
        {
            var tarefas = await _context.Tarefa
                .Include(t => t.Categoria) // Inclui a categoria relacionada
                .ToListAsync();
            return View(tarefas);
        }

        // GET: Tarefas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // GET: Tarefas/Create
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_context.Categoria, "Id", "Nome");
            return View();
        }

        // POST: Tarefas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UtilizadorId,Nome,Prioridade,Estado,CategoriaId,DataCriacao,DataLimite")] Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarefa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recarregar categorias em caso de erro de validação
            ViewBag.Categorias = new SelectList(_context.Categoria, "Id", "Nome");
            return View(tarefa);
        }

        // GET: Tarefas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();
            }

            // Preencher o ViewBag com as categorias disponíveis
            ViewBag.Categorias = new SelectList(_context.Categoria, "Id", "Nome", tarefa.CategoriaId);

            return View(tarefa);
        }

        // POST: Tarefas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UtilizadorId,Nome,Prioridade,Estado,CategoriaId,DataCriacao,DataLimite")] Tarefa tarefa)
        {
            if (id != tarefa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarefa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarefaExists(tarefa.Id))
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

            // Recarregar o ViewBag caso haja erro de validação
            ViewBag.Categorias = new SelectList(_context.Categoria, "Id", "Nome", tarefa.CategoriaId);

            return View(tarefa);
        }

        // GET: Tarefas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: Tarefas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa != null)
            {
                _context.Tarefa.Remove(tarefa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarefaExists(int id)
        {
            return _context.Tarefa.Any(e => e.Id == id);
        }
    }
}
