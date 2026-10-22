using Newtonsoft.Json;   // ← aqui
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Forms_Fruteira
{
    // ApiClient.cs
    using System.Net.Http;

    public static class ApiClient
    {
        private static readonly HttpClient http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7123/") // porta do Swagger
        };

        public static async Task<List<Produto>> ListarAsync()
        {
            var json = await http.GetStringAsync("api/produtos");
            return JsonConvert.DeserializeObject<List<Produto>>(json);
        }

        public static async Task<List<Categoria>> ListarCategoriasAsync()
        {
            var json = await http.GetStringAsync("api/Categorias");
            return JsonConvert.DeserializeObject<List<Categoria>>(json);
        }

        public static async Task CadastrarAsync(Produto p)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var body = new StringContent(JsonConvert.SerializeObject(p, settings), Encoding.UTF8, "application/json");
            var resp = await http.PostAsync("api/produtos", body);

            if (!resp.IsSuccessStatusCode)
            {
                string erro = await resp.Content.ReadAsStringAsync();
                throw new Exception($"A API respondeu {(int)resp.StatusCode} ({resp.StatusCode}):\n{erro}");
            }
        }
    }
}