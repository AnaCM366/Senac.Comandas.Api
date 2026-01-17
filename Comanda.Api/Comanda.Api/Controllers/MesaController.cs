using System.Runtime.CompilerServices;
using Comanda.Api.DTOs;
using Comanda.Api.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Comanda.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MesaController : ControllerBase
    {
        private readonly ComandasDBContext _context;
        public MesaController(ComandasDBContext context)
        {
            _context = context;
        }

        // GET: api/<MesaController>
        [HttpGet]
        public IResult GetMesa()
        {
            return Results.Ok(_context.Mesas.ToList());
        }

        // GET api/<MesaController>/5
        [HttpGet("{id}")]
        public IResult Get(int id)
        {
            var mesa = _context.Mesas.FirstOrDefault(m => m.Id == id);
            if (mesa == null)
            {
                return Results.NotFound("Mesa não encontrada!");
            }
            return Results.Ok(_context.Mesas);
        }

        // POST api/<MesaController>
        [HttpPost]
        public IResult Post([FromBody] MesaCreateRequest mesaCreate)
        {
            // Validações
            if (mesaCreate.NumeroMesa <= 0)
                return Results.BadRequest("O número da mesa deve ser maior que zero.");

            // Criar uma nova mesa
            var novaMesa = new Mesa
            {
                NumeroMesa = mesaCreate.NumeroMesa,
                SituacaoMesa = 0 
            };

            // Adiciona a nova mesa na lista
            _context.Mesas.Add(novaMesa);
            _context.SaveChanges();

            // Retorna a nova mesa criada e o codigo 201 CREATED
            return Results.Created($"/api/mesa/{novaMesa.Id}", novaMesa);
        }

        // PUT api/<MesaController>/5
        [HttpPut("{id}")]
        public IResult Put(int id, [FromBody] MesaUpdateRequest mesaUpdate)
        {
            // Localiza pelo ID
            var mesa = _context.Mesas.FirstOrDefault(m => m.Id == id);
            if (mesa is null)
                return Results.NotFound($"Mesa {id} não encontrada!");

            // Validações
            if (mesaUpdate.NumeroMesa <= 0)
                return Results.BadRequest("O número da mesa deve ser maior que zero.");
            if (mesaUpdate.SituacaoMesa < 0 || mesaUpdate.SituacaoMesa > 2)
                return Results.BadRequest("Situação da mesa inválida.");

            // Atualiza os dados
            mesa.NumeroMesa = mesaUpdate.NumeroMesa;
            mesa.SituacaoMesa = mesaUpdate.SituacaoMesa;

            // Retorno sem conteudo
            _context.SaveChanges();
            return Results.NoContent();
        }

        // DELETE api/<MesaController>/5
        [HttpDelete("{id}")]
        public IResult Delete(int id)
        {
            // Localiza pelo ID
            var mesa = _context.Mesas.FirstOrDefault(m => m.Id == id);

            // Retorna não encontrado se for null (404)
            if (mesa is null)
                return Results.NotFound($"Cardápio {id} não encontrado!");

            // Remove a mesa
            _context.Mesas.Remove(mesa);
            var mesaRemovida = _context.SaveChanges();

            // Retorna sem conteudo (204)
            if (mesaRemovida>0)
                return Results.NoContent();

            return Results.StatusCode(500);
        }
    }
}
