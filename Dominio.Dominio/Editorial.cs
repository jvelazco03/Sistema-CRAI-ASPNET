using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Editorial
    {
        [Display(Name = "Codigo"), Required] public int CodigoEditorial { get; set; }
        [Display(Name = "Editorial"), Required] public string Nombre { get; set; }
        [Display(Name = "Pais"), Required] public string Pais { get; set; }
    }
}
