using Comanda.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Configurar o contexto do banco de dados para usar o InMemoryDatabase
builder.Services.AddDbContext<ComandasDBContext>(options =>
    options.UseSqlite("DataSource=comandas.db")
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MinhaPolitica", policy =>
    {
        policy.WithOrigins("http://localhost", "http://127.0.0.1:5500", "http://127.0.0.1", "http://127.0.0.1:5501") // Origens permitidas
        .AllowAnyHeader() // Permite qualquer cabeçalho
        .AllowAnyMethod(); // Permite qualquer método HTTP
    });
});

var app = builder.Build();

// criar o banco de dados
// criar um escopo usado para obter instancias de variaveis
using(var scope = app.Services.CreateScope())
{
    // obtem um objeto do banco de dados 
    var db = scope.ServiceProvider.GetRequiredService<ComandasDBContext>();
    // executa  as migrations no banco de dados
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseCors("MinhaPolitica");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
