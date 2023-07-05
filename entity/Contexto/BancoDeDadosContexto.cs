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
    public DbSet<Fornecedor> Fornecedores { get; set; } = default!;

    //Para criar a migration dotnet ef migrations add CriandoFornecedor
    //Para aplicar a migration dotnet ef database update
    //No exemplo eu criei o start.sh para rodar

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

    // forma 2 de anotacao
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region "Clientes"
        // Defina aqui as configurações específicas do modelo, como chaves primárias, restrições, etc.
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("tb_clientes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("cli_id")
                .ValueGeneratedOnAdd().UseMySqlIdentityColumn();
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("cli_nome");
            entity.Property(e => e.Telefone)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("cli_telefone")
                .HasComment("Este é o número do telefone do cliente.");
            entity.Property(e => e.Observacao)
                .HasColumnType("text");
        });
        #endregion

        base.OnModelCreating(modelBuilder);
    }
}
