using entity.Contexto;
using Microsoft.EntityFrameworkCore;
using entity.Entidades;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

//USO DA ESTRATÉGIA 1
//var contexto = new BancoDeDadosContexto();

/*/// EXEMPLO INSERT
contexto.Clientes.Add(new Cliente
{
    Nome = "Josiane",
    Telefone = "31995687447"
});

contexto.SaveChanges();*/
/*
// EXEMPLO LISTA DE DADOS
var clientes = contexto.Clientes.ToList();
Console.WriteLine(clientes);
var clientes = contexto.Clientes.Find(1);
*/

// EXEMPLO DE UPDATE DADOS
/*var cliente = contexto.Clientes.First();
cliente.Nome = "Pedro";
contexto.Update(cliente);
contexto.SaveChanges();
*/

// Exemplo de deletar
/*var cliente = contexto.Clientes.First();
contexto.Remove(cliente);
contexto.SaveChanges();
*/
/*
contexto.Remove(new Cliente{Id = 1});
contexto.SaveChanges();
*/

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Estratégia 2. Por injeção de dependência
builder.Services.AddDbContext<BancoDeDadosContexto>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("conexao"),
    new MySqlServerVersion(new Version(8, 0, 33)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherforecast")
.WithOpenApi();


app.MapGet("/", ([FromServices] BancoDeDadosContexto contexto) =>
{
    return contexto.Clientes.ToList();
})
.WithName("Hom")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
