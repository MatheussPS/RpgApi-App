using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRpgEtec.Models
{
    public class Endereco
    {
        public int Place_id {get; set;}
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Name { get; set; }
        public string Display_name { get; set; }
    }
}
