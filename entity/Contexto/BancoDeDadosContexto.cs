using Microsoft.EntityFrameworkCore;
using entity.Entidades;

namespace entity.Contexto;

class BancoDeDadosContexto : DbContext
{
    public DbSet<Cliente> Clientes { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;database=aula_entity;uid=root;password=654321",
        new MySqlServerVersion(new Version(8, 0, 33)));

        //optionsBuilder.UseMySQL("server=localhost;database=aula_entity;uid=root;password=654321");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Defina aqui as configurações específicas do modelo, como chaves primárias, restrições, etc.
    }
}
