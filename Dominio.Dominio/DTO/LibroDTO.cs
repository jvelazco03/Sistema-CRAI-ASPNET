using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio.DTO
{
    public class LibroDTO
    {
        public string CodigoLibro { get; set; }
        public string Titulo { get; set; }
        public string ISBN { get; set; }
        public string Editorial { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
