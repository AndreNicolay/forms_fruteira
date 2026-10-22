using System;

namespace Forms_Fruteira
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        public byte[] Imagem { get; set; }
    }

    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}