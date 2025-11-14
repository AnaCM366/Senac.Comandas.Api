namespace Comanda.Api.DTOs
{
    public class PedidoCozinhaCreateRequest
    {
        public int ComandaId { get; set; }
        public List<PedidoCozinhaItemCreateRequest> Itens { get; set; } = [];
    }

    public class PedidoCozinhaItemCreateRequest
    {
        public int ComandaItemId { get; set; }
    }
}
