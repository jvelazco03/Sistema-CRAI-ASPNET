using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO_Implement
{
    internal class ConexionDAO
    {
        public static string cn = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        private static void LlenarParametros(SqlCommand cmd,
                          params object[] valoresParametros)
        {
            int indice = 0;
            SqlCommandBuilder.DeriveParameters(cmd);
            // recorrer cada parámetro y asignarle su valor
            foreach (SqlParameter prm in cmd.Parameters)
            {
                // si el nombre del parámetro es diferente a
                // "@RETURN_VALUE", entonces le asignaremos el elemento
                // correspondiente del array de valores (según indice)
                if (prm.ParameterName != "@RETURN_VALUE")
                {
                    prm.Value = valoresParametros[indice];
                    indice++;
                }
            }
        }

        // Método para el CRUD (Insertar, Actualizar y "Eliminar")
        public static void EjecutarSP(string nombreSP,
                          params object[] valoresParametros)
        {
            using (SqlConnection cnx = new SqlConnection(cn))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(nombreSP, cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                //
                if (valoresParametros.Length > 0)
                    LlenarParametros(cmd, valoresParametros);
                //
                cmd.ExecuteNonQuery();
            }
        }

        // Método para cualquier consulta, listado, etc (Select)
        public static DataTable EjecutarSPDataTable(string nombreSP,
                          params object[] valoresParametros)
        {
            DataTable Tabla = new DataTable();
            using (SqlDataAdapter adap = new SqlDataAdapter(nombreSP, cn))
            {
                adap.SelectCommand.Connection.Open();
                adap.SelectCommand.CommandType = CommandType.StoredProcedure;
                //
                if (valoresParametros.Length > 0)
                    LlenarParametros(adap.SelectCommand, valoresParametros);
                // poblar el datatable
                adap.Fill(Tabla);
            }
            //
            return Tabla;
        }
    }
}
