using MySqlConnector;
using System;
using MauiMYSQL.Models;

namespace MauiMYSQL.Models
{
    public class Pagamentos : Conecta
    {
        public int id_pagamento { get; set; }
        public int id_comanda { get; set; }
        public decimal valor_pago { get; set; }
        public string forma_pagamento { get; set; } = string.Empty;
        public DateTime data_hora_pagamento { get; set; }

        public bool RegistrarPagamento(int idComanda, decimal valorPago, string forma)
        {
            if (!Conexao()) return false;

            StrQuery = @"INSERT INTO Pagamentos
                            (id_comanda, valor_pago, forma_pagamento)
                            VALUES
                            (@id, @valor, @forma)";
            Cmd = new MySqlCommand(StrQuery, Conn);
            Cmd.Parameters.AddWithValue("@id", idComanda);
            Cmd.Parameters.AddWithValue("@valor", valorPago);
            Cmd.Parameters.AddWithValue("@forma", forma);

            try
            {
                Cmd.ExecuteNonQuery();

                // Fechar comanda após pagamento
                var comanda = new MesasComandas();
                comanda.FecharComanda(idComanda);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar pagamento: {ex.Message}");
                return false;
            }
            finally
            {
                FechaConexao();
            }
        }
    }
}

