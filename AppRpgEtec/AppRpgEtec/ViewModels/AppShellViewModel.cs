using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Services.Usuarios;
using Azure.Storage.Blobs;

namespace AppRpgEtec.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {

        private UsuarioService usuarioService;
        private static string conexaoAzureStorage = "CHAVE";
        private static string container = "arquivos";

        private byte[] foto;
        public byte[] Foto
        {
            get => foto;
            set
            {
                foto = value;
                OnPropertyChanged();
            }
        }
        public AppShellViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            usuarioService = new UsuarioService(token);
            CarregarUsuarioAzure();
            
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
