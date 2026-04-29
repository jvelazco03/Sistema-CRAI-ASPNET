using Dominio.Dominio;
using Dominio.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO_Implement
{
    public class LibroDAO : IRepositorioCRUD<Libro>, IRepositorioList<Libro>
    {
        public int Insertar(Libro reg)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
                {
                    SqlCommand cmd = new SqlCommand("usp_insertarLibro", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodigoLibro", reg.CodigoLibro);
                    cmd.Parameters.AddWithValue("@Titulo", reg.Titulo);
                    cmd.Parameters.AddWithValue("@ISBN", reg.ISBN);
                    cmd.Parameters.AddWithValue("@CodigoEditorial", reg.CodigoEditorial);
                    cmd.Parameters.AddWithValue("@Precio", reg.Precio);
                    cmd.Parameters.AddWithValue("@Stock", reg.Stock);
                    cn.Open();
                    resultado = cmd.ExecuteNonQuery();
                    return resultado;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    // Clave duplicada
                    return -1;
                }
            }
            return resultado;
        }
        public int Actualizar(Libro reg)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
                {
                    SqlCommand cmd = new SqlCommand("usp_actualizarLibro", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodigoLibro", reg.CodigoLibro);
                    cmd.Parameters.AddWithValue("@Titulo", reg.Titulo);
                    cmd.Parameters.AddWithValue("@ISBN", reg.ISBN);
                    cmd.Parameters.AddWithValue("@CodigoEditorial", reg.CodigoEditorial);
                    cmd.Parameters.AddWithValue("@Precio", reg.Precio);
                    cmd.Parameters.AddWithValue("@Stock", reg.Stock);
                    cn.Open();
                    resultado = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultado;
        }

        public bool Eliminar(string id)
        {
            bool eliminado = false;
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_elimina_libro", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    eliminado = filasAfectadas > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
            return eliminado;
        }

        public Libro Find(string id)
        {
            Libro reg = null;
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_listarLibroxID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    reg = new Libro
                    {
                        CodigoLibro = dr["CodigoLibro"].ToString(),
                        Titulo = dr["Titulo"].ToString(),
                        ISBN = dr["ISBN"].ToString(),
                        CodigoEditorial = (int)dr["CodigoEditorial"],
                        Editorial = dr["Nombre"].ToString(),
                        Precio = (decimal)dr["Precio"],
                        Stock = (int)dr["Stock"]
                    };
                }
            }
            return reg;
        }

        public IEnumerable<Libro> GetList(Libro reg)
        {
            List<Libro> lista = new List<Libro>();
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_listarLibro", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Libro
                    {
                        CodigoLibro = dr["CodigoLibro"].ToString(),
                        Titulo = dr["Titulo"].ToString(),
                        ISBN = dr["ISBN"].ToString(),
                        Editorial = dr["Nombre"].ToString(),
                        Precio = (decimal)dr["Precio"],
                        Stock = (int)dr["Stock"]
                    });
                }
            }
            return lista;
        }

        public IEnumerable<Libro> ListarLibrosPaginado(string search, int page, int pageSize)
        {
            List<Libro> lista = new List<Libro>();
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_listarLibroPaginado", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@search", search ?? "");
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Libro
                    {
                        CodigoLibro = dr["CodigoLibro"].ToString(),
                        Titulo = dr["Titulo"].ToString(),
                        ISBN = dr["ISBN"].ToString(),
                        Editorial = dr["Editorial"].ToString(),
                        Precio = (decimal)dr["Precio"],
                        Stock = (int)dr["Stock"]
                    });
                }
            }
            return lista;
        }

        public int ContarLibros(string search)
        {
            int total = 0;
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_contarLibros", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@search", search ?? "");
                cn.Open();
                total = (int)cmd.ExecuteScalar();
            }
            return total;
        }

    }
}
