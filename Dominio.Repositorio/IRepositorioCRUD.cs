using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Repositorio
{
    public interface IRepositorioCRUD<T> where T : class
    {
        int Insertar(T reg);
        int Actualizar(T reg);
        bool Eliminar(string id);
        T Find(string id);
    }
}
