using entity.Contexto;
using Microsoft.EntityFrameworkCore;
using entity.Entidades;
using Microsoft.AspNetCore.Mvc;
using entity.ModelViews;

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
    //return Results.NotFound(new { Mensagem = "Não encontrado" });
    //return Results.NoContent();
    return contexto.Clientes.ToList();
})
.WithName("Hom")
.WithOpenApi();

app.MapGet("/clientes-com-pedidos", ([FromServices] BancoDeDadosContexto contexto, [FromQuery] int? page) =>
{
    //return contexto.Pedidos.ToList(); //Não traz a instancia de cliente;
    /*return contexto.Pedidos
        .Include(p => p.Cliente).ToList();//traz tudo de pedidos e cliente*/
    /*return contexto.Pedidos
        .Include(p => p.Cliente)
                .Select(p => new
                {
                    cli_nome = p.Cliente.Nome,
                    cli_telefone = p.Cliente.Telefone,
                    ValorTotal = p.ValorTotal
                }).ToList();*/

    var totalPage = 2;
    if (page == null || page < 1)
    {
        page = 1;
    }
    int offset = ((int)page - 1) * totalPage;

    //JOIN SIMPLES
    /*var relatorioEntity = contexto.Pedidos
        .Include(p => p.Cliente)
                .Select(p => new PedidoCliente
                {
                    Nome = p.Cliente.Nome,
                    Telefone = p.Cliente.Telefone,
                    ValorTotal = p.ValorTotal
                });*/

    //QUERY MAIS COMPLEXA - SEM GROUP BY
    /*var relatorioEntity = contexto.Pedidos
        .Include(p => p.Cliente)    //Não preciso fazer join pois em Pedido existe cliente
        .Join(
            contexto.PedidosProdutos,
            pedido => pedido.Id,                        //Representante do lado da esquerda
            pedidoProduto => pedidoProduto.PedidoId,    //Representante do lado da direita
            (pedido, pedidoProduto) => new
            {
                Pedido = pedido,
                PedidoProduto = pedidoProduto
            }
        )
        .Join(
            contexto.produtos,
            p => p.PedidoProduto.ProdutoId,
            produto => produto.Id,
            (p, produto) => new PedidoCliente
            {
                PedidoId = p.Pedido.Id,
                Nome = p.Pedido.Cliente.Nome,
                Telefone = p.Pedido.Cliente.Telefone,
                ValorTotal = p.Pedido.ValorTotal,
                NomeProduto = produto.Nome,
                QuantidadeVendidaParaProduto = p.PedidoProduto.Quantidade,
                ValorVendidoParaProduto = p.PedidoProduto.Valor
            }
    );*/

    // QUERY COM GROUP BY
    /*var relatorioEntity = contexto.Pedidos
        .Join
        (
            contexto.PedidosProdutos,
            pedido => pedido.Id,                        //Representante do lado da esquerda
            pedidoProduto => pedidoProduto.PedidoId,    //Representante do lado da direita
            (pedido, pedidoProduto) => new
            {
                Pedido = pedido,
                PedidoProduto = pedidoProduto
            }
        )
        .Join
        (
            contexto.produtos,
            p => p.PedidoProduto.ProdutoId,
            produto => produto.Id,
            (p, produto) => new PedidoCliente
            {
                PedidoId = p.Pedido.Id,
                Nome = p.Pedido.Cliente.Nome,
                Telefone = p.Pedido.Cliente.Telefone,
                ValorTotal = p.Pedido.ValorTotal,
                NomeProduto = produto.Nome,
                QuantidadeVendidaParaProduto = p.PedidoProduto.Quantidade,
                ValorVendidoParaProduto = p.PedidoProduto.Valor
            }
        ).GroupBy(p => new { p.PedidoId, p.Nome, p.Telefone, p.ValorTotal })
        .Select(g => new PedidoClienteSomadas
        {
            PedidoId = g.Key.PedidoId,
            Nome = g.Key.Nome,
            Telefone = g.Key.Telefone,
            ValorTotal = g.Key.ValorTotal,
            QuantidadeSomadaProduto = g.Sum(p => p.QuantidadeVendidaParaProduto),
            ValorSomadoProduto = g.Sum(p => p.ValorVendidoParaProduto)
        });
    */
    //AGORA EXEMPLO COM LINQTOSQL
    var relatorioEntity = (from pedido in contexto.Pedidos
                           join pedidoProduto in contexto.PedidosProdutos on pedido.Id equals pedidoProduto.PedidoId
                           join produto in contexto.Produtos on pedidoProduto.ProdutoId equals produto.Id
                           select new PedidoCliente
                           {
                               PedidoId = pedido.Id,
                               Nome = pedido.Cliente.Nome,
                               Telefone = pedido.Cliente.Telefone,
                               ValorTotal = pedido.ValorTotal,
                               NomeProduto = produto.Nome,
                               QuantidadeVendidaParaProduto = pedidoProduto.Quantidade,
                               ValorVendidoParaProduto = pedidoProduto.Valor
                           })
        .GroupBy(p => new { p.PedidoId, p.Nome, p.Telefone, p.ValorTotal })
        .Select(g => new PedidoClienteSomadas
        {
            PedidoId = g.Key.PedidoId,
            Nome = g.Key.Nome,
            Telefone = g.Key.Telefone,
            ValorTotal = g.Key.ValorTotal,
            QuantidadeSomadaProduto = g.Sum(p => p.QuantidadeVendidaParaProduto),
            ValorSomadoProduto = g.Sum(p => p.ValorVendidoParaProduto)
        });

    var lista = relatorioEntity
                .Skip(offset) // Offsetdefine o número de registros a serem ignorados
                .Take(totalPage) // limitador
                .ToList();

    return new RegistroPaginado<PedidoClienteSomadas>
    {
        Registros = lista,
        TotalPorPagina = totalPage,
        PaginaCorrente = (int)page,
        TotalRegistros = relatorioEntity.Count()
    };
})
.WithName("ClientesComPedidos")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
