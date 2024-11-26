namespace ToDo.Models
{
    public class Utilizador
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string UtilizadorPassword { get; set; }
        public int UtilizadorAdmin {  get; set; }
        public DateTime UltimoLogin { get; set; }
        public DateTime DataCriacao { get; set; }

    }
}
