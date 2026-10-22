using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms_Fruteira
{
    public partial class Fruteira : Form
    {
        public Fruteira()
        {
            InitializeComponent();
        }

        private void Fruteira_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            AbrirEstoque();
        }

        private void AbrirEstoque()
        {
            // new cria a segunda tela na memória.
            // using garante que ela é liberada quando terminarmos.
            using (Estoque estoque = new Estoque())
            {
                // Esconde o Cadastro para as duas telas não ficarem empilhadas.
                this.Hide();

                // ShowDialog() PARA aqui e só continua na linha seguinte quando
                // o usuário fechar o Estoque. Se usássemos Show(), o código
                // continuaria correndo na hora e o Cadastro reapareceria já.
                estoque.ShowDialog();
            }

            // O Estoque foi fechado: mostramos o Cadastro de novo.
            this.Show();
        }
    }
}
