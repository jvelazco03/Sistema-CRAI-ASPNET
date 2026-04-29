using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Venta
    {
        [Display(Name = "Codigo"), Required] public int idVenta { get; set; }
        [Display(Name = "Libro"), Required] public string CodigoLibro { get; set; }
        [Display(Name = "Cantidad"), Required] public int Cantidad { get; set; }
        [Display(Name = "Precio Unitario"), Required] public decimal PrecioUnitario { get; set; }
        [Display(Name = "Editorial"), Required] public decimal Total { get; set; }
        [Display(Name = "Fecha de venta"), Required] public DateTime FechaVenta { get; set; }
    }
}
