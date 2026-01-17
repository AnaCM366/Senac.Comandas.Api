namespace Comanda.Api.DTOs
{
    public class CardapioItemCreateRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public bool PossuiPreparo { get; set; }
        public int? CategoriaCardapioId { get; set; }
    }
}
