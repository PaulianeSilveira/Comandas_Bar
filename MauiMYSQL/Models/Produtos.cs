using MySqlConnector;

using System;

using System.Collections.Generic;

using MauiMYSQL.Models;



namespace MauiMYSQL.Models

{

    public class Produtos : Conecta

    {

        public int id_produto { get; set; }

        public string nome_produto { get; set; } = string.Empty;

        public string? descricao { get; set; }

        public decimal valor_unitario { get; set; }



        public List<Produtos> listaProdutos = new();



        public Produtos() { }



        // Cadastra novo produto no cardápio

        public bool AdicionarProduto(string nome, string descricao, decimal valor)

        {

            if (!Conexao()) return false;



            StrQuery = @"INSERT INTO Produtos 

                         (nome_produto, descricao, valor_unitario) 

                         VALUES 

                         (@nome, @descricao, @valor)";

            Cmd = new MySqlCommand(StrQuery, Conn);

            Cmd.Parameters.AddWithValue("@nome", nome);

            Cmd.Parameters.AddWithValue("@descricao", descricao);

            Cmd.Parameters.AddWithValue("@valor", valor);



            try

            {

                Cmd.ExecuteNonQuery();

                return true;

            }

            catch (Exception ex)

            {

                Console.WriteLine($"Erro ao adicionar produto: {ex.Message}");

                return false;

            }

            finally

            {

                FechaConexao();

            }

        }



        // Retorna todos os produtos do cardápio

        public bool ConsultarProdutos()

        {

            if (!Conexao()) return false;



            StrQuery = @"SELECT * FROM Produtos ORDER BY nome_produto";

            Cmd = new MySqlCommand(StrQuery, Conn);

            Dr = Cmd.ExecuteReader();

            listaProdutos.Clear();



            while (Dr.Read())

            {

                listaProdutos.Add(new Produtos

                {

                    id_produto = Dr.GetInt32("id_produto"),

                    nome_produto = Dr.GetString("nome_produto"),

                    descricao = Dr.IsDBNull(Dr.GetOrdinal("descricao")) ? null : Dr.GetString("descricao"),

                    valor_unitario = Dr.GetDecimal("valor_unitario")

                });

            }



            FechaConexao();

            return true;

        }



        // Consulta um produto específico

        public Produtos? BuscarProdutoPorId(int idProduto)

        {

            if (!Conexao()) return null;



            StrQuery = "SELECT * FROM Produtos WHERE id_produto = @id";

            Cmd = new MySqlCommand(StrQuery, Conn);

            Cmd.Parameters.AddWithValue("@id", idProduto);

            Dr = Cmd.ExecuteReader();



            Produtos? produto = null;



            if (Dr.Read())

            {

                produto = new Produtos

                {

                    id_produto = Dr.GetInt32("id_produto"),

                    nome_produto = Dr.GetString("nome_produto"),

                    descricao = Dr.IsDBNull(Dr.GetOrdinal("descricao")) ? null : Dr.GetString("descricao"),

                    valor_unitario = Dr.GetDecimal("valor_unitario")

                };

            }



            FechaConexao();

            return produto;

        }

    }

}



