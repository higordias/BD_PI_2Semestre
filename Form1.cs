using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.IO;
using Microsoft.VisualBasic;
using System.Collections.Generic;


namespace BancoDeDados_PI
{
    public partial class BDHomeAutomation : Form
    {
        string dir_projeto = System.AppContext.BaseDirectory;
        List<string> colunas = new List<string>();

        public BDHomeAutomation()
        {
            InitializeComponent();
        }

        private void excluirTabela(string tabela)
        {
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            string sqlCommand = "DROP TABLE " + tabela;
            SqlCeCommand command = new SqlCeCommand(sqlCommand, cn);
            command.ExecuteNonQuery();
            cn.Close();
        }
        public int countEntries(string tabela, string coluna, TextBox campo, SqlCeConnection cn)
        {
            string check = "SELECT COUNT(*) from " + tabela + " WHERE " + coluna + "='" + campo.Text + "'";
            SqlCeCommand command = new SqlCeCommand(check, cn);
            int count = (int)command.ExecuteScalar();
            return count;
        }

        public void deleteEntries(string tabela, string coluna, TextBox campo, DataGridView grid)
        {
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }

            int Count = countEntries(tabela, coluna, campo, cn);

            if (Count == 0)
            {
                MessageBox.Show("Entrada nao existe", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                campo.Focus();
                showDataBase(tabela, grid);
                cn.Close();
            }
            else
            {
                if (campo.Text == "")
                {
                    MessageBox.Show("Para deletar um dado, digite o codigo do cliente!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    campo.Focus();
                    showDataBase(tabela, grid);
                    cn.Close();
                }
                else
                {
                    string st = "DELETE FROM " + tabela + " WHERE " + coluna + "='" + campo.Text + "'";
                    SqlCeCommand sqlcom = new SqlCeCommand(st, cn);
                    try
                    {
                        sqlcom.ExecuteNonQuery();
                        MessageBox.Show("Delete successful", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        campo.Text = "";
                        campo.Focus();
                        showDataBase(tabela, grid);
                        cn.Close();
                    }
                    catch (SqlCeException ex)
                    {
                        MessageBox.Show(ex.Message);
                        cn.Close();
                    }
                }
            }
        }

        private void CarregarLinhaTabelaLogin(string nome, string email, string login, string senha, SqlCeConnection cn)
        {
            string infoHex = "";
            foreach (char c in senha)
                infoHex += ((int)c).ToString("x");

            SqlCeCommand cmd;
            string sqlLogin = "insert into TabelaLogin "
                        + "(nome, email, login, senha) "
                        + "values (@Nome, @Email, @Login, @Senha)";
            try
            {
                cmd = new SqlCeCommand(sqlLogin, cn);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Senha", infoHex);
                cmd.ExecuteNonQuery();
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarLinhaTabelaClientes(string codigoCliente, string moduloInstalado, string nome, string telefone,
            string celular, string endereco, string complemento, string CEP, string cidade, string estado, SqlCeConnection cn)
        {

            SqlCeCommand cmd;
            string sqlLogin = "insert into TabelaClientes "
                        + "(codigoCliente, moduloInstalado, nome, telefone, celular, endereco, complemento, CEP, cidade, estado) "
                        + "values (@CodigoCliente, @ModuloInstalado, @Nome, @Telefone, @Celular, @Endereco, @Complemento, @CEP, "
                        + "@Cidade, @Estado)";
            try
            {
                cmd = new SqlCeCommand(sqlLogin, cn);
                cmd.Parameters.AddWithValue("@CodigoCliente", codigoCliente);
                cmd.Parameters.AddWithValue("@ModuloInstalado", moduloInstalado);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Telefone", telefone);
                cmd.Parameters.AddWithValue("@Celular", celular);
                cmd.Parameters.AddWithValue("@Endereco", endereco);
                cmd.Parameters.AddWithValue("@Complemento", complemento);
                cmd.Parameters.AddWithValue("@CEP", CEP);
                cmd.Parameters.AddWithValue("@Cidade", cidade);
                cmd.Parameters.AddWithValue("@Estado", estado);
                cmd.ExecuteNonQuery();
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarLinhaTabelaKits(string codigoKit, string componente1, string componente2, string componente3, SqlCeConnection cn)
        {
            SqlCeCommand cmd;
            string sqlKits = "insert into TabelaKits "
                        + "(codigoKit, componente1, componente2, componente3) "
                        + "values (@CodigoKit, @Componente1, @Componente2, @Componente3)";
            try
            {
                cmd = new SqlCeCommand(sqlKits, cn);
                cmd.Parameters.AddWithValue("@CodigoKit", codigoKit);
                cmd.Parameters.AddWithValue("@Componente1", componente1);
                cmd.Parameters.AddWithValue("@Componente2", componente2);
                cmd.Parameters.AddWithValue("@Componente3", componente3);
                cmd.ExecuteNonQuery();
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void showDataBase(string tabela, DataGridView grid)
        {
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            if (cn.State == ConnectionState.Closed)
            {
                try
                {
                    cn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            try
            {
                // define o command para usar a tabela e não a consulta
                SqlCeCommand cmd = new SqlCeCommand(tabela, cn);
                cmd.CommandType = CommandType.TableDirect;
                // Pega a tabela
                SqlCeResultSet rs = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);
                // carrega o resultado no grid 
                grid.DataSource = rs;
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string stringConexao()
        {
            string connectionString = "";
            try
            {
                string nomeArquivo = @dir_projeto + "\\DB_SmartHomeAutomation.sdf";
                string senha = "";
                connectionString = string.Format("DataSource=\"{0}\"; Password='{1}'", nomeArquivo, senha);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return connectionString;
        }


        /*private void btnExcluir_Click(object sender, EventArgs e)
        {
            string infoHex = "";
            foreach (char c in tbSenha.Text)
                infoHex += ((int)c).ToString("x");

            string nonInfoHex = "";
            for (int i =0; i < infoHex.Length / 2; i++)
            {
                //string hexsenha = infoHex.Substring(i * 2, 2);
                //int hexvalue = Convert.ToInt32(infoHex.Substring(i * 2, 2), 16);
                nonInfoHex += Char.ConvertFromUtf32(Convert.ToInt32(infoHex.Substring(i * 2, 2), 16));
//                raw[i] = Convert.ToByte(infoHex.Substring(i * 2, 2), 16);
            }
            //var infoHex = String.Format("{0:x}", Convert.ToString(tbSenha.Text));

            //MessageBox.Show(infoHex + " " + nonInfoHex);
        }*/        

        /*
        private void button9_Click(object sender, EventArgs e)
        {
            // define os parâmetros para o inputbox
            string Prompt = "Informe o nome do Banco de Dados a ser criado.Ex: Teste.sdf";
            string Titulo = "www.macoratti.net";
            string Resultado = Interaction.InputBox(Prompt, Titulo, @dir_projeto + "\\DB_SmartHomeAutomation.sdf", 650, 350);
            // verifica se o resultado é uma string vazia o que indica que foi cancelado.
            if (Resultado != "")
            {
                if (!Resultado.Contains(".sdf"))
                {
                    MessageBox.Show("Informe a extensão .sdf no arquivo...");
                    return;
                }
                try
                {
                    string connectionString;
                    string nomeArquivoBD = Resultado;
                    string senha = "HomeAutomationDB";

                    if (File.Exists(nomeArquivoBD))
                    {
                        if (MessageBox.Show("O arquivo já existe !. Deseja excluir e criar novamente ? ", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            File.Delete(nomeArquivoBD);
                        }
                        else
                        {
                            return;
                        }
                    }

                    connectionString = string.Format("DataSource=\"{0}\"; Password='{1}'", nomeArquivoBD, senha);

                    if (MessageBox.Show("Será criado arquivo " + connectionString + " Confirma ? ", "Criar", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        SqlCeEngine SqlEng = new SqlCeEngine(connectionString);
                        SqlEng.CreateDatabase();
                        //lblResultado.Text = "Banco de dados " + nomeArquivoBD + " com sucesso !";
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("A operação foi cancelada...");
            }

        }
        */

        private void btnMostrarDados_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaLogin", dgvLogin);
        }


        private void btnAdicionar_Click_1(object sender, EventArgs e)
        {
            if ((tbNome.Text == "") ||
                (tbEmail.Text == "") ||
                (tbLogin.Text == "") ||
                (tbSenha.Text == ""))
            {
                MessageBox.Show("Preencha todos os campos antes de adicionar uma entrada!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (tbNome.Text == "") { tbNome.Focus(); }
                else if (tbEmail.Text == "") { tbEmail.Focus(); }
                else if (tbLogin.Text == "") { tbLogin.Focus(); }
                else if (tbSenha.Text == "") { tbSenha.Focus(); }
            }
            else
            {
                SqlCeConnection cn = new SqlCeConnection(stringConexao());
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }

                int nameCount = countEntries("TabelaLogin", "Nome", tbNome, cn);
                int emailCount = countEntries("TabelaLogin", "Email", tbEmail, cn);
                int loginCount = countEntries("TabelaLogin", "Login", tbLogin, cn);

                if ((nameCount > 0) || (emailCount > 0) || (loginCount > 0))
                {
                    if (loginCount > 0)
                    {
                        MessageBox.Show("Login ja existe", "Entrada Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbLogin.Focus();
                    }
                    if (emailCount > 0)
                    {
                        MessageBox.Show("Email ja existe", "Entrada Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbEmail.Focus();
                    }
                    if (nameCount > 0)
                    {
                        MessageBox.Show("Nome ja existe", "Entrada Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbNome.Focus();
                    }
                }
                else
                {
                    try
                    {
                        CarregarLinhaTabelaLogin(tbNome.Text, tbEmail.Text, tbLogin.Text, tbSenha.Text, cn);
                        tbNome.Text = "";
                        tbEmail.Text = "";
                        tbLogin.Text = "";
                        tbSenha.Text = "";
                        showDataBase("TabelaLogin", dgvLogin);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                cn.Close();
            }
        }

        /*
        private void btnProximo_Click(object sender, EventArgs e)
        {
            if (tbNomeColuna.Text == "")
            {
                MessageBox.Show("Nome da Coluna nao pode ser vazio");
            }
            else
            {
                colunas.Add(tbNomeColuna.Text);
                lblColunas.Text += tbNomeColuna.Text + "\n";
                tbNomeColuna.Text = "";
                tbNomeColuna.Focus();
            }
        }
        */

        /*
        private void btnConcluir_Click(object sender, EventArgs e)
        {
            if (tbNomeColuna.Text != "")
            {
                colunas.Add(tbNomeColuna.Text);
                lblColunas.Text += tbNomeColuna.Text;
                tbNomeColuna.Text = "";
            }
            //lblColunas.Text = "";
            //for (int i = 0; i < colunas.Count; i++)
            //{
            //    if (colunas[i] != "")
            //    {
            //        lblColunas.Text += colunas[i] + "\n";
            //    }
            //}
        }
        */

        
        //private void button11_Click(object sender, EventArgs e)
        //{
        //    SqlCeConnection cn = new SqlCeConnection(stringConexao());
        //    if (cn.State == ConnectionState.Closed)
        //    {
        //        cn.Open();
        //    }
            /*
             * DELETE ENTRY
            string st = "DELETE FROM TabelaLogin WHERE Nome='" + tbToDelete.Text + "'";
            SqlCeCommand sqlcom = new SqlCeCommand(st, cn);
            try
            {
                sqlcom.ExecuteNonQuery();
                MessageBox.Show("Delete successful");
                tbToDelete.Text = "";
                tbToDelete.Focus();
                cn.Close();
            }
            catch (SqlCeException ex)
            {
                MessageBox.Show(ex.Message);
                cn.Close();
            }
            */

            /*
             * CHECK IF ENTRY EXISTS
            string checkEntry = "SELECT COUNT(*) as cnt from TabelaLogin where Nome=@nome";
            SqlCeCommand sqlcom = new SqlCeCommand(checkEntry, cn);
            sqlcom.Parameters.Clear();
            sqlcom.Parameters.AddWithValue("@Nome", tbToDelete.Text);
            if(sqlcom.ExecuteScalar().ToString() == "1")
            {
                MessageBox.Show("Nome existe");
            }
            else
            {
                MessageBox.Show("Nome nao existe");
            }
            cn.Close();
            */
        //}

        private void btnExcluir_Click_1(object sender, EventArgs e)
        {
            deleteEntries("TabelaLogin", "Nome", tbNome, dgvLogin);
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnMostrarClientes_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaClientes", dgvClientes);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // define os parâmetros para o inputbox
            string Prompt = "Informe o nome da tabela a ser criada.Ex: Teste";
            string Titulo = "Smart Home Automation";
            string Resultado = Interaction.InputBox(Prompt, Titulo, "Nome Tabela", 650, 350);
            // verifica se o resultado é uma string vazia o que indica que foi cancelado.

            if (Resultado != "" ||
                Resultado != "Nome Tabela")
            {
                if (Resultado.Contains(".sdf"))
                {
                    MessageBox.Show("Não informe a extensão .sdf no arquivo...");
                    return;
                }

                SqlCeConnection cn = new SqlCeConnection(stringConexao());

                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }
                SqlCeCommand cmd;

                /*string sql = "create table " + Resultado + "("
                           + "CodigoCliente nvarchar (10) not null, "
                           + "ModuloInstalado nvarchar (10), "
                           + "Nome nvarchar (40), "
                           + "Telefone nvarchar (20), "
                           + "Celular nvarchar (20), "
                           + "Endereco nvarchar (50), "
                           + "Complemento nvarchar (20), "
                           + "CEP nvarchar (15), "
                           + "Cidade nvarchar (20), "
                           + "Estado nvarchar (2) )";
                */

                string sql = "create table " + Resultado + "("
                           + "CodigoKit nvarchar (10) not null, "
                           + "Componente1 nvarchar (20), "
                           + "Componente2 nvarchar (20), "
                           + "Componente3 nvarchar (20) )";

                cmd = new SqlCeCommand(sql, cn);

                if (MessageBox.Show("Confirma a criação da tabela ? ", "Criar Tabela", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        //lblResultado.Text = "Tabela " + Resultado + " criada com sucesso ";
                    }
                    catch (SqlCeException sqlexception)
                    {
                        MessageBox.Show(sqlexception.Message, "Caramba1.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Caramba2.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("A operação foi cancelada...");
            }
        }

        private void btnAdicionarCliente_Click(object sender, EventArgs e)
        {
            if ((tbCodigo.Text == "")           ||
                (tbModuloInstalado.Text == "")  ||
                (tbCliente.Text == "")          ||
                (tbTelefone.Text == "")         ||
                (tbCelular.Text == "")          ||
                (tbEndereco.Text == "")         ||
                (tbCep.Text == "")              ||
                (tbCidade.Text == "")           ||
                (tbUF.Text == ""))
            {
                MessageBox.Show("Preencha todos os campos antes de adicionar uma entrada!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if      (tbCodigo.Text == "")           { tbCodigo.Focus();             }
                else if (tbModuloInstalado.Text == "")  { tbModuloInstalado.Focus();    }
                else if (tbCliente.Text == "")          { tbCliente.Focus();            }
                else if (tbTelefone.Text == "")         { tbTelefone.Focus();           }
                else if (tbCelular.Text == "")          { tbCelular.Focus();            }
                else if (tbEndereco.Text == "")         { tbEndereco.Focus();           }
                else if (tbCep.Text == "")              { tbCep.Focus();                }
                else if (tbCidade.Text == "")           { tbCidade.Focus();             }
                else if (tbUF.Text == "")               { tbUF.Focus();                 }
            }
            else
            {
                SqlCeConnection cn = new SqlCeConnection(stringConexao());
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }

                int codigoCount = countEntries("TabelaClientes", "CodigoCliente", tbCodigo, cn);
                int clienteCount = countEntries("TabelaClientes", "Nome", tbCliente, cn);
                int enderecoCount = countEntries("TabelaClientes", "Endereco", tbEndereco, cn);

                if ((codigoCount > 0)   || 
                    (clienteCount > 0)  ||
                    (enderecoCount > 0))
                {
                    if (codigoCount > 0)
                    {
                        MessageBox.Show("Codigo ja existe", "Entrada Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbCodigo.Focus();
                    }
                    if (clienteCount > 0)
                    {
                        MessageBox.Show("Cliente ja existe", "Entrada Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbCliente.Focus();
                    }
                    if (enderecoCount > 0)
                    {
                        MessageBox.Show("Endereco ja existe", "Entrada Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbEndereco.Focus();
                    }
                }
                else
                {
                    try
                    {
                        CarregarLinhaTabelaClientes(tbCodigo.Text, tbModuloInstalado.Text, tbCliente.Text, tbTelefone.Text,
                            tbCelular.Text, tbEndereco.Text, tbComplemento.Text, tbCep.Text, tbCidade.Text, tbUF.Text, cn);

                        tbCodigo.Text = "";
                        tbModuloInstalado.Text = "";
                        tbCliente.Text = "";
                        tbTelefone.Text = "";
                        tbCelular.Text = "";
                        tbEndereco.Text = "";
                        tbComplemento.Text = "";
                        tbCep.Text = "";
                        tbCidade.Text = "";
                        tbUF.Text = "";
                        showDataBase("TabelaClientes", dgvClientes);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                cn.Close();
            }
        }

        private void btnExcluirCliente_Click(object sender, EventArgs e)
        {
            deleteEntries("TabelaClientes", "CodigoCliente", tbCodigo, dgvClientes);
        }

        private void btnSairCliente_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSairKits_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnMostrarKits_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaKits", dgvKits);
        }

        private void btnAdicionarKits_Click(object sender, EventArgs e)
        {
            if ((tbCodigoModulo.Text == "") ||
                (tbComponente1.Text == "")  ||
                (tbComponente2.Text == "")  ||
                (tbComponente3.Text == ""))
            {
                MessageBox.Show("Preencha todos os campos antes de adicionar uma entrada!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (tbCodigoModulo.Text == "")      { tbCodigoModulo.Focus();   }
                else if (tbComponente1.Text == "")  { tbComponente1.Focus();    }
                else if (tbComponente2.Text == "")  { tbComponente2.Focus();    }
                else if (tbComponente3.Text == "")  { tbComponente3.Focus();    }
            }
            else
            {
                SqlCeConnection cn = new SqlCeConnection(stringConexao());
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }

                int codigoCount = countEntries("TabelaKits", "CodigoKit", tbCodigoModulo, cn);

                if (codigoCount > 0)
                {
                    if (codigoCount > 0)
                    {
                        MessageBox.Show("Codigo ja existe", "Entrada Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbCodigoModulo.Focus();
                    }
                }
                else
                {
                    try
                    {
                        CarregarLinhaTabelaKits(tbCodigoModulo.Text, tbComponente1.Text, tbComponente2.Text, tbComponente3.Text, cn);
                        tbCodigoModulo.Text = "";
                        tbComponente1.Text = "";
                        tbComponente2.Text = "";
                        tbComponente3.Text = "";
                        showDataBase("TabelaKits", dgvKits);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                cn.Close();
            }
        }

        private void btnExcluirTabela_Click(object sender, EventArgs e)
        {
            excluirTabela("TabelaKits");
        }

        private void btnExcluirKits_Click(object sender, EventArgs e)
        {
            deleteEntries("TabelaKits", "CodigoKit", tbCodigoModulo, dgvKits);
        }
    }
}
