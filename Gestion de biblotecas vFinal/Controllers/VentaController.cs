using DAO_Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Dominio;
using Dominio.Dominio.DTO;
using Gestion_de_Biblioteca.Models;

namespace Gestion_de_biblotecas_vFinal.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        /// <summary>
        /// Muestra la interfaz principal del carrito de compras.
        /// Transita los DTOs cacheados hacia el modelo visual VentaViewModel.
        /// </summary>
        /// <returns>Vista renderizada del carrito</returns>
        [HttpGet]
        public ActionResult VerCarrito()
        {
            var modeloVenta = new VentaViewModel();
            
            // Si la sesión del carrito ya existe, la cargamos en el modelo
            if (Session["Carrito"] != null)
            {
                modeloVenta.LineasCarrito = (List<CarritoDTO>)Session["Carrito"];
            }

            return View(modeloVenta);
        }

        /// <summary>
        /// Analiza e incorpora un producto a la memoria temporal (Session).
        /// Si el libro ya existe en el carrito, se amalgaman las cantidades.
        /// </summary>
        /// <param name="codigo">ID primario del libro</param>
        /// <param name="titulo">Nombre literario del libro</param>
        /// <param name="precio">Costo unitario extraído de BDD</param>
        /// <param name="cantidad">Ejemplares a sustraer</param>
        /// <returns>Refresco de la pantalla de carrito</returns>
        [HttpPost]
        public ActionResult AgregarCarrito(string codigo, string titulo, decimal precio, int cantidad)
        {
            List<CarritoDTO> carrito;

            // Paso 1: Verificar si existe el Carrito en sesión
            if (Session["Carrito"] == null)
            {
                carrito = new List<CarritoDTO>();
            }
            else
            {
                carrito = (List<CarritoDTO>)Session["Carrito"];
            }

            // Paso 2: Buscar si el libro ya fue metido al carrito previamente
            var itemExistente = carrito.FirstOrDefault(x => x.CodigoLibro == codigo);
            
            if (itemExistente != null)
            {
                // Solo acumulamos la cantidad, no duplicamos registros
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                // Si es nuevo, lo creamos y anexamos a la lista
                carrito.Add(new CarritoDTO
                {
                    CodigoLibro = codigo,
                    Titulo = titulo,
                    Precio = precio,
                    Cantidad = cantidad
                });
            }

            // Paso 3: Salvaguardar nuevamente la información en memoria de servidor
            Session["Carrito"] = carrito;

            // Enviamos un temporizador/alerta a la interfaz
            TempData["AlertMessage"] = Alert.SweetAlertSuccess("Libro agregado al carrito");

            // Redirigir a la vista del carrito
            return RedirectToAction("VerCarrito");
        }
        
        /// <summary>
        /// Punto de anclaje de legacy. Redirige el tráfico genérico al carrito.
        /// </summary>
        /// <returns>Redirección (302)</returns>
        public ActionResult Index()
        {
            return RedirectToAction("VerCarrito");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuitarCarrito(string codigo)
        {
            if (Session["Carrito"] != null)
            {
                var carrito = (List<CarritoDTO>)Session["Carrito"];
                var item = carrito.FirstOrDefault(x => x.CodigoLibro == codigo);
                if (item != null)
                {
                    carrito.Remove(item);
                    Session["Carrito"] = carrito;
                    TempData["AlertMessage"] = Alert.SweetAlertInfo("Material retirado de la bandeja");
                }
            }
            return RedirectToAction("VerCarrito");
        }

        /// <summary>
        /// Orquesta el "Checkout" validando los tokens y variables de sesión.
        /// Invoca a VentaDAO de forma segura para garantizar la transacción ACID de la compra.
        /// Vacia la cesta en caso de éxito.
        /// </summary>
        /// <returns>Redirección asertiva basada en el Commit o Rollback SQL</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcesarVenta()
        {
            try
            {
                // Validación del Carrito
                if (Session["Carrito"] == null)
                {
                    TempData["AlertMessage"] = Alert.SweetAlertError("El carrito se encuentra vacío.");
                    return RedirectToAction("VerCarrito");
                }

                // Generar instancia de DTOs del Carrito
                List<CarritoDTO> listaCarrito = (List<CarritoDTO>)Session["Carrito"];
                
                // Extraer IdUsuario de la sesión validada
                if (Session["IdUsuario"] == null)
                {
                    TempData["AlertMessage"] = Alert.SweetAlertError("La sesión ha expirado o no se pudo validar tu usuario.");
                    return RedirectToAction("Login", "Usuario");
                }

                int idUsuario = Convert.ToInt32(Session["IdUsuario"]);

                // Re-calcular total por seguridad backend para inyectar al DAO
                decimal totalVenta = listaCarrito.Sum(x => x.Importe);

                // Ejecución Transaccional del DAO
                VentaDAO ventaDAO = new VentaDAO();
                bool exito = ventaDAO.RegistrarVenta(listaCarrito, totalVenta, idUsuario);

                if (exito)
                {
                    // Fase final manual: Vaciamos el Carrito global
                    Session["Carrito"] = null;

                    // Alerta nativa de éxito y ruta al Catálogo
                    TempData["AlertMessage"] = Alert.SweetAlertSuccess("¡Venta registrada con éxito!");
                    return RedirectToAction("Index", "Libro");
                }
                else
                {
                    TempData["AlertMessage"] = Alert.SweetAlertError("Hubo un error silencioso de base de datos.");
                    return RedirectToAction("VerCarrito");
                }
            }
            catch (Exception ex)
            {
                // Mostrando captura estricta del Transaction.Rollback() a la vista sin romperla
                TempData["AlertMessage"] = Alert.SweetAlertError(ex.Message);
                return RedirectToAction("VerCarrito");
            }
        }
    }
}