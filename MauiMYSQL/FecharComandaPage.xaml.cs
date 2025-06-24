using MauiMYSQL.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq; // Importante para usar o .Sum()

namespace MauiMYSQL
{
    public partial class FecharComandaPage : ContentPage
    {
        private int idComanda;
        private string nomeComanda;
        private List<Consumos> listaConsumos = new();

        public FecharComandaPage(int idComanda, string nomeComanda)
        {
            InitializeComponent();
            this.idComanda = idComanda;
            this.nomeComanda = nomeComanda;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            lblTituloComanda.Text = $"Comanda: {nomeComanda}";
            CarregarConsumos();
        }

        private void CarregarConsumos()
        {
            var consumoModel = new Consumos();
            if (consumoModel.ConsultarConsumosPorComanda(idComanda))
            {
                listaConsumos = consumoModel.listaConsumos
                    .OrderBy(c => c.data_hora_lancamento) // Ordena para uma exibição mais organizada
                    .ToList();

                lstConsumos.ItemsSource = listaConsumos;

                decimal total = listaConsumos.Sum(c => c.Subtotal);
                lblTotal.Text = $"Total: R$ {total:F2}";
            }
            else
            {
                lblStatus.Text = "⚠️ Erro ao carregar consumos."; // Ícone de alerta mais claro
            }
        }

        private async void OnFinalizarPagamento(object sender, EventArgs e) // Método assíncrono para usar DisplayAlert
        {
            if (pickerFormaPagamento.SelectedIndex == -1)
            {
                lblStatus.Text = "⚠️ Selecione a forma de pagamento."; // Avisa se nada foi selecionado
                return;
            }

            decimal total = listaConsumos.Sum(c => c.Subtotal);
            // Uso do operador ?. e ?? para garantir que SelectedItem não é nulo antes de ToString()
            // e para fornecer um valor padrão "N/A" caso seja nulo.
            string forma = pickerFormaPagamento.SelectedItem?.ToString() ?? "N/A";

            var pagamento = new Pagamentos();
            // Chama o método RegistrarPagamento que cuida da inserção do pagamento
            // e da atualização do status da comanda para 'Fechada'.
            bool sucesso = pagamento.RegistrarPagamento(idComanda, total, forma);

            if (sucesso)
            {
                // Exibe uma mensagem de sucesso para o usuário
                await DisplayAlert("Sucesso", "Pagamento registrado! Comanda encerrada.", "OK");

                // Limpa os elementos visuais da tela de fechamento após o sucesso
                lblStatus.Text = "✅ Pagamento registrado! Comanda encerrada.";
                pickerFormaPagamento.SelectedIndex = -1; // Desseleciona a forma de pagamento no Picker
                lstConsumos.ItemsSource = null; // Remove os itens da lista exibida
                lblTotal.Text = "Total: R$ 0,00"; // Zera o valor total na tela

                // Retorna para a página anterior (MesasComandasPage),
                // onde a comanda que acabou de ser fechada não deverá mais aparecer.
                await Navigation.PopAsync();
            }
            else
            {
                // Se houver qualquer falha no registro do pagamento, informa o usuário
                lblStatus.Text = "❌ Erro ao registrar o pagamento.";
            }
        }
    }
}