//Matheus Pinter e Paulo Sergio 

using AppRpgEtec.Models;
using AppRpgEtec.Services.Disputas;
using AppRpgEtec.Services.Personagens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Services.PersonagemHabilidade;

namespace AppRpgEtec.ViewModels.Disputas
{
    public class DisputaViewModel : BaseViewModel
    {
        private PersonagemHabilidadeService phService;
        public ObservableCollection<PersonagemHabilidade> Habilidades { get; set; }

        private PersonagemService pService;
        public ObservableCollection<Personagem> Personagens { get; set; }
        public Personagem Atacante { get; set; }
        public Personagem Oponente { get; set; }
        private DisputaService dService;
        public Disputa DisputaPersonagens { get; set; }

        public DisputaViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            dService = new DisputaService(token);
            phService = new PersonagemHabilidadeService(token);

            Atacante = new Personagem();
            Oponente = new Personagem();
            DisputaPersonagens = new Disputa();

            Personagens = new ObservableCollection<Personagem>();
            PesquisarPersonagensCommand = new Command<string>(async (string pesquisa) => { await PesquisarPersonagens(pesquisa); });
            DisputaComArmaCommand = new Command(async () => { await ExecutarDisputaArmada(); });
            DisputaComHabilidadeCommand = new Command(async () => { await ExecutarDisputaHabilidades(); });
            DisputaGeralCommand = new Command(async () => { await ExecutarDisputaGeral(); });


        }

        public ICommand PesquisarPersonagensCommand { get; set; }
        public ICommand DisputaComArmaCommand { get; set; }
        public ICommand DisputaComHabilidadeCommand { get; set; }
        public ICommand DisputaGeralCommand { get; set; }


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

        public async Task ObterHabilidadesAsync(int personagemId)
        {
            try
            {
                Habilidades = await phService.GetPersonagemHabilidadesAsync(personagemId);
                OnPropertyChanged(nameof(Habilidades));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "OK");
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

        private PersonagemHabilidade habilidadeSelecionada;

        public PersonagemHabilidade HabilidadeSelecionada
        {
            get { return habilidadeSelecionada; }
            set
            {
                if (value != null)
                {
                    try
                    {
                        habilidadeSelecionada = value;
                        OnPropertyChanged();
                    }
                    catch (Exception ex)
                    {
                        Application.Current.MainPage.DisplayAlert("Ops", ex.Message, "OK");
                    }
                }
            }
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
                    await this.ObterHabilidadesAsync(p.Id);
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

        private async Task ExecutarDisputaArmada()
        {
            try
            {
                var disputa = new Disputa
                {
                    Narracao = "E então: ",
                    AtacanteId = Atacante.Id,
                    OponenteId = Oponente.Id
                };

                var resultado = await dService.PostDisputaComArmaAsync(disputa);

                await Application.Current.MainPage
                    .DisplayAlert("Resultado", resultado.Narracao, "Ok");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }


        private async Task ExecutarDisputaHabilidades()
        {
            try
            {
                DisputaPersonagens.Narracao = "E então: ";
                DisputaPersonagens.AtacanteId = Atacante.Id;
                DisputaPersonagens.OponenteId = Oponente.Id;
                DisputaPersonagens.HabilidadeId = habilidadeSelecionada.HabilidadeId;
                DisputaPersonagens = await dService.PostDisputaComHabilidadesAsync(DisputaPersonagens);

                await Application.Current.MainPage
                    .DisplayAlert("Resultado", DisputaPersonagens.Narracao, "Ok");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }
        private async Task ExecutarDisputaGeral()
        {
            try
            {
                DisputaPersonagens.Narracao = "E então: ";
                ObservableCollection<Personagem> lista = await pService.GetPersonagensAsync();
                DisputaPersonagens.ListaIdPersonagens = lista.Select(x => x.Id).ToList();

                DisputaPersonagens = await dService.PostDisputaGeralAsync(DisputaPersonagens);
                string resultados = string.Join(" | ", DisputaPersonagens.Resultados);

                await Application.Current.MainPage.DisplayAlert("Resultado", resultados, "OK");
            }
            catch (Exception ex) {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}
