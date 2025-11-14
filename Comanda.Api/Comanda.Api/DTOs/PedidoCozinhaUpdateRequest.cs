namespace Comanda.Api.DTOs
{
    public class PedidoCozinhaUpdateRequest
    {
        public int Id { get; set; }
        public int ComandaId { get; set; }
        public List<PedidoCozinhaItemUpdateRequest> Itens { get; set; } = [];
    }

    public class PedidoCozinhaItemUpdateRequest
    {
        public int Id { get; set; }
        public int PedidoCozinhaId { get; set; }
        public int ComandaItemId { get; set; }
    }
}
