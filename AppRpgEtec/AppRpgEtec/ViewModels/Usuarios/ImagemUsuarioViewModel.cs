using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using Azure.Storage.Blobs;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class ImagemUsuarioViewModel : BaseViewModel
    {

        private UsuarioService uService;
        private static string conexaoAzureStorage = "";
        private static string container = "arquivos";

        private ImageSource fonteImagem;

        private byte[] foto;

        public ImageSource FonteImagem
        {
            get { return fonteImagem; }
            set
            {
                fonteImagem = value;
                OnPropertyChanged();
            }
        }

        public byte[] Foto
        {
            get => foto;
            set
            {
                foto = value;
                OnPropertyChanged();
            }
        }


        public ImagemUsuarioViewModel()
        {

            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);

            FotografarCommand = new Command(Fotografar);
            SalvarImagemCommand = new Command(SalvarImagemAzure);
            AbrirGaleriaCommand = new Command(AbrirGaleria);
            CarregarUsuarioAzure();
        }

        public ICommand FotografarCommand { get; }
        public ICommand SalvarImagemCommand { get; }

        public ICommand AbrirGaleriaCommand { get; }


        public async void Fotografar()
        {
            try
            {

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {

                        using (Stream sourceStream = await photo.OpenReadAsync())
                        {

                            using (MemoryStream ms = new MemoryStream())
                            {

                                await sourceStream.CopyToAsync(ms);

                                Foto = ms.ToArray();

                                FonteImagem = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));


                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {

                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");

            }
        }

        public async void SalvarImagemAzure()
        {
            try
            {
                Usuario u = new Usuario();
                u.Foto = foto;
                u.Id = Preferences.Get("UsuarioId", 0);

                string filename = $"{u.Id}.jpg";

                var blobClient = new BlobClient(conexaoAzureStorage, container, filename);

                if (blobClient.Exists()) blobClient.Delete();

                using (var stream = new MemoryStream(u.Foto))
                {
                    blobClient.Upload(stream);
                }
                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");
                await App.Current.MainPage.Navigation.PopAsync();

            }
            catch (Exception e)
            {
                await Application.Current.MainPage
                    .DisplayAlert("OPS!", e.Message + "Detalhes: " + e.InnerException, "Ok");
            }
        }

        public async void AbrirGaleria()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                    if (photo != null)
                    {
                        using (Stream sourceStream = await photo.OpenReadAsync()) ;
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage
                    .DisplayAlert("OPS!", e.Message + "Detalhes: " + e.InnerException, "Ok");
            }
        }

        public async void CarregarUsuarioAzure()
        {
            try
            {
                int usuarioID = Preferences.Get("UsuarioId", 0);
                string filename = $"{usuarioID}.jpg";
                var blobClient = new BlobClient(conexaoAzureStorage, container, filename);

                if (blobClient.Exists())
                {
                    Byte[] fileBytes;

                    using (MemoryStream ms = new MemoryStream())
                    {

                        blobClient.OpenRead().CopyTo(ms);
                        fileBytes = ms.ToArray();

                    }
                    Foto = fileBytes;
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", e.Message + "Detalhes" + e.InnerException, "Ok");
            }
        }
    }
}
