using Comanda.Api.DTOs;
using Comanda.Api.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Comanda.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        // variavel que representa o banco de dados
        public ComandasDBContext _context { get; set; }
        public UsuarioController(ComandasDBContext context)
        {
            _context = context;
        }

        // GET: api/<UsuarioController>
        [HttpGet]
        public IResult Get()
        {
            var usuarios = _context.Usuarios.ToList();
            return Results.Ok(usuarios);
        }

        // GET api/<UsuarioController>/5
        [HttpGet("{id}")]
        public IResult Get(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if(usuario is null)
                return Results.NotFound("Usuário não encontrado!");

            return Results.Ok(usuario);
        }

        // POST api/<UsuarioController>
        [HttpPost]
        public IResult Post([FromBody] UsuarioCreateRequest usuarioCreate)
        {
            if (usuarioCreate.Senha.Length < 6)
                return Results.BadRequest("A senha deve ter no mínimo 6 caracteres.");
            if (usuarioCreate.Nome.Length < 3)
                return Results.BadRequest("O nome deve ter no mínimo 3 caracteres.");
            if (usuarioCreate.Email.Length < 5 || !usuarioCreate.Email.Contains("@"))
                return Results.BadRequest("O email deve ser válido.");
            
            var emailExistente = _context.Usuarios
                .FirstOrDefault(u => u.Email == usuarioCreate.Email);
            if (emailExistente is not null)
                return Results.BadRequest("O email já está em uso.");

            var usuario = new Usuario
            {
                Nome = usuarioCreate.Nome,
                Email = usuarioCreate.Email,
                Senha = usuarioCreate.Senha
            };
            // adicionar o usuário na lista
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Results.Created($"/api/usuario/{usuario.Id }", usuario);

        }

        // PUT api/<UsuarioController>/5
        /// <summary>
        /// ATUALIZA UM USUÁRIO
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <param name="usuarioUpdate">Dados do usuário </param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IResult Put(int id, [FromBody] UsuarioUpdateRequest usuarioUpdate)
        {
            if (usuarioUpdate.Senha.Length < 6)
                return Results.BadRequest("A senha deve ter no mínimo 6 caracteres.");

            // se não encontrar retorno notfound
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario is null)
                return Results.NotFound($"Usuário do id {id} não encontrado.");
            // atualiza o usuário
            usuario.Nome = usuarioUpdate.Nome;
            usuario.Email = usuarioUpdate.Email;
            usuario.Senha = usuarioUpdate.Senha;


            _context.SaveChanges();

            // retorno no content
            return Results.NoContent();
        }

        // DELETE api/<UsuarioController>/5
        [HttpDelete("{id}")]
        public IResult Delete(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario is null)
                return Results.NotFound($"Usuário {id} não encontrado!");

            _context.Usuarios.Remove(usuario);
            var removido = _context.SaveChanges();

            if (removido>0)
                return Results.NoContent();

            return Results.StatusCode(500);
        }

        // criar metodo de login
        // POST api/usuario/login
        // { "email": "rafael@hotmail.com", "senha": "123" }
        [HttpPost("login")]
        public IResult Login([FromBody] LoginRequest loginRequest)
        {
            var usuario = _context.Usuarios
                 .FirstOrDefault(u =>
                    u.Email == loginRequest.email
                 && u.Senha == loginRequest.senha);

            // 401
            if (usuario is null)
                return Results.Unauthorized();
            // 200
            return Results.Ok("Usuário autenticado");
        }
    }
}
