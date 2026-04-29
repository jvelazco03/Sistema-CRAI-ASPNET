using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dominio.Dominio.DTO;

namespace Gestion_de_Biblioteca.Models
{
    public class VentaViewModel
    {
        // Lista de items agregados al carrito
        public List<CarritoDTO> LineasCarrito { get; set; }
        
        // Propiedad calculada del total de la venta entera
        public decimal TotalVenta 
        {
            get 
            { 
                if (LineasCarrito != null && LineasCarrito.Any())
                {
                    return LineasCarrito.Sum(x => x.Importe);
                }
                return 0;
            }
        }

        public VentaViewModel()
        {
            LineasCarrito = new List<CarritoDTO>();
        }
    }
}
