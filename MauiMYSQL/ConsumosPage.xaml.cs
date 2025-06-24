using MauiMYSQL.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MauiMYSQL
{
    public partial class ConsumosPage : ContentPage
    {
        private MesasComandas comandaSelecionada;
        private List<Consumos> itensPendentes = new();

        Produtos produtosModel = new Produtos();
        List<Produtos> listaProdutos = new();

        public ConsumosPage(MesasComandas comanda)
        {
            InitializeComponent();

            // Busca a comanda com o status atualizado do banco
            // Isso é importante para verificar o status mais recente da comanda (Aberta/Fechada)
            comandaSelecionada = new MesasComandas().BuscarComandaPorId(comanda.id_comanda);
            lblComanda.Text = $"Comanda: {comandaSelecionada.nome_comanda}";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Carrega produtos para o Picker
            if (produtosModel.ConsultarProdutos())
            {
                listaProdutos = produtosModel.listaProdutos;
                pickerProdutos.ItemsSource = listaProdutos;
                pickerProdutos.ItemDisplayBinding = new Binding("nome_produto");
            }

            // Atualiza a visibilidade do botão 'Fechar Comanda'
            // O botão só deve aparecer se a comanda estiver "Aberta"
            btnFecharComanda.IsVisible = string.Equals(
                comandaSelecionada.status_comanda?.Trim(),
                "Aberta",
                StringComparison.OrdinalIgnoreCase
            );

            AtualizarListaVisual();
        }

        private void OnProdutoSelecionado(object sender, EventArgs e)
        {
            var produtoSelecionado = (Produtos)pickerProdutos.SelectedItem;
            if (produtoSelecionado != null)
            {
                entryValorUnitario.Text = produtoSelecionado.valor_unitario.ToString("F2");
            }
        }

        private void OnAdicionarConsumo(object sender, EventArgs e)
        {
            var produtoSelecionado = (Produtos)pickerProdutos.SelectedItem;

            if (comandaSelecionada == null || produtoSelecionado == null)
            {
                lblStatus.Text = "⚠️ Selecione um produto.";
                return;
            }

            if (string.IsNullOrWhiteSpace(entryQuantidade.Text) ||
                !int.TryParse(entryQuantidade.Text, out int quantidade) ||
                quantidade <= 0)
            {
                lblStatus.Text = "⚠️ Informe uma quantidade válida.";
                return;
            }

            var novo = new Consumos
            {
                id_comanda = comandaSelecionada.id_comanda,
                id_produto = produtoSelecionado.id_produto,
                // Certifica que o NomeProduto é atribuído para exibição na lista temporária
                NomeProduto = produtoSelecionado.nome_produto,
                quantidade = quantidade,
                valor_unitario = produtoSelecionado.valor_unitario
            };

            itensPendentes.Add(novo);

            lblStatus.Text = "📝 Adicionado à lista!";
            entryQuantidade.Text = string.Empty;
            entryValorUnitario.Text = string.Empty;
            pickerProdutos.SelectedIndex = -1; // Limpa a seleção do picker

            AtualizarListaVisual();
        }

        private void AtualizarListaVisual()
        {
            // Atribui null primeiro para forçar a atualização completa da CollectionView
            lstConsumosPendentes.ItemsSource = null;
            lstConsumosPendentes.ItemsSource = itensPendentes;

            decimal total = itensPendentes.Sum(c => c.Subtotal);
            lblTotal.Text = $"Total: R$ {total:F2}";
        }

        private void OnRemoverConsumo(object sender, EventArgs e)
        {
            if (sender is ImageButton btn &&
                btn.CommandParameter is Consumos item)
            {
                itensPendentes.Remove(item);
                lblStatus.Text = "🗑️ Item removido.";
                AtualizarListaVisual();
            }
        }

        private void OnConfirmarConsumos(object sender, EventArgs e)
        {
            if (itensPendentes.Count == 0)
            {
                lblStatus.Text = "⚠️ Nenhum item para confirmar.";
                return;
            }

            bool sucessoGeral = true;

            foreach (var item in itensPendentes)
            {
                var consumo = new Consumos();
                bool sucesso = consumo.AdicionarConsumo(
                    item.id_comanda,
                    item.id_produto,
                    item.quantidade,
                    item.valor_unitario
                );

                if (!sucesso)
                {
                    sucessoGeral = false;
                    // Opcional: Você pode adicionar uma lógica aqui para tentar novamente
                    // ou notificar qual item falhou, se for o caso.
                }
            }

            if (sucessoGeral)
            {
                lblStatus.Text = "✅ Consumos confirmados!";
                itensPendentes.Clear(); // Limpa a lista local após salvar no banco
                AtualizarListaVisual(); // Atualiza a UI para mostrar a lista vazia
            }
            else
            {
                lblStatus.Text = "❌ Erro ao confirmar alguns itens.";
            }
        }

        private async void OnFecharComanda(object sender, EventArgs e)
        {
            // Pergunta ao usuário se ele realmente deseja ir para a tela de fechamento.
            bool confirmado = await DisplayAlert("Fechar Comanda", "Deseja prosseguir para o fechamento desta comanda e visualizar o total a pagar?", "Sim", "Não");
            if (!confirmado)
            {
                return; // Se o usuário escolher "Não", a operação é cancelada.
            }

            // Navega para a FecharComandaPage, passando o ID e o nome da comanda selecionada.
            // A comanda só será efetivamente "fechada" no banco de dados na FecharComandaPage,
            // após o registro do pagamento.
            await Navigation.PushAsync(new FecharComandaPage(comandaSelecionada.id_comanda, comandaSelecionada.nome_comanda));
        }
    }
}