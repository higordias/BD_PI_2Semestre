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
        string dir_projeto = System.AppContext.BaseDirectory; //variable that holds the database path
        List<string> colunas = new List<string>();

        public BDHomeAutomation()
        {
            InitializeComponent();
        }

        /**
         * countEntries is used to count the number of entries a certain data has in a table
         * @tabela: table's name where the value is
         * @coluna: column's name
         * @campo: field in the form where the value is typed
         * @cn: sql database connection
         * @return: the number of entries
         **/
        public int countEntries(string tabela, string coluna, TextBox campo, SqlCeConnection cn)
        {
            string check = "SELECT COUNT(*) from " + tabela + " WHERE " + coluna + "='" + campo.Text + "'";
            SqlCeCommand command = new SqlCeCommand(check, cn);
            int count = (int)command.ExecuteScalar();
            return count;
        }

        /**
         * deleteEntries is used to delete a certain data from a table
         * @tabela: table's name where the value is
         * @coluna: column's name
         * @campo: field in the form where the value is typed
         * @grid: grid to show the values before deleting the entry
         **/
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
            }
            else
            {
                if (campo.Text == "")
                {
                    MessageBox.Show("Digite a entrada a ser deletada!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    campo.Focus();
                    showDataBase(tabela, grid);
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
                    }
                    catch (SqlCeException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /**
         * CarregarLinhaTabelaLogin is used to add entries to TabelaLogin
         * @nome: client's name
         * @email: client's email
         * @login: client's login
         * @senha: client's password
         * @cn: sql database connection
         **/
        private void CarregarLinhaTabelaLogin(string nome, string email, string login, string senha, string RG, string CPF,
                                              string pergunta, string resposta, string privilegio, SqlCeConnection cn)
        {
            string infoHex = "";
            foreach (char c in senha)
                infoHex += ((int)c).ToString("x");

            SqlCeCommand cmd;
            string sqlLogin = "insert into TabelaLogin "
                            + "(nome, email, login, senha, RG, CPF, pergunta, resposta, privilegio) "
                            + "values (@Nome, @Email, @Login, @Senha, @RG, @CPF, @Pergunta, @Resposta, @Privilegio)";
            try
            {
                cmd = new SqlCeCommand(sqlLogin, cn);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Senha", infoHex);
                cmd.Parameters.AddWithValue("@Email", RG);
                cmd.Parameters.AddWithValue("@Login", CPF);
                cmd.Parameters.AddWithValue("@Senha", pergunta);
                cmd.Parameters.AddWithValue("@Senha", resposta);
                cmd.Parameters.AddWithValue("@Privilegio", privilegio);
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

        /**
         * CarregarLinhaTabelaClientes is used to add entries to TabelaClientes
         * @codigoCliente: client's code
         * @moduloInstalado: client's installed kit
         * @nome: client's nome
         * @telefone: client's phone number
         * @celular: client's cell number
         * @email: client's email
         * @endereco: client's address
         * @complemento: client's address complement
         * @CEP: client's zip code
         * @cidade: client's city
         * @estado: client's state
         * @cn: sql database connection
         **/
        private void CarregarLinhaTabelaClientes(string codigoCliente, string moduloInstalado, string nome,
                                                 string telefone, string celular, string endereco, string complemento,
                                                 string CEP, string cidade, string estado, SqlCeConnection cn)
        {

            SqlCeCommand cmd;
            string sqlLogin = "insert into TabelaClientes "
                            + "(codigoCliente, moduloInstalado, nome, telefone, celular,"
                            + " endereco, complemento, CEP, cidade, estado) "
                            + "values (@CodigoCliente, @ModuloInstalado, @Nome, @Telefone, "
                            + "@Celular, @Endereco, @Complemento, @CEP, @Cidade, @Estado)";
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

        /**
         * CarregarLinhaTabelaKits is used to add entries to TabelaKits
         * @codigoKit: kit's code
         * @componente1: name of the component 1
         * @componente2: name of the component 2
         * @componente3: name of the component 3
         * @cn: sql database connection
         **/
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

        /**
         * CarregarLinhaTabelaRFID is used to add entries to TabelaRFID
         * @nome: client's name
         * @cartao1 - cartao10: client's rfid card
         * @cn: sql database connection
         **/
        private void CarregarLinhaTabelaRFID(string Responsavel, string Cartao1, string Cartao2, string Cartao3, string Cartao4, string Cartao5,
                                              string Cartao6, string Cartao7, string Cartao8, string Cartao9, string Cartao10, SqlCeConnection cn)
        {
            SqlCeCommand cmd;
            string sqlRFID = "insert into TabelaRFID "
                            + "(Responsavel, Cartao1, Cartao2, Cartao3, Cartao4, Cartao5, Cartao6, Cartao7, Cartao8, Cartao9, Cartao10) "
                            + "values (@Responsavel, @Cartao1, @Cartao2, @Cartao3, @Cartao4, @Cartao5, @Cartao6, @Cartao7, @Cartao8, @Cartao9, @Cartao10)";
            try
            {
                cmd = new SqlCeCommand(sqlRFID, cn);
                cmd.Parameters.AddWithValue("@Responsavel", Responsavel);
                cmd.Parameters.AddWithValue("@Cartao1", Cartao1);
                cmd.Parameters.AddWithValue("@Cartao2", Cartao2);
                cmd.Parameters.AddWithValue("@Cartao3", Cartao3);
                cmd.Parameters.AddWithValue("@Cartao4", Cartao4);
                cmd.Parameters.AddWithValue("@Cartao5", Cartao5);
                cmd.Parameters.AddWithValue("@Cartao6", Cartao6);
                cmd.Parameters.AddWithValue("@Cartao7", Cartao7);
                cmd.Parameters.AddWithValue("@Cartao8", Cartao8);
                cmd.Parameters.AddWithValue("@Cartao9", Cartao9);
                cmd.Parameters.AddWithValue("@Cartao10", Cartao10);
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

        /**
         * showDataBase will load the database and show it in a grid
         * @tabela: table's name
         * @grid: grid where the table will be showed
         **/
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

        /**
         * stringConexao connects to the database
         **/
        public string stringConexao()
        {
            string connectionString = "";
            try
            {
                string nomeArquivo = @dir_projeto + "\\DB_SmartHomeAutomation.sdf";
                string senha = "";
                connectionString = string.Format("DataSource=\"{0}\"; Password='HomeAutomationDB'", nomeArquivo, senha);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return connectionString;
        }

        /**
         * btnMostrarDados_Click action used in the button Mostrar Dados (Tab Tabela Login)
         **/
        private void btnMostrarDados_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaLogin", dgvLogin);
        }

        /**
         * btnAdicionar_Click_1 action used in the button Adicionar (Tab Tabela Login)
         **/
        private void btnAdicionar_Click_1(object sender, EventArgs e)
        {
            bool nameSelected = false;
            string selectedItemName = cbNome.SelectedItem.ToString();
            int nameIndex = cbNome.FindString(selectedItemName);

            if (nameIndex >= 0)
            {
                nameSelected = true;
            }

            bool questionSelected = false;
            string selectedItemQuestion = cbPergunta.SelectedItem.ToString();
            int questionIndex = cbPergunta.FindString(selectedItemQuestion);

            if (questionIndex >= 0)
            {
                questionSelected = true;
            }

            bool privilegySelected = false;
            string selectedItemPrivilegy = cbPrivilegio.SelectedItem.ToString();
            int privilegyIndex = cbPrivilegio.FindString(selectedItemPrivilegy);

            if (privilegyIndex >= 0)
            {
                privilegySelected = true;
            }

            if ((nameSelected == false)  ||
                (tbEmail.Text == "") ||
                (tbLogin.Text == "") ||
                (tbSenha.Text == "") ||
                (tbCPF.Text == "") ||
                (tbRG.Text == "") ||
                (privilegySelected == false) ||
                (questionSelected == false) ||
                (tbResposta.Text == ""))
            {
                MessageBox.Show("Preencha todos os campos antes de adicionar uma entrada!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (nameSelected == false) { cbNome.Focus(); }
                else if (tbEmail.Text == "") { tbEmail.Focus(); }
                else if (tbLogin.Text == "") { tbLogin.Focus(); }
                else if (tbSenha.Text == "") { tbSenha.Focus(); }
                else if (tbCPF.Text == "") { tbCPF.Focus(); }
                else if (tbRG.Text == "") { tbRG.Focus(); }
                else if (questionSelected == false) { cbPergunta.Focus(); }
                else if (privilegySelected == false) { cbPrivilegio.Focus(); }
                else if (tbResposta.Text == "") { tbResposta.Focus(); }
            }
            else
            {
                SqlCeConnection cn = new SqlCeConnection(stringConexao());
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }

                int loginCount = countEntries("TabelaLogin", "Login", tbLogin, cn);
                int emailCount = countEntries("TabelaLogin", "Email", tbEmail, cn);


                if ((loginCount > 0) && (emailCount > 0))
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
                }
                else
                {
                    try
                    {
                        CarregarLinhaTabelaLogin(selectedItemName, tbEmail.Text, tbLogin.Text, tbSenha.Text, tbRG.Text, tbCPF.Text,
                                                 selectedItemQuestion, tbResposta.Text, selectedItemPrivilegy, cn);
                        tbEmail.Text = "";
                        tbLogin.Text = "";
                        tbSenha.Text = "";
                        cbNome.SelectedIndex = 0;
                        tbRG.Text = "";
                        tbCPF.Text = "";
                        cbPergunta.SelectedIndex = 0;
                        cbPrivilegio.SelectedIndex = 1;
                        tbResposta.Text = "";
                        showDataBase("TabelaLogin", dgvLogin);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /**
         * btnExcluir_Click_1 action used in the button Excluir (Tab Tabela Login)
         **/
        private void btnExcluir_Click_1(object sender, EventArgs e)
        {
            if (tbLogin.Text == "")
            {
                MessageBox.Show("Digite o Login a ser deletado!", "Campo Vazio", MessageBoxButtons.OK);
                tbLogin.Focus();
            }
            else
            {
                deleteEntries("TabelaLogin", "Login", tbLogin, dgvLogin);
            }
        }

        /**
         * btnSair_Click action used in the button Sair (Tab Tabela Login)
         **/
        private void btnSair_Click(object sender, EventArgs e)
        {
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            cn.Close();
            Close();
        }

        /**
         * btnMostrarClientes_Click action used in the button Mostrar Dados (Tab Tabela Clientes)
         **/
        private void btnMostrarClientes_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaClientes", dgvClientes);
        }

        /**
         * btnAdicionarCliente_Click action used in the button Adicionar (Tab Tabela Clientes)
         **/
        private void btnAdicionarCliente_Click(object sender, EventArgs e)
        {
            bool selected = false;
            string selectedItem = cbModuloInstalado.SelectedItem.ToString();
            int index = cbModuloInstalado.FindString(selectedItem);

            if (index >= 0)
            {
                selected = true;
            }

            if ((tbCodigo.Text == "")           ||
                (selected == false)             ||
                (tbCliente.Text == "")          ||
                (tbCelular.Text == "")          ||
                (tbEndereco.Text == "")         ||
                (tbCep.Text == "")              ||
                (tbCidade.Text == "")           ||
                (tbCPF.Text == "")              ||
                (tbRG.Text == "")               ||
                (tbUF.Text == ""))
            {
                MessageBox.Show("Preencha todos os campos antes de adicionar uma entrada!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if      (tbCodigo.Text == "")           { tbCodigo.Focus();             }
                else if (selected == false)             { cbModuloInstalado.Focus();    }
                else if (tbCliente.Text == "")          { tbCliente.Focus();            }
                else if (tbCPF.Text == "")              { tbCPF.Focus();                }
                else if (tbRG.Text == "")               { tbRG.Focus();                 }
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
                       CarregarLinhaTabelaClientes(tbCodigo.Text, selectedItem, tbCliente.Text, tbTelefone.Text, tbCelular.Text,
                            tbEndereco.Text, tbComplemento.Text, tbCep.Text, tbCidade.Text, tbUF.Text, cn);

                        tbCodigo.Text       = "";
                        tbCliente.Text      = "";
                        tbTelefone.Text     = "";
                        tbCPF.Text          = "";
                        tbRG.Text           = "";
                        tbCelular.Text      = "";
                        tbEndereco.Text     = "";
                        tbComplemento.Text  = "";
                        tbCep.Text          = "";
                        tbCidade.Text       = "";
                        tbUF.Text           = "";
                        showDataBase("TabelaClientes", dgvClientes);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /**
         * btnExcluirCliente_Click action used in the button Excluir (Tab Tabela Clientes)
         **/
        private void btnExcluirCliente_Click(object sender, EventArgs e)
        {
            if (tbCodigo.Text == "")
            {
                MessageBox.Show("Digite o Codigo a ser deletado!", "Campo Vazio", MessageBoxButtons.OK);
                tbCodigo.Focus();
            }
            else
            {
                deleteEntries("TabelaClientes", "CodigoCliente", tbCodigo, dgvClientes);
            }
        }

        /**
         * btnSairCliente_Click action used in the button Sair (Tab Tabela Clientes)
         **/
        private void btnSairCliente_Click(object sender, EventArgs e)
        {
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            cn.Close();
            Close();
        }

        /**
         * btnSairKits_Click action used in the button Sair (Tab Tabela Kits)
         **/
        private void btnSairKits_Click(object sender, EventArgs e)
        {
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            cn.Close();
            Close();
        }

        /**
         * btnMostrarKits_Click action used in the button Mostrar Dados (Tab Tabela Kits)
         **/
        private void btnMostrarKits_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaKits", dgvKits);
        }

        /**
         * btnAdicionarKits_Click action used in the button Adicionar (Tab Tabela Kits)
         **/
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
                        tbComponente1.Text  = "";
                        tbComponente2.Text  = "";
                        tbComponente3.Text  = "";
                        showDataBase("TabelaKits", dgvKits);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /**
         * btnExcluirKits_Click action used in the button Excluir (Tab Tabela Kits)
         **/
        private void btnExcluirKits_Click(object sender, EventArgs e)
        {
            if (tbCodigoModulo.Text == "")
            {
                MessageBox.Show("Digite o Modulo a ser deletado!", "Campo Vazio", MessageBoxButtons.OK);
                tbCodigoModulo.Focus();
            }
            else
            {
                deleteEntries("TabelaKits", "CodigoKit", tbCodigoModulo, dgvKits);
            }
        }

        /**
         * TabelaLogin_Enter action used whe the Tab Tabela Login is selected
         * will fill the combo box Nome with dashes and then with the values stores in the TabelaClientes table
         **/
        private void TabelaLogin_Enter(object sender, EventArgs e)
        {
            cbNome.Items.Clear();
            cbPrivilegio.Items.Clear();
            cbPrivilegio.Items.Add("Adm");
            cbPrivilegio.Items.Add("User");
            cbNome.Items.Add("------");
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            SqlCeCommand command = new SqlCeCommand("SELECT * FROM TabelaClientes", cn);
            SqlCeDataAdapter da = new SqlCeDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                cbNome.Items.Add(dr["Nome"].ToString());
            }
            cbNome.SelectedIndex = 0;
        }

        /**
         * TabelaClientes_Enter action used whe the Tab Tabela Clietes is selected
         * will fill the combo box Modulo Instado with dashes and then with the values stores in the TabelaKits table
         **/
        private void TabelaClientes_Enter(object sender, EventArgs e)
        {
            cbModuloInstalado.Items.Clear();
            cbModuloInstalado.Items.Add("------");
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            SqlCeCommand command = new SqlCeCommand("SELECT * FROM TabelaKits", cn);
            SqlCeDataAdapter da = new SqlCeDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                cbModuloInstalado.Items.Add(dr["CodigoKit"].ToString());
            }
            cbModuloInstalado.SelectedIndex = 0;
        }

        /**
         * btnLimparClientes_Click action used in the button Limpar (Tab Tabela Clientes)
         * clean all the entries
         **/
        private void btnLimparClientes_Click(object sender, EventArgs e)
        {
            tbCodigo.Text       = "";
            tbCliente.Text      = "";
            tbCPF.Text          = "";
            tbRG.Text           = "";
            tbTelefone.Text     = "";
            tbCelular.Text      = "";
            tbEndereco.Text     = "";
            tbComplemento.Text  = "";
            tbCep.Text          = "";
            tbCidade.Text       = "";
            tbUF.Text           = "";
            cbModuloInstalado.SelectedIndex = 0;
        }

        /**
         * btnLimparLogin_Click action used in the button Limpar (Tab Tabela Login)
         * clean all the entries
         **/
        private void btnLimparLogin_Click(object sender, EventArgs e)
        {
            tbEmail.Text = "";
            tbLogin.Text = "";
            tbSenha.Text = "";
            cbNome.SelectedIndex = 0;
            tbRG.Text = "";
            tbCPF.Text = "";
            tbResposta.Text = "";
            cbPergunta.SelectedIndex = 0;
            cbPrivilegio.SelectedIndex = 1;
        }

        /**
         * BDHomeAutomation_Load action used when the database is loaded
         * It adds questions to the combo box Pergunta in the Tabala Login Tab
         **/
        private void BDHomeAutomation_Load(object sender, EventArgs e)
        {
            cbPergunta.Items.Clear();
            cbPergunta.Items.Add("Qual seu animal favorito?");
            cbPergunta.Items.Add("Qual o nome da sua primeira escola?");
            cbPergunta.Items.Add("Qual o nome da sua primeira professora?");
            cbPergunta.Items.Add("Qual o seu carro favorito?");
            cbPergunta.Items.Add("Qual o nome do seu primeiro animal de estimação?");
            cbPergunta.Items.Add("Qual a cidade natal de sua avó?");
            cbPergunta.Items.Add("Qual país você mais deseja conhecer?");
        }

        /**
         * TabelaRFID_Enter action used when the tab TabelaRFID is selected
         * It adds dashes to the client`s name combo box and also adds the name of
         * the clients located in the TabelaClientes
         **/
        private void TabelaRFID_Enter(object sender, EventArgs e)
        {
            cbResponsavelRFID.Items.Clear();
            cbResponsavelRFID.Items.Add("------");
            SqlCeConnection cn = new SqlCeConnection(stringConexao());
            SqlCeCommand command = new SqlCeCommand("SELECT * FROM TabelaClientes", cn);
            SqlCeDataAdapter da = new SqlCeDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                cbResponsavelRFID.Items.Add(dr["Nome"].ToString());
            }
            cbResponsavelRFID.SelectedIndex = 0;
        }

        /**
         * btnAddRFID_Click action used in the button Adicionar (Tab Tabela RFID)
         * Add entries to the table
         **/
        private void btnAddRFID_Click(object sender, EventArgs e)
        {
            bool nameSelected = false;
            string selectedItemName = cbResponsavelRFID.SelectedItem.ToString();
            int nameIndex = cbResponsavelRFID.FindString(selectedItemName);

            if (nameIndex >= 0)
            {
                nameSelected = true;
            }

            if (nameSelected == false)
            {
                MessageBox.Show("Selecione um cliente para adicionar uma entrada!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (nameSelected == false) { cbResponsavelRFID.Focus(); }
            }
            else
            {
                SqlCeConnection cn = new SqlCeConnection(stringConexao());
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }

                try
                {
                    CarregarLinhaTabelaRFID(selectedItemName, tbCartao1.Text, tbCartao2.Text, tbCartao3.Text, tbCartao4.Text, tbCartao5.Text,
                                                tbCartao6.Text, tbCartao7.Text, tbCartao8.Text, tbCartao9.Text, tbCartao10.Text, cn);
                    tbCartao1.Text = "";
                    tbCartao2.Text = "";
                    tbCartao3.Text = "";
                    cbResponsavelRFID.SelectedIndex = 0;
                    tbCartao4.Text = "";
                    tbCartao5.Text = "";
                    tbCartao6.Text = "";
                    tbCartao7.Text = "";
                    tbCartao8.Text = "";
                    tbCartao9.Text = "";
                    tbCartao10.Text = "";
                    showDataBase("TabelaRFID", dgvRFID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /**
         * btnMostrarRFID_Click action used in the button Mostrar Dados (Tab Tabela RFID)
         * Show the table TabelaRFID
         **/
        private void btnMostrarRFID_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaRFID", dgvRFID);
        }

        /**
         * btnExcluirRFID_Click action used in the button Excluir (Tab Tabela RFID)
         * Delete selected client from the table TabelaRFID
         **/
        private void btnExcluirRFID_Click(object sender, EventArgs e)
        {
            bool nameSelected = false;
            string selectedItemName = cbResponsavelRFID.SelectedItem.ToString();
            int nameIndex = cbResponsavelRFID.FindString(selectedItemName);
            tbResponsavelRFID.Text = selectedItemName;

            if (nameIndex >= 0)
            {
                nameSelected = true;
            }

            if (nameSelected == false)
            {
                MessageBox.Show("Selecione um cliente para ser deletado!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbResponsavelRFID.Focus();
            }
            else
            {
                deleteEntries("TabelaRFID", "Responsavel", tbResponsavelRFID, dgvRFID);
            }
        }

        /**
         * button1_Click action used in the button Limpar (Tab Tabela RFID)
         * Clean all the textboxes
         **/
        private void button1_Click(object sender, EventArgs e)
        {
            cbResponsavelRFID.SelectedIndex = 0;
            tbResponsavelRFID.Text = "";
            tbCartao1.Text = "";
            tbCartao2.Text = "";
            tbCartao3.Text = "";
            tbCartao4.Text = "";
            tbCartao5.Text = "";
            tbCartao6.Text = "";
            tbCartao7.Text = "";
            tbCartao8.Text = "";
            tbCartao9.Text = "";
            tbCartao10.Text = "";
        }

        /**
         * button2_Click action used in the button Sair (Tab Tabela RFID)
         * Exit the program
         **/
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /**
         * btnMostrarAcessos_Click action used in the button Mostrar Dados (Tab Tabela Acessos)
         * show the table TabelaAcessos
         **/
        private void btnMostrarAcessos_Click(object sender, EventArgs e)
        {
            showDataBase("TabelaAcessos", dgvAcessos);
        }

        /**
         * btnSairAcessos_Click action used in the button Sair (Tab Tabela Acessos)
         * Exit the program
         **/
        private void btnSairAcessos_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
