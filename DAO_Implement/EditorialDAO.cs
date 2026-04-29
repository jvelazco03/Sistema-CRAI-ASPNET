using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Dominio;
using Dominio.Repositorio;

namespace DAO_Implement
{
    public class EditorialDAO : IRepositorioList<Editorial>
    {
        public IEnumerable<Editorial> GetList(Editorial reg)
        {
            List<Editorial> lista = new List<Editorial>();
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_listarEditorial", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Editorial
                    {
                        CodigoEditorial = (int)dr["CodigoEditorial"],
                        Nombre = dr["Nombre"].ToString(),
                        Pais = dr["Pais"].ToString()
                    });
                }
            }
            return lista;
        }
        public int Insertar(Editorial reg)
        {
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_insertarEditorial", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodigoEditorial", reg.CodigoEditorial);
                cmd.Parameters.AddWithValue("@Nombre", reg.Nombre);
                cmd.Parameters.AddWithValue("@Pais", reg.Pais);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public int Actualizar(Editorial reg)
        {
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_actualizarEditorial", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodigoEditorial", reg.CodigoEditorial);
                cmd.Parameters.AddWithValue("@Nombre", reg.Nombre);
                cmd.Parameters.AddWithValue("@Pais", reg.Pais);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public Editorial Find(int id)
        {
            Editorial edi = null;
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM tb_editorial WHERE CodigoEditorial = @id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    edi = new Editorial
                    {
                        CodigoEditorial = (int)dr["CodigoEditorial"],
                        Nombre = dr["Nombre"].ToString(),
                        Pais = dr["Pais"].ToString()
                    };
                }
            }
            return edi;
        }
    }

    

}
