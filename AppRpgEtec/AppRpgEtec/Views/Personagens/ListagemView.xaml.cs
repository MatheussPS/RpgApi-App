namespace AppRpgEtec.Views.Personagens;
using AppRpgEtec.ViewModels.Personagens;

public partial class ListagemView : ContentPage
{
	ListaPersonagemViewModel viewModel;
	public ListagemView()
	{
		InitializeComponent();

		viewModel = new ListaPersonagemViewModel();
		BindingContext = viewModel;
		Title = "Personagens -App Rpg";
	}
}