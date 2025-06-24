using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MauiMYSQL.Models
{
    public class Consumos : Conecta
    {
        public int id_consumo { get; set; }
        public int id_comanda { get; set; }
        public int id_produto { get; set; }
        public int quantidade { get; set; }
        public decimal valor_unitario { get; set; }
        public DateTime data_hora_lancamento { get; set; }

        // Exibição
        public string NomeProduto { get; set; } = string.Empty;
        public string NomeComanda { get; set; } = string.Empty;

        // Calculado em tempo real
        public decimal Subtotal => quantidade * valor_unitario;

        public List<Consumos> listaConsumos { get; set; } = new();

        public Consumos() { }

        public bool AdicionarConsumo(int idComanda, int idProduto, int quantidade, decimal valorUnitario)
        {
            if (!Conexao()) return false;

            StrQuery = @"INSERT INTO Consumos
                            (id_comanda, id_produto, quantidade, valor_unitario)
                            VALUES
                            (@idComanda, @idProduto, @quantidade, @valorUnitario)";
            Cmd = new MySqlCommand(StrQuery, Conn);
            Cmd.Parameters.AddWithValue("@idComanda", idComanda);
            Cmd.Parameters.AddWithValue("@idProduto", idProduto);
            Cmd.Parameters.AddWithValue("@quantidade", quantidade);
            Cmd.Parameters.AddWithValue("@valorUnitario", valorUnitario);

            try
            {
                Cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar consumo: {ex.Message}");
                return false;
            }
            finally
            {
                FechaConexao();
            }
        }

        public bool ConsultarConsumosPorComanda(int idComanda)
        {
            if (!Conexao()) return false;

            StrQuery = @"
                SELECT C.*,
                        P.nome_produto,
                        MC.nome_comanda
                FROM Consumos C
                JOIN Produtos P ON C.id_produto = P.id_produto
                JOIN MesasComandas MC ON C.id_comanda = MC.id_comanda
                WHERE C.id_comanda = @idComanda
                ORDER BY C.data_hora_lancamento";

            MySqlCommand cmd = new MySqlCommand(StrQuery, Conn);
            cmd.Parameters.AddWithValue("@idComanda", idComanda);

            Dr = cmd.ExecuteReader();
            listaConsumos.Clear();

            while (Dr.Read())
            {
                listaConsumos.Add(new Consumos
                {
                    id_consumo = Dr.GetInt32("id_consumo"),
                    id_comanda = Dr.GetInt32("id_comanda"),
                    id_produto = Dr.GetInt32("id_produto"),
                    quantidade = Dr.GetInt32("quantidade"),
                    valor_unitario = Dr.GetDecimal("valor_unitario"),
                    data_hora_lancamento = Dr.GetDateTime("data_hora_lancamento"),
                    NomeProduto = Dr.GetString("nome_produto"),
                    NomeComanda = Dr.GetString("nome_comanda")
                });
            }

            FechaConexao();
            return true;
        }

        public decimal CalcularTotalComanda(int idComanda)
        {
            if (!Conexao()) return 0;

            StrQuery = @"SELECT SUM(quantidade * valor_unitario) AS Total
                            FROM Consumos
                            WHERE id_comanda = @idComanda";

            MySqlCommand cmd = new MySqlCommand(StrQuery, Conn);
            cmd.Parameters.AddWithValue("@idComanda", idComanda);

            try
            {
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao calcular total: {ex.Message}");
                return 0;
            }
            finally
            {
                FechaConexao();
            }
        }
    }
}
