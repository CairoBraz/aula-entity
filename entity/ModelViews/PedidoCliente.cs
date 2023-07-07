namespace entity.ModelViews;

public struct PedidoCliente
{
    public int PedidoId { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public double ValorTotal { get; set; }
    public string NomeProduto { get; set; }
    public int QuantidadeVendidaParaProduto { get; set; }
    public double ValorVendidoParaProduto { get; set; }
}
