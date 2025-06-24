using System;
using MySqlConnector;

namespace MauiMYSQL.Models
{
    public class Conecta
    {
        public string conexao_status { get; set; } = string.Empty;
        public string StrQuery { get; set; } = string.Empty;
        public string StrCon { get; set; } = string.Empty;

        public MySqlDataReader Dr { get; set; } = default!;
        public MySqlCommand Cmd { get; set; } = default!;
        public MySqlConnection Conn { get; set; } = default!;

        public Conecta()
        {
        }

        public bool Conexao()
        {
            // Constrói a string de conexão para a VM na Azure
            MySqlConnectionStringBuilder StrCon = new MySqlConnectionStringBuilder
            {
                Server = "48.211.167.75",
                Port = 3306,
                UserID = "pauliane",
                Password = "FatecEstude123",
                Database = "bar_caixa_db"
            };

            Conn = new MySqlConnection(StrCon.ToString());
            bool ret = false;

            try
            {
                Conn.Open();
                conexao_status = "Conexão realizada com sucesso!";
                ret = true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                conexao_status = ex.Message;
                ret = false;
            }

            return ret;
        }

        public void FechaConexao()
        {
            if (Conn != null && Conn.State == System.Data.ConnectionState.Open)
            {
                Conn.Close();
            }
        }
    }
}
