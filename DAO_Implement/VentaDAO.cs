using Dominio.Dominio;
using Dominio.Dominio.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAO_Implement
{
    public class VentaDAO
    {
        private string cadenaDeConexion = ConexionDAO.cn;

        public bool RegistrarVenta(List<CarritoDTO> lineasCarrito, decimal totalVenta, int idUsuario)
        {
            bool exito = false;
            
            // Usamos bloque USING para asegurar el cierre de la conexión de red
            using (SqlConnection conexion = new SqlConnection(cadenaDeConexion))
            {
                conexion.Open();

                // Arrancamos nuestra Transacción en duro ADO.NET (Como exige el manual)
                using (SqlTransaction transaction = conexion.BeginTransaction())
                {
                    try
                    {
                        // ----- PRIMER PASO: INSERTAR CABECERA -----
                        int idVentaGenerada = 0;
                        string queryCabecera = "INSERT INTO tb_venta_cabecera (Fecha, IdUsuario, Total) VALUES (GETDATE(), @IdUsuario, @Total); SELECT SCOPE_IDENTITY();";
                        
                        using (SqlCommand cmdCabecera = new SqlCommand(queryCabecera, conexion, transaction))
                        {
                            // Parámetros seguros
                            cmdCabecera.Parameters.AddWithValue("@IdUsuario", idUsuario);
                            cmdCabecera.Parameters.AddWithValue("@Total", totalVenta);
                            
                            // ExecuteScalar devuelve el SCOPE_IDENTITY() de la cabecera
                            idVentaGenerada = Convert.ToInt32(cmdCabecera.ExecuteScalar());
                        }

                        // ----- SEGUNDO Y TERCER PASO: INSERTAR DETALLES Y ACTUALIZAR STOCK -----
                        string queryDetalle = "INSERT INTO tb_venta_detalle (IdVenta, CodigoLibro, Cantidad, PrecioUnitario) VALUES (@IdVenta, @CodigoLibro, @Cantidad, @PrecioUnitario);";
                        string queryUpdateStock = "UPDATE tb_libro SET Stock = Stock - @Cantidad WHERE CodigoLibro = @CodigoLibro;";

                        foreach (var item in lineasCarrito)
                        {
                            // Inserción al detalle
                            using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conexion, transaction))
                            {
                                cmdDetalle.Parameters.AddWithValue("@IdVenta", idVentaGenerada);
                                cmdDetalle.Parameters.AddWithValue("@CodigoLibro", item.CodigoLibro);
                                cmdDetalle.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", item.Precio);
                                cmdDetalle.ExecuteNonQuery();
                            }

                            // Actualizar stock restando la cantidad vendida
                            using (SqlCommand cmdUpdate = new SqlCommand(queryUpdateStock, conexion, transaction))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                cmdUpdate.Parameters.AddWithValue("@CodigoLibro", item.CodigoLibro);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }

                        // Si todas las bases de datos respondieron perfectamente (100% Ok), guardamos los cambios permanentes
                        transaction.Commit();
                        exito = true;
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre cualquier mini error de tipo, de conexión, o caída de la BD
                        // Aplicamos el Rollback e invertimos todo para devolver la BD a como estaba
                        transaction.Rollback();
                        throw new Exception("Error transaccional al procesar la venta: " + ex.Message);
                    }
                }
            }

            return exito;
        }
    }
}
