using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace entity.Entidades;


//Anotacoes da forma 1
[Table("tb_fornecedores")]
public record Fornecedor
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [Column(name: "for_id")]
    public int Id { get; set; }

    [Required] //Notnull
    [MaxLength(100)] //tamanho
    [Column(name: "for_nome")]
    public string Nome { get; set; } = default!;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [MaxLength(20)]
    [Column(name: "for_telefone")]
    public string Telefone { get; set; } = default!;

    [Column(TypeName = "text")]
    public string Observacao { get; set; } = default!;


}
