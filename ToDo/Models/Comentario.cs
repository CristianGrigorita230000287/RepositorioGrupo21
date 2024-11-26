namespace ToDo.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public int TarefaId { get; set; }
        public string Comentarios { get; set; }
        public DateTime DataCriacao { get; set; }

    }
}
