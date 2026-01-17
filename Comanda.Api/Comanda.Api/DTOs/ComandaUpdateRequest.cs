namespace Comanda.Api.DTOs
{
    public class ComandaUpdateRequest
    {
        public string NomeCliente { get; set; }
        public int NumeroMesa { get; set; }

        public ComandaItemUpdateRequest[] Itens { get; set; } = [];
    }

    public class ComandaItemUpdateRequest
    {
        public int Id { get; set; }  // id da comanda item
        public bool Remove { get; set; } // indicar se esta removendo
        public int CardapioItemId { get; set; } // indicar se esta inserido
    }
}
