using Dominio.Dominio;
using System.Collections.Generic;

namespace Gestion_de_Biblioteca.Models
{
    public class LibroPaginadoViewModel
    {
        public IEnumerable<Dominio.Dominio.DTO.LibroDTO> Libros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public string SearchString { get; set; }
    }
}
