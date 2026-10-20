using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Forms_Fruteira
{
    public partial class Fruteira : Form
    {
        private static readonly CultureInfo ptBR = new CultureInfo("pt-BR");

        private byte[] imagemBytes;

        public Fruteira()
        {
            InitializeComponent();
        }

        // Ao abrir a tela, carrega as categorias da API no combo "Tipo"
        private async void Fruteira_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // mostra a imagem inteira, sem cortar
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; // impede digitar texto livre
            await CarregarCategoriasAsync();
        }

        private async Task CarregarCategoriasAsync()
        {
            try
            {
                var categorias = await ApiClient.ListarCategoriasAsync();

                // Mostra o Nome, mas guarda o Id de cada categoria
                comboBox1.DisplayMember = "Nome";
                comboBox1.ValueMember = "Id";
                comboBox1.DataSource = categorias;
                comboBox1.SelectedIndex = -1; // começa sem nada selecionado
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar as categorias:\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botão Cadastrar
        private async void button4_Click(object sender, EventArgs e)
        {
            Produto produto;
            if (!ValidarCampos(out produto)) return;

            try
            {
                button4.Enabled = false;          // evita cadastrar duas vezes com clique duplo
                Cursor = Cursors.WaitCursor;

                await ApiClient.CadastrarAsync(produto);

                MessageBox.Show($"\"{produto.Nome}\" cadastrada com sucesso!", "Fruteira",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar:\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button4.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        // Botão Cancelar
        private void button3_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private bool ValidarCampos(out Produto produto)
        {
            produto = null;

            string nome = textBox1.Text.Trim();
            if (nome == "")
            {
                Avisar("Informe o nome da fruta.", textBox1);
                return false;
            }

            int quantidade;
            if (!int.TryParse(textBox2.Text.Trim(), out quantidade) || quantidade < 0)
            {
                Avisar("A quantidade deve ser um número inteiro (ex.: 10).", textBox2);
                return false;
            }

            if (comboBox1.SelectedValue == null)
            {
                Avisar("Escolha o tipo da fruta.", comboBox1);
                return false;
            }

            // Aceita "4,99", "R$ 4,99" e também "4.99" (troca o ponto por vírgula)
            string textoValor = textBox3.Text.Trim();
            if (!textoValor.Contains(",")) textoValor = textoValor.Replace('.', ',');

            decimal preco;
            if (!decimal.TryParse(textoValor, NumberStyles.Currency, ptBR, out preco) || preco <= 0)
            {
                Avisar("Valor inválido. Exemplo: 4,99", textBox3);
                return false;
            }

            produto = new Produto
            {
                Nome = nome,
                QuantidadeEstoque = quantidade,
                Preco = preco,
                CategoriaId = (int)comboBox1.SelectedValue,
                Imagem = imagemBytes          // pode ser null: a imagem é opcional
            };
            return true;
        }

        private void Avisar(string mensagem, Control campo)
        {
            MessageBox.Show(mensagem, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            campo.Focus();
        }

        private void LimparCampos()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            monthCalendar1.SetDate(DateTime.Today);
            pictureBox1.Image = null;
            textBox1.Focus();
            imagemBytes = null;
        }

        // Botão Estoque
        private void button2_Click(object sender, EventArgs e)
        {
            AbrirEstoque();
        }

        private void AbrirEstoque()
        {
            using (Estoque estoque = new Estoque())
            {
                this.Hide();
                estoque.ShowDialog();
            }
            this.Show();
        }

        // Referenciado pelo Designer, não apague só por aqui
        private void label2_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Escolha a imagem da fruta";
                dialogo.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (dialogo.ShowDialog() != DialogResult.OK) return;

                try
                {
                    // O "using" solta o arquivo logo depois de ler (Image.FromFile trava o arquivo)
                    using (var original = Image.FromFile(dialogo.FileName))
                    {
                        var reduzida = ImagemHelper.Redimensionar(original, 400);
                        pictureBox1.Image = reduzida;
                        imagemBytes = ImagemHelper.ParaBytes(reduzida);
                    }
                }
                catch (OutOfMemoryException)   // é assim que o .NET avisa "não é uma imagem válida"
                {
                    MessageBox.Show("Esse arquivo não é uma imagem válida.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}