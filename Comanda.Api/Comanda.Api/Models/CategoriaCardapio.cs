using System.ComponentModel.DataAnnotations;

namespace Comanda.Api.Models
{
    public class CategoriaCardapio
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = default!;
        public string? Descricao { get; set; } = default!;
        public ICollection<CardapioItem>? Itens { get; set; }
    }
}
