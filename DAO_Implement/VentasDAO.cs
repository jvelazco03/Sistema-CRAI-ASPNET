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
    public class VentasDAO : IRepositorioList<Venta>
    {
        public IEnumerable<Venta> GetList(Venta reg)
        {
            List<Venta> lista = new List<Venta>();
            using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
            {
                SqlCommand cmd = new SqlCommand("listadoVenta", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Venta
                    {
                        idVenta = (int)dr["idVenta"],
                        CodigoLibro = dr["CodigoLibro"].ToString(),
                        Cantidad = (int)dr["Cantidad"],
                        PrecioUnitario = (decimal)dr["PrecioUnitario"],
                        Total = (decimal)dr["Total"],
                        FechaVenta = (DateTime)dr["FechaVenta"],
                    });
                }
            }
            return lista;
        }

        public int InsertarVenta(string id, int cantidad)
        {
            int resultado = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(ConexionDAO.cn))
                {
                    SqlCommand cmd = new SqlCommand("usp_registrarVenta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodigoLibro", id);
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);

                    cn.Open();
                    resultado = cmd.ExecuteNonQuery();
                    return resultado;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50001 || ex.Number == 50002)
                {
                    Console.WriteLine("Error de negocio: " + ex.Message);
                    return -2;
                }

                Console.WriteLine("Error SQL inesperado: " + ex.Message);
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general: " + ex.Message);
                return -3;

            }
        }
    }
}
