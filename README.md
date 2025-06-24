# Sistema de Gerenciamento de Comandas para Bar (MAUI + MySQL)

## Contexto Acadêmico

Este projeto foi desenvolvido como um **trabalho individual** para as disciplinas de **Laboratório de Programação para Dispositivos Móveis** (com objetivo de avaliação e recuperação) e **Computação em Nuvem I** (como avaliação do 2º bimestre).

**Enunciado do Trabalho:**
Criar um aplicativo mobile que realize a inclusão de dados e uma consulta, retornando os dados de um banco de dados MySQL hospedado em uma máquina virtual Azure. O desenvolvimento foi feito utilizando .NET MAUI, baseando-se no projeto de referência disponibilizado em sala de aula (`https://github.com/alessandro-fukuta/MauiMYSQL.git`). O tema do projeto foi escolhido como "Gerenciamento de Comandas para Bar".

## Sumário

* [Visão Geral](#visão-geral)
* [Funcionalidades](#funcionalidades)
* [Pré-requisitos](#pré-requisitos)
* [Configuração do Ambiente](#configuração-do-ambiente)
    * [Banco de Dados (MySQL)](#banco-de-dados-mysql)
    * [Projeto .NET MAUI](#projeto-net-maui)
* [Como Usar](#como-usar)
* [Estrutura do Projeto](#estrutura-do-projeto)
* [Melhorias Futuras (Opcional)](#melhorias-futuras-opcional)

---

## Visão Geral

O objetivo deste projeto é demonstrar a integração entre um aplicativo Multiplataforma (.NET MAUI) e um banco de dados MySQL. Ele simula as operações básicas de um bar ou restaurante que utiliza comandas para controlar os pedidos e pagamentos dos clientes.

## Funcionalidades

* **Abertura de Comandas:** Criar novas comandas, identificadas por um nome (ou número) e com status "Aberta".
* **Adição de Consumos:** Lançar produtos consumidos em uma comanda específica, com quantidade e valor unitário.
* **Fechamento de Comandas:** Visualizar o extrato total de uma comanda, selecionar a forma de pagamento e encerrá-la, alterando seu status para "Fechada".
* **Listagem de Comandas:** Visualizar todas as comandas (abertas e fechadas), com seus respectivos valores totais e status, ordenadas alfabeticamente.

## Pré-requisitos

Antes de configurar e rodar o projeto, certifique-se de ter o seguinte instalado:

* **Visual Studio 2022:** Com a carga de trabalho ".NET Multi-platform App UI development" (MAUI) instalada.
* **SDK do .NET:** Compatível com a versão usada pelo MAUI (geralmente .NET 6, 7 ou 8).
* **MySQL Server:** Uma instância do MySQL rodando (local ou **em uma Máquina Virtual Azure**, conforme requisito do trabalho).
* **MySQL Workbench** (ou outro cliente MySQL): Para criar o banco de dados e as tabelas.
* **Pacote NuGet `MySqlConnector`**: Será adicionado ao projeto MAUI para conexão com o MySQL.

## Configuração do Ambiente

### Banco de Dados (MySQL)

1.  **Crie um Banco de Dados:**
    No seu cliente MySQL (ex: MySQL Workbench), execute o seguinte comando para criar o banco de dados:
    ```sql
    CREATE DATABASE bar_caixa_db;
    USE bar_caixa_db;
    ```

2.  **Crie as Tabelas:**
    Execute os comandos SQL abaixo para criar as tabelas `MesasComandas`, `Produtos`, `Consumos` e `Pagamentos`:

    ```sql
    -- Tabela MesasComandas
    CREATE TABLE MesasComandas (
        id_comanda INT AUTO_INCREMENT PRIMARY KEY,
        nome_comanda VARCHAR(100) NOT NULL UNIQUE,
        data_abertura DATETIME DEFAULT CURRENT_TIMESTAMP,
        status_comanda ENUM('Aberta', 'Fechada') DEFAULT 'Aberta',
        observacoes VARCHAR(255) NULL
    );

    -- Tabela Produtos
    CREATE TABLE Produtos (
        id_produto INT AUTO_INCREMENT PRIMARY KEY,
        nome_produto VARCHAR(100) NOT NULL UNIQUE,
        descricao VARCHAR(255) NULL,
        valor_unitario DECIMAL(10, 2) NOT NULL
    );

    -- Tabela Consumos
    CREATE TABLE Consumos (
        id_consumo INT AUTO_INCREMENT PRIMARY KEY,
        id_comanda INT NOT NULL,
        id_produto INT NOT NULL,
        quantidade INT NOT NULL,
        valor_unitario DECIMAL(10, 2) NOT NULL,
        data_hora_lancamento DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (id_comanda) REFERENCES MesasComandas(id_comanda),
        FOREIGN KEY (id_produto) REFERENCES Produtos(id_produto)
    );

    -- Tabela Pagamentos
    CREATE TABLE Pagamentos (
        id_pagamento INT AUTO_INCREMENT PRIMARY KEY,
        id_comanda INT NOT NULL,
        valor_pago DECIMAL(10, 2) NOT NULL,
        forma_pagamento VARCHAR(50) NOT NULL,
        data_hora_pagamento DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (id_comanda) REFERENCES MesasComandas(id_comanda)
    );
    ```

3.  **Adicione Produtos de Exemplo (Opcional):**
    Você pode inserir alguns produtos para testar:
    ```sql
    INSERT INTO Produtos (nome_produto, descricao, valor_unitario) VALUES
    ('Cerveja Pilsen 350ml', 'Cerveja Pilsen em lata 350ml', 6.50),
    ('Porção de Batata Frita', 'Porção generosa de batata frita com molho', 28.00),
    ('Refrigerante Cola', 'Refrigerante sabor cola 500ml', 7.00),
    ('Caipirinha Limão', 'Clássica caipirinha de limão com cachaça', 22.00),
    ('Combo Frios', 'Mix de queijos e embutidos', 45.00),
    ('Suco de Laranja Natural', 'Suco de laranja fresco', 12.00);
    ```

### Projeto .NET MAUI

1.  **Abra o Projeto no Visual Studio 2022.**

2.  **Instale o Pacote NuGet `MySqlConnector`:**
    * No **Gerenciador de Soluções**, clique com o botão direito do mouse no seu projeto MAUI (`MauiMYSQL`).
    * Selecione **"Gerenciar Pacotes NuGet..."**.
    * Vá para a aba **"Procurar"** (Browse), digite `MySqlConnector` e clique em **"Instalar"**.

3.  **Configure a Conexão no `Models/Conecta.cs`:**
    * Esta classe é a base para a conexão com o banco de dados, centralizando a lógica de conexão.
    * Abra o arquivo `Models/Conecta.cs`.
    * Certifique-se de que a linha `using MySqlConnector;` está no topo.
    * Atualize a linha `connectionString` com os dados do seu servidor MySQL:
        ```csharp
        public class Conecta
        {
            // Variáveis e objetos para gerenciar a conexão e comandos SQL
            protected MySql.Data.MySqlClient.MySqlConnection Conn;
            protected MySql.Data.MySqlClient.MySqlCommand Cmd;
            protected MySql.Data.MySqlClient.MySqlDataReader Dr;
            protected string StrQuery = string.Empty;
            // ATENÇÃO: SUBSTITUA COM SEUS DADOS REAIS DO BANCO DE DADOS
            protected string connectionString = "Server=SEU_SERVIDOR_MYSQL;Port=3306;Database=bar_caixa_db;Uid=SEU_USUARIO;Pwd=SUA_SENHA;";
            // Exemplo para MySQL em VM Azure: "Server=SEU_IP_PUBLICO_AZURE;Port=3306;Database=bar_caixa_db;Uid=SEU_USUARIO_MYSQL;Pwd=SUA_SENHA_MYSQL;";
            // Lembre-se de configurar as regras de firewall na VM Azure para permitir a conexão.

            // Métodos Conexao() e FechaConexao() também devem estar nesta classe.
            protected bool Conexao() { /* ... */ return true; }
            protected void FechaConexao() { /* ... */ }
        }
        ```
    * **Importante:** Todas as suas outras classes de modelo (`Consumos.cs`, `MesasComandas.cs`, `Pagamentos.cs`, `Produtos.cs`) devem **herdar** de `Conecta` (ex: `public class Consumos : Conecta`). Elas devem utilizar os membros e métodos definidos em `Conecta` (como `Conn`, `Cmd`, `Dr`, `Conexao()`, `FechaConexao()`) para interagir com o banco de dados.
    * Garanta que apenas `using MySqlConnector;` esteja no topo desses arquivos de modelo.

4.  **Limpar e Recompilar a Solução:**
    * No Visual Studio, vá em **"Compilar" (Build)** > **"Limpar Solução" (Clean Solution)**.
    * Em seguida, vá em **"Compilar" (Build)** > **"Recompilar Solução" (Rebuild Solution)**.

## Como Usar

1.  Após a compilação bem-sucedida, execute o aplicativo no emulador ou dispositivo desejado.
2.  Na tela inicial, você poderá abrir novas comandas.
3.  Selecione uma comanda aberta para adicionar consumos, escolhendo produtos da lista.
4.  Na tela de fechamento, visualize o extrato, selecione a forma de pagamento e finalize a comanda.
5.  A lista de comandas mostrará as comandas abertas e fechadas, ordenadas alfabeticamente.

## Estrutura do Projeto

* **`MainPage.xaml` / `MainPage.xaml.cs`**: Tela principal do aplicativo.
* **`ConsumosPage.xaml` / `ConsumosPage.xaml.cs`**: Tela para adicionar consumos às comandas.
* **`FecharComandaPage.xaml` / `FecharComandaPage.xaml.cs`**: Tela para finalizar o pagamento de uma comanda.
* **`Models/`**: Pasta que contém as classes de modelo e lógica de acesso ao banco de dados (`Conecta.cs`, `MesasComandas.cs`, `Produtos.cs`, `Consumos.cs`, `Pagamentos.cs`).

## Melhorias Futuras (Opcional)

* Implementação de login/usuário.
* Geração de relatórios de vendas.
* Funcionalidade de reabrir comanda fechada.
* Gerenciamento de estoque de produtos.
* Interface do usuário mais rica e responsiva.
* Tratamento de erros mais robusto com mensagens amigáveis ao usuário.
* Opção para editar ou remover consumos já lançados na comanda.
