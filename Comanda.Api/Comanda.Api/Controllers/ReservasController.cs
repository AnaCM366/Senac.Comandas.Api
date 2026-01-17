using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comanda.Api;
using Comanda.Api.Models;
using SQLitePCL;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.CodeAnalysis.Operations;

namespace Comanda.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ComandasDBContext _context;

        public ReservasController(ComandasDBContext context)
        {
            _context = context;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas.ToListAsync();
        }

        // GET: api/Reservas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return reserva;
        }

        // PUT: api/Reservas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserva(int id, Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return BadRequest();
            }

            // atualização
            _context.Entry(reserva).State = EntityState.Modified;

            // mudar a situacao para livre da mesa original
            // mudar a situacao para reservada da nova mesa
            // remocao e inclusao da reserva na mesa (2 - reservada - 1.livre)
            // 2 - livre 1 - reservada
            // 
            // ---------------
            var novaMesa = await _context.Mesas.FirstOrDefaultAsync(m => m.NumeroMesa == reserva.NumeroMesa);
            if (novaMesa is null)
                return BadRequest("Mesa não encontrada.");
            novaMesa.SituacaoMesa = (int)SituacaoMesa.Reservado; // novaMesa agora está reservada

            // mudar a situacao para livre da mesa original
            var reservaOriginal = await _context.Reservas.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            // consulta o numero da mesa original
            var numeroMesaOriginal = reservaOriginal!.NumeroMesa;
            // consulta a mesa original
            var mesaOriginal = await _context.Mesas
                .FirstOrDefaultAsync(m => m.NumeroMesa == numeroMesaOriginal);
            mesaOriginal!.SituacaoMesa = (int)SituacaoMesa.Livre; // mesa original agora está livre
            // ---------------------

            try
            {
                // salva
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reservas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);

            // Atualiza o status da mesa para "Reservado" 
            var mesa = await _context.Mesas
                .FirstOrDefaultAsync(m => m.NumeroMesa == reserva.NumeroMesa);
            
            if (mesa is null)
                return BadRequest("Mesa não encontrada.");

            // se mesa encontrada
            if (mesa is not null)
            {
                if(mesa.SituacaoMesa != (int)SituacaoMesa.Livre)
                {
                    return BadRequest("Mesa não está disponível para reserva.");
                }
                // atualizar o status da mesa para reservado
                mesa.SituacaoMesa = (int)SituacaoMesa.Reservado;
            }
            // -----------------
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReserva", new { id = reserva.Id }, reserva);
        }

        // DELETE: api/Reservas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            // -----------------------------
            // consultar a mesa
            var mesa = await _context.Mesas
                .FirstOrDefaultAsync(m => m.NumeroMesa == reserva.NumeroMesa);
            if (mesa is null)
            {
                return BadRequest("Mesa não encontrada.");
            }

            // atualizar a mesa para livre
            mesa.SituacaoMesa = (int)SituacaoMesa.Livre; // (int) converte o enum para int
            // -----------------------------
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
