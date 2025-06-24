using MauiMYSQL.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace MauiMYSQL
{
    public partial class MesasComandasPage : ContentPage
    {
        MesasComandas comandoModel = new MesasComandas();
        List<MesasComandas> listaComandas = new();

        public MesasComandasPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // CHAVE DA CORREÇÃO: Força a atualização da lista SEMPRE que a página se torna visível.
            AtualizarLista();
        }

        private void AtualizarLista()
        {
            if (comandoModel.ConsultarComandasAbertas())
            {
                listaComandas = comandoModel.listaMesasComandas;

                // Força a CollectionView a se renderizar novamente com os dados atualizados.
                // Atribuir null primeiro e depois a nova lista garante que o UI seja totalmente redesenhado.
                lstMesasComandas.ItemsSource = null;
                lstMesasComandas.ItemsSource = listaComandas;

                lblStatus.Text = $"Carregadas: {listaComandas.Count} comandas.";
            }
            else
            {
                lblStatus.Text = "⚠️ Erro ao carregar comandas.";
            }
        }

        private void OnAbrirComandaClicked(object sender, EventArgs e)
        {
            string nome = txtNomeComanda.Text?.Trim();
            string obs = txtObservacoes.Text?.Trim();

            if (string.IsNullOrEmpty(nome))
            {
                lblStatus.Text = "⚠️ Informe o nome da comanda.";
                return;
            }

            bool sucesso = comandoModel.AbrirComanda(nome, obs);
            if (sucesso)
            {
                txtNomeComanda.Text = "";
                txtObservacoes.Text = "";
                lblStatus.Text = "✅ Comanda aberta com sucesso!";
                AtualizarLista(); // Também atualiza a lista após abrir uma nova comanda
            }
            else
            {
                lblStatus.Text = "❌ Não foi possível abrir a comanda.";
            }
        }

        private async void OnComandaTocada(object sender, EventArgs e)
        {
            if (sender is VisualElement element &&
                element.BindingContext is MesasComandas comandaTocada)
            {
                await Navigation.PushAsync(new ConsumosPage(comandaTocada));
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível identificar a comanda tocada.", "OK");
            }
        }
    }
}
