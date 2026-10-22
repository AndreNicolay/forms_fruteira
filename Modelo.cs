using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json.Serialization;

namespace Forms_Fruteira
{
    // Modelos.cs

    // Este modelo representa um Produto como ele vem da API.
    public class Produto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        [JsonPropertyName("validade")]
        public DateTime Validade { get; set; }

        [JsonPropertyName("categoriaId")]
        public int CategoriaId { get; set; }

        // Podemos ignorar o objeto Categoria completo na listagem se não precisarmos dele aqui.
    }

}
