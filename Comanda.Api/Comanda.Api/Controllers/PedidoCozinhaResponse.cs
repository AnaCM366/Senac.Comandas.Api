namespace Comanda.Api.Controllers
{
    public class PedidoCozinhaResponse
    {
        public int Id { get; set; }
        public int ComandaId { get; set; }
        public IEnumerable<PedidoCozinhaItemResponse> Itens { get; internal set; }
    }
}
