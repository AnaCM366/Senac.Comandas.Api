using Comanda.Api.DTOs;
using Comanda.Api.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Comanda.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComandaController : ControllerBase
    {
        private readonly ComandasDBContext _context;
        public ComandaController(ComandasDBContext context)
        {
            _context = context;
        }

        // GET: api/<ComandaController>
        [HttpGet]
        public IResult Get()
        {
            var comandas = _context.Comandas
                .Select(c => new ComandaCreateResponse
                {
                    Id = c.Id,
                    NomeCliente = c.NomeCliente,
                    NumeroMesa = c.NumeroMesa,
                    Itens = c.Itens.Select(i => new ComandaItemResponse
                    {
                        Id = i.Id,
                        Titulo = _context.CardapioItems
                        .First(ci => ci.Id == i.CardapioItemId).Titulo
                    }).ToList()
                }).ToList();


            return Results.Ok(comandas);
        }

        // GET api/<ComandaController>/5
        [HttpGet("{id}")]
        public IResult Get(int id)
        {
            var comanda = _context.Comandas
                .Select(c => new ComandaCreateResponse
                {
                    Id = c.Id,
                    NomeCliente = c.NomeCliente,
                    NumeroMesa = c.NumeroMesa,
                    Itens = c.Itens.Select(i => new ComandaItemResponse
                    {
                        Id = i.Id,
                        Titulo = _context.CardapioItems
                    .First(ci => ci.Id == i.CardapioItemId).Titulo
                    }).ToList()
                 })
                .FirstOrDefault(c => c.Id == id);
            if (comanda is null)
                return Results.NotFound("Comanda não encontrada");

            return Results.Ok(comanda);
        }

        // POST api/<ComandaController>
        [HttpPost]
        public IResult Post([FromBody] ComandaCreateRequest comandaCreate)
        {
            if (comandaCreate.NomeCliente.Length < 3)
                return Results.BadRequest("O nome do cliente deve ter no mínimo 3 caracteres.");
            if (comandaCreate.NumeroMesa < 1)
                return Results.BadRequest("O número da mesa deve ser maior que zero.");
            if (comandaCreate.CardapioItemsIds.Length == 0)
                return Results.BadRequest("A comanda deve ter pelo menos um item do cardápio.");
            var novacomanda = new Models.Comanda
            {
                NomeCliente = comandaCreate.NomeCliente,
                NumeroMesa = comandaCreate.NumeroMesa
            };

            var mesa = _context.Mesas
                .FirstOrDefault(m => m.NumeroMesa == comandaCreate.NumeroMesa);
            mesa.SituacaoMesa = 1;

            // cria uma variavel do tipo lista de itens
            var comandaItens = new List<ComandaItem>();
            // percorre os ids dos itens do cardapio
            foreach (int cardapioItemId in comandaCreate.CardapioItemsIds)
            {
                // cria um novo item da comanda
                var comandaItem = new ComandaItem
                {
                    CardapioItemId = cardapioItemId,
                    Comanda = novacomanda
                };
                // adiciona o itens na lista de itens   
                comandaItens.Add(comandaItem);

                var cardapioItem = _context.CardapioItems
                    .FirstOrDefault(c => c.Id == cardapioItemId);
                if (cardapioItem!.PossuiPreparo)
                {
                    var pedido = new PedidoCozinha
                    {
                        Comanda = novacomanda,
                    };

                    var pedidoItem = new PedidoCozinhaItem
                    {
                        ComandaItem = comandaItem,
                        PedidoCozinha = pedido,
                    };
                    _context.PedidoCozinhas.Add(pedido);
                    _context.PedidoCozinhaItems.Add(pedidoItem);
                }
            }

            // atribui os itens a nota comanda
             novacomanda.Itens = comandaItens;
            _context.Comandas.Add(novacomanda);
            _context.SaveChanges();

            var response = new ComandaCreateResponse
            {
                Id = novacomanda.Id,
                NomeCliente = novacomanda.NomeCliente,
                NumeroMesa = novacomanda.NumeroMesa,
                Itens = novacomanda.Itens.Select(i => new ComandaItemResponse
                {
                    Id = i.Id,
                    Titulo = _context.CardapioItems.First(ci => ci.Id == i.CardapioItemId).Titulo
                }).ToList()
            };

            // adiciona a nova comanda na lista de comandas
            return Results.Created($"/api/comanda/{response.Id}", response);
        }

        // PUT api/<ComandaController>/5
        [HttpPut("{id}")]
        public IResult Put(int id, [FromBody] ComandaUpdateRequest comandaUpdate)
        {
            // pesquisa uma comanda na lista de comandas pelo id da comanda que veio no parametro da request
            var comanda = _context.Comandas.FirstOrDefault(c => c.Id == id);

            // validar o nome do cliente
            if (comandaUpdate.NomeCliente.Length < 3)
                return Results.BadRequest("O nome do cliente deve ter no mínimo 3 caracteres.");
            // validar o numero da mesa
            if (comandaUpdate.NumeroMesa < 1)
                return Results.BadRequest("O número da mesa deve ser maior que zero.");
            if (comanda is null) // se não encontrou a comanda pesquisada
            // retorna um codigo 404 Não encontrado
                return Results.NotFound($"Comanda {id} não encontrada");
            
            // Atualia os dados da comanda
            comanda.NomeCliente = comandaUpdate.NomeCliente;
            comanda.NumeroMesa = comandaUpdate.NumeroMesa;

            // percorrendo a lista de itens
            foreach(var itemUpdate in comandaUpdate.Itens)
            {
                // se id for informado e remover for verdadeiro
                if(itemUpdate.Id >0 && itemUpdate.Remove == true)
                {
                    RemoverItemComanda(itemUpdate.Id);
                }
                // se cardaitemid foi informado
                if (itemUpdate.CardapioItemId >0)
                {
                    InserirItemComanda(comanda, itemUpdate.CardapioItemId);
                }
            }

            _context.SaveChanges();

            // retorna 204 Sem conteudo
            return Results.NoContent();
        }

        private void InserirItemComanda(Models.Comanda comanda, int cardapioItemId)
        {
            _context.ComandaItems.Add(
                new ComandaItem
                {
                    CardapioItemId = cardapioItemId,
                    Comanda = comanda
                }
            );
        }

        private void RemoverItemComanda(int id)
        {
            // consulta o item da comanda pelo id
            var comandaItem = _context.ComandaItems.FirstOrDefault(ci => ci.Id == id);
            if (comandaItem is not null)
            {
                // remove o item da comanda
                _context.ComandaItems.Remove(comandaItem);
            }
        }

        // DELETE api/<ComandaController>/5
        [HttpDelete("{id}")]
        public IResult Delete(int id)
        {
            var comanda = _context.Comandas
                .FirstOrDefault(c => c.Id == id);

            if (comanda is null)
                return Results.NotFound($"Comanda {id} não encontrada");

            _context.Comandas.Remove(comanda);
            var comandaRemovida = _context.SaveChanges();

            if (comandaRemovida>0)
                return Results.NoContent();
            
            return Results.StatusCode(500);
        }
    }
}
