using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using DAO_Implement;
using DocumentFormat.OpenXml.Drawing;
using Dominio.Dominio;
using Gestion_de_Biblioteca.Models;

namespace Gestion_de_Biblioteca.Controllers
{
    [Authorize]
    public class LibroController : Controller
    {
        private static LibroDAO _libro = new LibroDAO();
        private static EditorialDAO _editorial = new EditorialDAO();
        public ActionResult Index(string searchString, int page = 1)
        {
            int pageSize = 5;
            int totalRegistros = _libro.ContarLibros(searchString);
            var libros = _libro.ListarLibrosPaginado(searchString, page, pageSize);

            // Mapeo a DTO (Data Transfer Object)
            var librosDTO = libros.Select(l => new Dominio.Dominio.DTO.LibroDTO
            {
                CodigoLibro = l.CodigoLibro,
                Titulo = l.Titulo,
                ISBN = l.ISBN,
                Editorial = l.Editorial,
                Precio = l.Precio,
                Stock = l.Stock
            }).ToList();

            var model = new LibroPaginadoViewModel
            {
                Libros = librosDTO,
                PaginaActual = page,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize),
                SearchString = searchString
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Crear()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("Acceso denegado. Se requieren permisos de Administrador.");
                return RedirectToAction("Index");
            }

            ViewBag.editorial = new SelectList(_editorial.GetList(new Editorial()), "CodigoEditorial", "Nombre");
            return View();
        }

