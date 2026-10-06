// Estoque.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms_Fruteira
{
    public partial class Estoque : Form
    {
        public Estoque()
        {
            InitializeComponent();
            // Dê um nome melhor para sua ListView no Designer, como "lstEstoque"
        }

        // Marcamos o método como 'async' para poder usar 'await'
        private async void Estoque_Load(object sender, EventArgs e)
        {
            await CarregarEstoqueAsync();
        }

        private async Task CarregarEstoqueAsync()
        {
            try
            {
                // Faz a requisição GET para o endpoint da API
                var response = await ApiClient.HttpClient.GetAsync($"{ApiClient.BaseUrl}/api/produtos");

                // Verifica se a requisição foi bem-sucedida (código 2xx)
                if (response.IsSuccessStatusCode)
                {
                    // Lê o conteúdo da resposta (o JSON) como uma string
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // Converte a string JSON para uma lista de objetos Produto
                    var produtos = JsonSerializer.Deserialize<List<Produto>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Facilita se o case das propriedades for diferente
                    });

                    // Limpa a lista antes de adicionar os novos itens
                    listView1.Items.Clear();

                    // Preenche a ListView com os dados dos produtos
                    foreach (var produto in produtos)
                    {
                        var item = new ListViewItem(produto.Nome);
                        item.SubItems.Add("N/D"); // Quantidade não está no seu modelo Produto da API
                        item.SubItems.Add(produto.Validade.ToShortDateString());
                        item.SubItems.Add(produto.Valor.ToString("C")); // "C" formata como moeda (R$)
                        item.SubItems.Add("N/D"); // Tipo/Categoria não está sendo retornado com nome
                        listView1.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show($"Erro ao buscar estoque: {response.ReasonPhrase}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro de conexão: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

