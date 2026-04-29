using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Repositorio
{
    public interface IRepositorioList<T> where T : class
    {
        IEnumerable<T> GetList(T reg);
    }
}
