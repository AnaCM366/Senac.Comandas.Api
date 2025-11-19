using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comanda.Api;
using Comanda.Api.Models;

namespace Comanda.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaCardapiosController : ControllerBase
    {
        private readonly ComandasDBContext _context;

        public CategoriaCardapiosController(ComandasDBContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaCardapios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaCardapio>>> GetCategoriaCardapio()
        {
            return await _context.CategoriaCardapio.ToListAsync();
        }

        // GET: api/CategoriaCardapios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaCardapio>> GetCategoriaCardapio(int id)
        {
            var categoriaCardapio = await _context.CategoriaCardapio.FindAsync(id);

            if (categoriaCardapio == null)
            {
                return NotFound();
            }

            return categoriaCardapio;
        }

        // PUT: api/CategoriaCardapios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaCardapio(int id, CategoriaCardapio categoriaCardapio)
        {
            if (id != categoriaCardapio.Id)
            {
                return BadRequest();
            }

            _context.Entry(categoriaCardapio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaCardapioExists(id))
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

        // POST: api/CategoriaCardapios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CategoriaCardapio>> PostCategoriaCardapio(CategoriaCardapio categoriaCardapio)
        {
            _context.CategoriaCardapio.Add(categoriaCardapio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoriaCardapio", new { id = categoriaCardapio.Id }, categoriaCardapio);
        }

        // DELETE: api/CategoriaCardapios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaCardapio(int id)
        {
            var categoriaCardapio = await _context.CategoriaCardapio.FindAsync(id);
            if (categoriaCardapio == null)
            {
                return NotFound();
            }

            _context.CategoriaCardapio.Remove(categoriaCardapio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriaCardapioExists(int id)
        {
            return _context.CategoriaCardapio.Any(e => e.Id == id);
        }
    }
}
