using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO_Implement
{
    public class UsuarioDAO
    {
        // Reutilizamos la cadena de conexión centralizada de ConexionDAO
        private string cadenaDeConexion = ConexionDAO.cn;

        /// <summary>
        /// Valida las credenciales de un usuario en la base de datos.
        /// Retorna un objeto Usuario con los datos y el rol correspondiente si las credenciales son correctas.
        /// </summary>
        public Usuario ValidarUsuario(string login, string password)
        {
            Usuario oUsuario = null;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeConexion))
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("usp_validarUsuario", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NombreUsuario", login != null ? login.Trim() : "");
                        cmd.Parameters.AddWithValue("@Clave", password != null ? password.Trim() : "");

                        // Utilizamos SqlDataReader para leer la fila
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oUsuario = new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                    NombreUsuario = reader["NombreUsuario"].ToString(),
                                    IdRol = Convert.ToInt32(reader["IdRol"]),
                                    Rol = new Rol
                                    {
                                        Nombre = reader["RolNombre"] != DBNull.Value ? reader["RolNombre"].ToString() : "Desconocido"
                                    }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error crítico Login ADO.NET: " + ex.Message);
            }

            return oUsuario;
        }
    }
}
