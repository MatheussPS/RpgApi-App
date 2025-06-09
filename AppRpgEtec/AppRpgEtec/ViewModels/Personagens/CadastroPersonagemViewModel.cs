﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Models;
using AppRpgEtec.Services;
using AppRpgEtec.Models.Enuns;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AppRpgEtec.ViewModels.Personagens
{
    [QueryProperty("PersonagemSelecionadoId", "pId")]
    public class CadastroPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;
        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }

        public CadastroPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            _ = ObterClasse();

            SalvarCommand = new Command(async () => { await SalvarPersonagem(); });
            CancelarCommand = new Command(async () => CancelarCadastro());
        }

        private int id;
        private string nome;
        private int pontosVida;
        private int forca;
        private int defesa;
        private int inteligencia;
        private int disputas;
        private int vitorias;
        private int derrotas;
        private TipoClasse tipoClasseSelecionado;

        public int Id { get => id; set { id = value; OnPropertyChanged(); } }
        public string Nome { get => nome; set { nome = value; OnPropertyChanged(); } }
        public int PontosVida { get => pontosVida; set { pontosVida = value; OnPropertyChanged(); } }
        public int Forca { get => forca; set { forca = value; OnPropertyChanged(); } }
        public int Defesa { get => defesa; set { defesa = value; OnPropertyChanged(); } }
        public int Inteligencia { get => inteligencia; set { inteligencia = value; OnPropertyChanged(); } }
        public int Disputas { get => disputas; set { disputas = value; OnPropertyChanged(); } }
        public int Vitorias { get => vitorias; set { vitorias = value; OnPropertyChanged(); } }
        public int Derrotas { get => derrotas; set { derrotas = value; OnPropertyChanged(); } }
        
        private ObservableCollection<TipoClasse> listaTipoClasse;
        public ObservableCollection<TipoClasse> ListaTipoClasse 
        { 
            get { return listaTipoClasse; } 
            set 
            { 
                listaTipoClasse = value; 
                if(value != null)
                {
                    listaTipoClasse = value;
                    OnPropertyChanged();
                }
            }
        }

        public TipoClasse TipoClasseSelecionado { get {return tipoClasseSelecionado; } set { if(value != null) {tipoClasseSelecionado = value; OnPropertyChanged(); } } }

        public async Task ObterClasse()
        {
            try
            {
                ListaTipoClasse = new ObservableCollection<TipoClasse>();
                ListaTipoClasse.Add(new TipoClasse() { Id = 1, Descricao = "Cavaleiro" });
                ListaTipoClasse.Add(new TipoClasse() { Id = 2, Descricao = "Mago" });
                ListaTipoClasse.Add(new TipoClasse() { Id = 3, Descricao = "Clerigo" });
                OnPropertyChanged(nameof(ListaTipoClasse));
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task SalvarPersonagem()
        {
            try
            {
                Personagem personagem = new Personagem()
                {
                    Nome = this.nome,
                    PontosVida = this.pontosVida,
                    Defesa = this.defesa,
                    Derrotas = this.derrotas,
                    Disputas = this.disputas,
                    Forca = this.forca,
                    Inteligencia = this.inteligencia,
                    Vitorias = this.vitorias,
                    Id = this.id,
                    Classe = (ClasseEnum)tipoClasseSelecionado.Id
                };
                if (personagem.Id == 0)
                    await pService.PostPersonagemAsync(personagem);

                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos!", "Ok");
                await Shell.Current.GoToAsync("..");

            }
            catch (Exception ex) {
                Application.Current.MainPage.DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }
        public async void CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async void CarregarPersonagem()
        {
            try
            {
                Personagem p = await pService.GetPersonagemAsync(int.Parse(PersonagemSelecionadoId));

                Nome = p.Nome;
                PontosVida = p.PontosVida;
                Defesa = p.Defesa;
                Derrotas = p.Derrotas;
                Disputas = p.Disputas;
                Forca = p.Forca;
                Inteligencia = p.Inteligencia;
                Vitorias = p.Vitorias;
                Id = p.Id;

                TipoClasseSelecionado = this.ListaTipoClasse.FirstOrDefault(tipo => tipo.Id == (int)p.Classe);
            }
            catch (Exception ex){
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}
