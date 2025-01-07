using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;
using ToDo.ViewModels;

namespace ToDo.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        private readonly ToDoContext _context;
        private readonly UserManager<Utilizador> _userManager;

        public AdminController(ToDoContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            var totalUtilizadores = _context.Utilizador.Count();
            var totalTarefas = _context.Tarefa.Count();
            var tarefasCompletadas = _context.Tarefa.Count(t => t.Estado == "Concluída");

            var model = new DashboardViewModel
            {
                TotalUtilizadores = totalUtilizadores,
                TotalTarefas = totalTarefas,
                TarefasCompletadas = tarefasCompletadas
            };

            return View(model);
        }

        public async Task<IActionResult> GerirUtilizadores(string sortOrder)
        {
            var utilizadores = await _context.Utilizador.ToListAsync();
            var adminEmail = "admin@geral.com";

            var utilizadorViewModels = utilizadores
                .Where(u => u.Email != adminEmail)
                .Select(u => new UtilizadorViewModel
            {
                Id = u.Id,
                Nome = $"{u.PrimeiroNome} {u.Apelido}",
                Email = u.Email,
                TotalTarefas = _context.Tarefa.Count(t => t.UtilizadorId == u.Id),
                TarefasConcluidas = _context.Tarefa.Count(t => t.UtilizadorId == u.Id && t.Estado == "Concluída"),
                TarefasPendentes = _context.Tarefa.Count(t => t.UtilizadorId == u.Id && t.Estado == "Pendente"),
                DataCriacao = u.DataCriacao,
                UltimoLogin = u.UltimoLogin,
                DiasSemLogin = (DateTime.Now - u.UltimoLogin).Days
            }).ToList();

            // Ordenação:
            ViewBag.NomeSortParm = String.IsNullOrEmpty(sortOrder) ? "nome_desc" : "";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.TotalTarefasSortParm = sortOrder == "TotalTarefas" ? "totalTarefas_desc" : "TotalTarefas";
            ViewBag.TarefasConcluidasSortParm = sortOrder == "TarefasConcluidas" ? "tarefasConcluidas_desc" : "TarefasConcluidas";
            ViewBag.TarefasPendentesSortParm = sortOrder == "TarefasPendentes" ? "tarefasPendentes_desc" : "TarefasPendentes";
            ViewBag.DataCriacaoSortParm = sortOrder == "DataCriacao" ? "dataCriacao_desc" : "DataCriacao";
            ViewBag.UltimoLoginSortParm = sortOrder == "UltimoLogin" ? "ultimoLogin_desc" : "UltimoLogin";
            ViewBag.DiasSemLoginSortParm = sortOrder == "DiasSemLogin" ? "diasSemLogin_desc" : "DiasSemLogin";

            utilizadorViewModels = sortOrder switch
            {
                "nome_desc" => utilizadorViewModels.OrderByDescending(u => u.Nome).ToList(),
                "Email" => utilizadorViewModels.OrderBy(u => u.Email).ToList(),
                "email_desc" => utilizadorViewModels.OrderByDescending(u => u.Email).ToList(),
                "TotalTarefas" => utilizadorViewModels.OrderBy(u => u.TotalTarefas).ToList(),
                "totalTarefas_desc" => utilizadorViewModels.OrderByDescending(u => u.TotalTarefas).ToList(),
                "TarefasConcluidas" => utilizadorViewModels.OrderBy(u => u.TarefasConcluidas).ToList(),
                "tarefasConcluidas_desc" => utilizadorViewModels.OrderByDescending(u => u.TarefasConcluidas).ToList(),
                "TarefasPendentes" => utilizadorViewModels.OrderBy(u => u.TarefasPendentes).ToList(),
                "tarefasPendentes_desc" => utilizadorViewModels.OrderByDescending(u => u.TarefasPendentes).ToList(),
                "DataCriacao" => utilizadorViewModels.OrderBy(u => u.DataCriacao).ToList(),
                "dataCriacao_desc" => utilizadorViewModels.OrderByDescending(u => u.DataCriacao).ToList(),
                "UltimoLogin" => utilizadorViewModels.OrderBy(u => u.UltimoLogin).ToList(),
                "ultimoLogin_desc" => utilizadorViewModels.OrderByDescending(u => u.UltimoLogin).ToList(),
                "DiasSemLogin" => utilizadorViewModels.OrderBy(u => u.DiasSemLogin).ToList(),
                "diasSemLogin_desc" => utilizadorViewModels.OrderByDescending(u => u.DiasSemLogin).ToList(),
                _ => utilizadorViewModels.OrderBy(u => u.Nome).ToList(),
            };

            return View(utilizadorViewModels);
        }

        // GET: Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            var totalTarefas = await _context.Tarefa.CountAsync(t => t.UtilizadorId == id);
            var tarefasCompletadas = await _context.Tarefa.CountAsync(t => t.UtilizadorId == id && t.Estado == "Concluída");
            var tarefasPendentes = totalTarefas - tarefasCompletadas;

            ViewBag.TotalTarefas = totalTarefas;
            ViewBag.TarefasCompletadas = tarefasCompletadas;
            ViewBag.TarefasPendentes = tarefasPendentes;

            return View(utilizador);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var utilizador = await _userManager.FindByIdAsync(id);
            if (utilizador != null)
            {
                // Excluir tarefas associadas ao usuário:
                var tarefas = _context.Tarefa.Where(t => t.UtilizadorId == id);
                _context.Tarefa.RemoveRange(tarefas);

                // Excluir categorias associadas ao usuário:
                var categorias = _context.Categoria.Where(c => c.UtilizadorId == id);
                _context.Categoria.RemoveRange(categorias);

                // Salvar alterações no banco de dados:
                await _context.SaveChangesAsync();

                // Excluir o usuário:
                var result = await _userManager.DeleteAsync(utilizador);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(GerirUtilizadores));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return RedirectToAction(nameof(GerirUtilizadores));
        }
    }
}
