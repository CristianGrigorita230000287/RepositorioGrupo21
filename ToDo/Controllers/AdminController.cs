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

        public async Task<IActionResult> GerirUtilizadores()
        {
            var utilizadores = await _context.Utilizador.ToListAsync();
            return View(utilizadores);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var utilizador = await _userManager.FindByIdAsync(id);
            if (utilizador != null)
            {
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
