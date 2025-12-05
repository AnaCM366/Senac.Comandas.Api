namespace Comanda.Api.DTOs
{
    public class PedidoCozinhaResponse
    {
        public int ComandaId { get; set; }
        public List<PedidoCozinhaItemResponse> Itens { get; set; } = [];
    }

    public class PedidoCozinhaItemResponse
    {
        public int ComandaItemId { get; set; }
    }
}
