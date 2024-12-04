using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDo.Models;

namespace ToDo.Data
{
    public class ToDoContext : DbContext
    {
        public ToDoContext (DbContextOptions<ToDoContext> options)
            : base(options)
        {
        }

        public DbSet<ToDo.Models.Tarefa> Tarefa { get; set; } = default!;
        public DbSet<ToDo.Models.Comentario> Comentario { get; set; } = default!;
        public DbSet<ToDo.Models.Categoria> Categoria { get; set; } = default!;
        public DbSet<ToDo.Models.Historia> Historia { get; set; } = default!;
        public DbSet<ToDo.Models.Estatistica> Estatistica { get; set; } = default!;
        public DbSet<ToDo.Models.Utilizador> Utilizador { get; set; } = default!;
    }
}
