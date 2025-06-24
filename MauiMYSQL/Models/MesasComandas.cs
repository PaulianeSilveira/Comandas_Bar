using MySqlConnector;
using System;
using System.Collections.Generic;
using MauiMYSQL.Models;

namespace MauiMYSQL.Models
{
    public class MesasComandas : Conecta
    {
        public int id_comanda { get; set; }
        public string nome_comanda { get; set; } = string.Empty;
        public DateTime data_abertura { get; set; }
        public string status_comanda { get; set; } = string.Empty;
        public string? observacoes { get; set; }

        public List<MesasComandas> listaMesasComandas = new();

        public MesasComandas() { }

        public bool AbrirComanda(string nomeComanda, string? observacoes = null)
        {
            if (!Conexao()) return false;

            StrQuery = @"INSERT INTO MesasComandas (nome_comanda, observacoes, status_comanda)
                         VALUES (@nome, @obs, 'Aberta')";

            Cmd = new MySqlCommand(StrQuery, Conn);
            Cmd.Parameters.AddWithValue("@nome", nomeComanda);
            Cmd.Parameters.AddWithValue("@obs", observacoes);

            try
            {
                Cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir comanda: {ex.Message}");
                return false;
            }
            finally
            {
                FechaConexao();
            }
        }

        public bool ConsultarComandasAbertas()
        {
            if (!Conexao()) return false;

            StrQuery = @"SELECT *
                         FROM MesasComandas
                         WHERE status_comanda = 'Aberta'
                         ORDER BY nome_comanda";
            Cmd = new MySqlCommand(StrQuery, Conn);
            Dr = Cmd.ExecuteReader();
            listaMesasComandas.Clear();

            while (Dr.Read())
            {
                listaMesasComandas.Add(new MesasComandas
                {
                    id_comanda = Dr.GetInt32("id_comanda"),
                    nome_comanda = Dr.GetString("nome_comanda"),
                    data_abertura = Dr.GetDateTime("data_abertura"),
                    status_comanda = Dr.GetString("status_comanda"),
                    observacoes = Dr.IsDBNull(Dr.GetOrdinal("observacoes")) ? null : Dr.GetString("observacoes")
                });
            }

            FechaConexao();
            return true;
        }

        public bool ConsultarTodasComandas()
        {
            if (!Conexao()) return false;

            StrQuery = @"SELECT *
                         FROM MesasComandas
                         ORDER BY data_abertura DESC";
            Cmd = new MySqlCommand(StrQuery, Conn);
            Dr = Cmd.ExecuteReader();
            listaMesasComandas.Clear();

            while (Dr.Read())
            {
                listaMesasComandas.Add(new MesasComandas
                {
                    id_comanda = Dr.GetInt32("id_comanda"),
                    nome_comanda = Dr.GetString("nome_comanda"),
                    data_abertura = Dr.GetDateTime("data_abertura"),
                    status_comanda = Dr.GetString("status_comanda"),
                    observacoes = Dr.IsDBNull(Dr.GetOrdinal("observacoes")) ? null : Dr.GetString("observacoes")
                });
            }

            FechaConexao();
            return true;
        }

        public MesasComandas BuscarComandaPorId(int id)
        {
            var comanda = new MesasComandas();

            StrQuery = "SELECT * FROM MesasComandas WHERE id_comanda = @id";
            if (!Conexao()) return comanda;

            Cmd = new MySqlConnector.MySqlCommand(StrQuery, Conn);
            Cmd.Parameters.AddWithValue("@id", id);

            Dr = Cmd.ExecuteReader();
            if (Dr.Read())
            {
                comanda.id_comanda = Dr.GetInt32("id_comanda");
                comanda.nome_comanda = Dr.GetString("nome_comanda");
                comanda.data_abertura = Dr.GetDateTime("data_abertura");
                comanda.status_comanda = Dr.GetString("status_comanda"); // ESSENCIAL!
            }

            FechaConexao();
            return comanda;
        }

        public bool FecharComanda(int idComanda)
        {
            if (!Conexao()) return false;

            StrQuery = @"UPDATE MesasComandas
                         SET status_comanda = 'Fechada'
                         WHERE id_comanda = @id";
            Cmd = new MySqlCommand(StrQuery, Conn);
            Cmd.Parameters.AddWithValue("@id", idComanda);

            try
            {
                Cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fechar comanda: {ex.Message}");
                return false;
            }
            finally
            {
                FechaConexao();
            }
        }
    }
}
