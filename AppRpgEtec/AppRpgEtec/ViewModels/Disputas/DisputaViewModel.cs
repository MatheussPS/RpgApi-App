//Matheus Pinter e Paulo Sergio 
using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Disputas
{
    public class DisputaViewModel : BaseViewModel
    {

        private PersonagemService pService;
        public ObservableCollection<Personagem> Personagens { get; set; }
        public Personagem Atacante { get; set; }
        public Personagem Oponente { get; set; }


        public DisputaViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);

            Atacante = new Personagem();
            Oponente = new Personagem();

            Personagens = new ObservableCollection<Personagem>();
            PesquisarPersonagensCommand = new Command<string>(async (string pesquisa) => { await PesquisarPersonagens(pesquisa); });
        }

        public ICommand PesquisarPersonagensCommand { get; set; }

        private async Task PesquisarPersonagens(string pesquisa)
        {
            try
            {
                var resultado = await pService.GetByNomeAproximadoAsync(pesquisa);

                if (resultado != null)
                {
                    Personagens.Clear();
                    foreach (var p in resultado)
                        Personagens.Add(p);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao pesquisar personagens: {ex.Message}", "OK");
            }
        }

        public string DescricaoAtacante
        {
            get => Atacante.Nome;
        }

        public string DescricaoOponente
        {
            get => Oponente.Nome;
        }

        private Personagem personagemSelecionado;


        public Personagem PersonagemSelecionado
        {
            set
            {
                if (value != null) {
                    personagemSelecionado = value;
                    SelecionarPersonagem(personagemSelecionado);
                    OnPropertyChanged();
                    Personagens.Clear();
                }
            }
        }

        public string textoBuscaDigitado = string.Empty;

        public string TextoBuscaDigitado
        {
            get { return textoBuscaDigitado; }
            set
            {
                if(value != null && !string.IsNullOrEmpty(value) && value.Length > 0)
                {
                    textoBuscaDigitado = value;
                    _ =  PesquisarPersonagens(textoBuscaDigitado);
                }
                else
                {
                    Personagens.Clear();
                }
            }
        }

        public async void SelecionarPersonagem(Personagem p)
        {
            try
            {
                string tipoCombatente = await Application.Current.MainPage.DisplayActionSheet("Atacante ou Oponente?", "Cancelar", "", "Atacante", "Oponente");
            
                if(tipoCombatente == "Atacante")
                {
                    Atacante = p;
                    OnPropertyChanged(nameof(DescricaoAtacante));
                }
                else if (tipoCombatente == "Oponente")
                {
                    Oponente = p;
                    OnPropertyChanged(nameof(DescricaoOponente));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }

        }
    }
}
