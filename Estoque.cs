// Estoque.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms_Fruteira
{
    public partial class Estoque : Form
    {
        // Guarda tudo que veio da API, para filtrar sem chamar a API de novo
        private List<Produto> todos = new List<Produto>();

        private readonly ImageList miniaturas = new ImageList();

        public Estoque()
        {
            InitializeComponent();

            miniaturas.ImageSize = new Size(48, 48);
            miniaturas.ColorDepth = ColorDepth.Depth32Bit;
            listView1.SmallImageList = miniaturas;   // as linhas ficam com 48px de altura
        }

        // Ao abrir a tela, busca os produtos na API
        private async void Estoque_Load(object sender, EventArgs e)
        {
            await CarregarAsync();
        }

        // Botão Pesquisar: aplica os filtros de nome e categoria
        private void button1_Click(object sender, EventArgs e)
        {
            Filtrar();
        }

        private async Task CarregarAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                todos = await ApiClient.ListarAsync() ?? new List<Produto>();
                PreencherCategorias();
                MostrarNaLista(todos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar o estoque:\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void PreencherCategorias()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Todas");

            var categorias = todos
                .Where(p => p.Categoria != null)
                .Select(p => p.Categoria.Nome)
                .Distinct()
                .OrderBy(n => n);

            foreach (var nome in categorias)
                comboBox1.Items.Add(nome);
        }

        private void Filtrar()
        {
            string nome = textBox1.Text.Trim();
            string categoria = comboBox1.SelectedItem as string;

            var filtrados = todos.Where(p =>
                (nome == "" || (p.Nome ?? "").IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (categoria == null || categoria == "Todas" || p.Categoria?.Nome == categoria));

            MostrarNaLista(filtrados);
        }

        private void MostrarNaLista(IEnumerable<Produto> produtos)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            miniaturas.Images.Clear();

            foreach (var p in produtos)
            {
                var item = new ListViewItem(p.Nome);

                if (p.Imagem != null && p.Imagem.Length > 0)
                {
                    try
                    {
                        string chave = p.Id.ToString();
                        using (var img = ImagemHelper.DeBytes(p.Imagem))
                            miniaturas.Images.Add(chave, ImagemHelper.Miniatura(img, 48));
                        item.ImageKey = chave;
                    }
                    catch { /* imagem corrompida: mostra a linha sem foto */ }
                }

                item.SubItems.Add(p.QuantidadeEstoque.ToString());
                item.SubItems.Add("-");
                item.SubItems.Add(p.Preco.ToString("C"));
                item.SubItems.Add(p.Categoria?.Nome ?? "");
                item.Tag = p;
                listView1.Items.Add(item);
            }

            listView1.EndUpdate();
        }

        // Os eventos abaixo continuam aqui porque o Designer os referencia.
        // Se apagar só o método, o projeto deixa de compilar (CS1061).
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}