        [HttpPost]
        public ActionResult Crear(Libro reg)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("Acceso denegado. Se requieren permisos de Administrador.");
                return RedirectToAction("Index");
            }

            int resultado = _libro.Insertar(reg);

            if (resultado == -1)
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("El codigo de libro ya existe");
                ViewBag.editorial = new SelectList(_editorial.GetList(new Editorial()), "CodigoEditorial", "Nombre");
                return View(reg);
            }
            if (resultado == 0)
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("No se pudo registrar el producto");
                ViewBag.editorial = new SelectList(_editorial.GetList(new Editorial()), "CodigoEditorial", "Nombre");
                return View(reg);
               
            }
            
            TempData["AlertMessage"] = Alert.SweetAlertSuccess("Libro registrado correctamente");
            return RedirectToAction("Index");

        }

        public ActionResult Detalle(String id)
        {
            return View(_libro.Find(id));
        }

        [HttpGet]
        public ActionResult Editar(String id)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("Acceso denegado. Se requieren permisos de Administrador.");
                return RedirectToAction("Index");
            }

            ViewBag.editorial = new SelectList(_editorial.GetList(new Editorial()), "CodigoEditorial", "Nombre");
            return View(_libro.Find(id));
        }

        [HttpPost]
        public ActionResult Editar(Libro reg)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("Acceso denegado. Se requieren permisos de Administrador.");
                return RedirectToAction("Index");
            }

            int resultado = _libro.Actualizar(reg);

            if (resultado == 0)
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("No se pudo actualizar el producto");
                ViewBag.editorial = new SelectList(_editorial.GetList(new Editorial()), "CodigoEditorial", "Nombre");
                return View(reg);

            }

            TempData["AlertMessage"] = Alert.SweetAlertSuccess("Libro registrado correctamente");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Eliminar(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Seguridad de Rol: Solo Admin
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("No tienes permisos de Administrador para eliminar libros.");
                return RedirectToAction("Index");
            }

            Libro libro = _libro.Find(id);

            if (libro == null)
            {
                return HttpNotFound();
            }
            return View(_libro.Find(id));
        }

        [HttpPost, ActionName("Eliminar")]
        public ActionResult EliminarC(String id)
        {
            // Seguridad de Rol: Solo Admin
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("No tienes permisos de Administrador para eliminar libros.");
                return RedirectToAction("Index");
            }

            try 
            {
                bool resultado = _libro.Eliminar(id);

                if (resultado)
                {
                    TempData["AlertMessage"] = Alert.SweetAlertSuccess("Libro eliminado correctamente");
                    return RedirectToAction("Index");
                }
                
                TempData["AlertMessage"] = Alert.SweetAlertError("No se encontró el libro a eliminar.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE") || ex.Message.Contains("FK_")) 
                {
                    TempData["AlertMessage"] = Alert.SweetAlertError("No se puede eliminar el libro porque tiene un Registro de Circulación (Ventas/Reservas) asociado en el sistema.");
                } 
                else 
                {
                    TempData["AlertMessage"] = Alert.SweetAlertError("Error de Base de Datos: " + ex.Message);
                }
                return RedirectToAction("Index");
            }
        }


        public ActionResult Importar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Importar(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel != null && archivoExcel.ContentLength > 0)
            {
                var existentes = _libro.GetList(new Libro()).Select(l => l.CodigoLibro).ToHashSet();
                List<string> errores = new List<string>();

                try
                {
                    using (var workbook = new XLWorkbook(archivoExcel.InputStream))
                    {
                        var hoja = workbook.Worksheet(1);
                        int fila = 2, importados = 0, duplicados = 0, conErrores = 0;

                        while (!hoja.Cell(fila, 1).IsEmpty())
                        {
                            try
                            {
                                string cod = hoja.Cell(fila, 1).GetString();
                                if (existentes.Contains(cod))
                                {
                                    duplicados++;
                                    fila++;
                                    continue;
                                }

                                var libro = new Libro
                                {
                                    CodigoLibro = cod,
                                    Titulo = hoja.Cell(fila, 2).GetString(),
                                    ISBN = hoja.Cell(fila, 3).GetString(),
                                    CodigoEditorial = int.Parse(hoja.Cell(fila, 4).GetString()),
                                    Precio = decimal.Parse(hoja.Cell(fila, 5).GetString()),
                                    Stock = int.Parse(hoja.Cell(fila, 6).GetString())
                                };
                                System.Diagnostics.Debug.WriteLine("Insertando libro: " + cod);
                                _libro.Insertar(libro);
                                importados++;
                            }
                            catch (Exception ex)
                            {
                                errores.Add($"Fila {fila}: {ex.Message}");
                                conErrores++;
                            }

                            fila++;
                        }

                        TempData["mensaje"] = $"Importación: {importados} OK, {duplicados} duplicados, {conErrores} errores.";
                        if (errores.Any())
                        {
                            string ruta = Server.MapPath("~/App_Data/ErroresImportacion.txt");
                            System.IO.File.WriteAllLines(ruta, errores);
                            TempData["logErrores"] = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["mensaje"] = "Error al procesar archivo: " + ex.Message;
                }
            }
            else
            {
                TempData["mensaje"] = "Seleccione un archivo válido.";
            }

            return RedirectToAction("Index");
        }

         
        public ActionResult Exportar()
        {
            var libros = _libro.GetList(new Libro());

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Libros");
                ws.Cell(1, 1).Value = "CodigoLibro";
                ws.Cell(1, 2).Value = "Titulo";
                ws.Cell(1, 3).Value = "ISBN";
                ws.Cell(1, 4).Value = "CodigoEditorial";
                ws.Cell(1, 5).Value = "Precio";
                ws.Cell(1, 6).Value = "Stock";

                int fila = 2;
                foreach (var libro in libros)
                {
                    ws.Cell(fila, 1).Value = libro.CodigoLibro;
                    ws.Cell(fila, 2).Value = libro.Titulo;
                    ws.Cell(fila, 3).Value = libro.ISBN;
                    ws.Cell(fila, 4).Value = libro.CodigoEditorial;
                    ws.Cell(fila, 5).Value = libro.Precio;
                    ws.Cell(fila, 6).Value = libro.Stock;
                    fila++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Libros.xlsx");
                }
            }
        }


    }
}