using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Libro
    {
        [Display(Name = "Codigo"), Required] public string CodigoLibro { get; set; }
        [Display(Name = "Titulo"), Required] public string Titulo { get; set; }
        [Display(Name = "ISBN"), Required] public string ISBN { get; set; }
        [Display(Name = "Codigo de Editorial"), Required] public int CodigoEditorial { get; set; }
        [Display(Name = "Editorial"), Required] public string Editorial { get; set; }
        [Display(Name = "Precio"), Required] public decimal Precio { get; set; }
        [Display(Name = "Stock"), Required] public int Stock { get; set; }
    }
}
