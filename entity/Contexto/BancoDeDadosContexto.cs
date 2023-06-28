using Microsoft.EntityFrameworkCore;
using entity.Entidades;
using Microsoft.Extensions.Configuration;
using System;

namespace entity.Contexto;

public class BancoDeDadosContexto : DbContext
{
    // Estratégia 3. Passando a dependencia via contrutor
    public BancoDeDadosContexto(DbContextOptions<BancoDeDadosContexto> options) : base(options)
    {

    }

    public BancoDeDadosContexto()
    {

    }
    //Estrategia 1 = vc pode criar a instância de onde estiver do seu contexto
    public DbSet<Cliente> Clientes { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();

            optionsBuilder.UseMySql(configuration.GetConnectionString("conexao"),
            new MySqlServerVersion(new Version(8, 0, 33)));

            //optionsBuilder.UseMySQL("server=localhost;database=aula_entity;uid=root;password=654321");
        }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Defina aqui as configurações específicas do modelo, como chaves primárias, restrições, etc.
    }
}
