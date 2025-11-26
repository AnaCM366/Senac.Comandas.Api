using Comanda.Api.DTOs;
using Comanda.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Comanda.Api.Controllers
{
    //CRIA A ROTA DO COTROLADOR
    [Route("api/[controller]")]
    [ApiController] // DEFINE QUE ESSA CLASSE É UM CONTROLADOR DE API
    public class CardapioItemController : ControllerBase // HERDA DE CONTROLLERBASE para PODER RESPONDER A REQUISICOES HTTP
    {
        public ComandasDBContext _context { get; set; }
        public CardapioItemController(ComandasDBContext context)
        {
            _context = context;
        }

        // METODO GET que retorna a lista de cardapio
        // GET: api/<CardapioItemController>
        [HttpGet] // ANOTACAO QUE INDICA QUE ESSE METODO RESPONDE A REQUISICOES GET
        public IResult GetCardapio()
        {
            var cardapios = _context.CardapioItems.Include(c => c.CategoriaCardapio).ToList();
            // CRIA UMA LISTA ESTATICA DE CARDAPIO E TRANSFORMA EM JSON
            return Results.Ok(cardapios);
        }


        // GET api/<CardapioItemController>/1
        [HttpGet("{id}")]
        public IResult Get(int id)
        {
            // BUSCAR NA LISTA de cardapios de acorod com o Id do parametro
            //joga o valor para a variavel o primeiro elemento de acordo com o id
            var cardapio = _context
                .CardapioItems
                .Include(ci => ci.CategoriaCardapio)
                .FirstOrDefault(c => c.Id == id);
            // SELECT * FROM CardapioItem
            // INNER JOIN CategoriaCardapio in cc.Id = c.CategoriaCardapioId
            // WHERE Id = 1

            if (cardapio is null)
            {
                return Results.NotFound("Cardápio não encontrado!");
            }
            // retorna o valor para o ednpoint da api
            return Results.Ok(cardapio);
        }

        // POST api/<CardapioItemController>
        [HttpPost]
        public IResult Post([FromBody] CardapioItemCreateRequest cardapio)
        {
            if (cardapio.Titulo.Length < 3)
                return Results.BadRequest("O título do item do cardápio deve ter no mínimo 3 caracteres.");
            if (cardapio.Descricao.Length < 5)
                return Results.BadRequest("A descrição do item do cardápio deve ter no mínimo 5 caracteres.");
            if (cardapio.Preco <= 0)
                return Results.BadRequest("O preço do item do cardápio deve ser maior que zero.");
            
            // validação da categoria se for preenchida
            if (cardapio.CategoriaCardapioId.HasValue)
            {
                var categoria = _context.CategoriaCardapio.FirstOrDefault(c => c.Id == cardapio.CategoriaCardapioId.Value);
                if (categoria is null)
                    return Results.BadRequest("Categoria de cardápio inválida.");
            }

            var cardapioItem = new CardapioItem
            {
                Titulo = cardapio.Titulo,
                Descricao = cardapio.Descricao,
                Preco = cardapio.Preco,
                PossuiPreparo = cardapio.PossuiPreparo,
                CategoriaCardapioId = cardapio.CategoriaCardapioId
            };
            _context.CardapioItems.Add(cardapioItem);
            _context.SaveChanges();

            return Results.Created($"/api/cardapioitem/{cardapioItem.Id}", cardapioItem);
        }

        // PUT api/<CardapioItemController>/5
        [HttpPut("{id}")]
        public IResult Put(int id, [FromBody] CardapioItemUpdateRequest cardapioUpdate)
        {
            var cardapioItem = _context.CardapioItems.FirstOrDefault(c => c.Id == id);
            if (cardapioItem is null)
                return Results.NotFound($"Cardápio {id} não encontrado!");
            
            // se categoria informada
            if (cardapioUpdate.CategoriaCardapioId.HasValue)
            {
                // consulta no banco pelo id da categoria
                var categoria = _context.CategoriaCardapio
                    .FirstOrDefault(c => c.Id == cardapioUpdate.CategoriaCardapioId);

                // se o retorno da consulta retornou nulo
                if (categoria is null)
                    return Results.BadRequest("Categoria de cardápio inválida.");
            }
            // Atualiza os dados do cardapio
            cardapioItem.Titulo = cardapioUpdate.Titulo;
            cardapioItem.Descricao = cardapioUpdate.Descricao;
            cardapioItem.Preco = cardapioUpdate.Preco;
            cardapioItem.PossuiPreparo = cardapioUpdate.PossuiPreparo;
            cardapioItem.CategoriaCardapioId = cardapioUpdate.CategoriaCardapioId;

            _context.SaveChanges();

            return Results.NoContent();
        }

        // DELETE api/<CardapioItemController>/5
        [HttpDelete("{id}")]
        public IResult Delete(int id)
        {
            // buscar o cardapio na lista pelo id
            var cardapioItem = _context.CardapioItems.FirstOrDefault(c => c.Id == id);
            
            // se estiver nulo, retorna 404
            if (cardapioItem is null)
                return Results.NotFound($"Cardápio {id} não encontrado!");
            
            // remove o cardapio da lista
            _context.CardapioItems.Remove(cardapioItem);
            var removidoComSucesso = _context.SaveChanges();

            // retorna 204 sem conteudo
            if (removidoComSucesso>0)
                return Results.NoContent();

            return Results.StatusCode(500);
        }
    }
}
