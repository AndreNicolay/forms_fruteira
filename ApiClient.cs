using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms_Fruteira
{
    // ApiClient.cs
    using System.Net.Http;

    public static class ApiClient
    {
        // Usamos 'static' para reutilizar o mesmo HttpClient.
        // Isso é uma boa prática para evitar problemas de esgotamento de sockets.
        public static HttpClient HttpClient { get; } = new HttpClient();

        // A URL base da sua API. Certifique-se de que a API esteja rodando!
        // Pegue a URL do seu arquivo launchSettings.json da API.
        public static string BaseUrl { get; } = "https://localhost:7288"; // <-- ATENÇÃO: Verifique e troque pela sua URL correta!
    }

}
