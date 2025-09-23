using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Models;

namespace AppRpgEtec.Services.Enderecos
{
    public class EnderecoService: Request
    {
        private readonly string _token;
        private readonly Request _request;
        private const string apiUrlBase = "https://nominatim.openstreetmap.org/search?format=json&q=";
        public EnderecoService(string token) { 
            _request = new Request();
            _token = token;
        }

        public async Task<Endereco> GetCepAsync(string cep)
        {
            string urlComplementar = string.Format("/{0}", cep);
            var endereco = await _request.GetAsync<Models.Endereco>(apiUrlBase + urlComplementar, _token);
            return endereco;
        }
    }
}
