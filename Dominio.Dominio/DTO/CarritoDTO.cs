using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio.DTO
{
    public class CarritoDTO
    {
        public string CodigoLibro { get; set; }
        public string Titulo { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        
        // Propiedad calculada
        public decimal Importe 
        { 
            get { return Precio * Cantidad; } 
        }
    }
}
