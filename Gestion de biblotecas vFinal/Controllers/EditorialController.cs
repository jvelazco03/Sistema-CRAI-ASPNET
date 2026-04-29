using DAO_Implement;
using DocumentFormat.OpenXml.Drawing;
using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Gestion_de_Biblioteca.Controllers
{
    public class EditorialController : Controller
    {
        private static EditorialDAO _editorial = new EditorialDAO();

        // TempData["AlertMessage"] = Alert.SweetAlertInfo("Bienvenido");
        public ActionResult Index()
        {
            return View(_editorial.GetList(new Editorial()));
        }
        // Mostrar formulario de creación
        public ActionResult Create()
        {
            return View();
        }

        // Procesar formulario de creación
        [HttpPost]
        public ActionResult Create(Editorial editorial)
        {
            int resultado = _editorial.Insertar(editorial);

            if (resultado == -1)
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("El codigo de editorial ya existe");
                return View(editorial);
            }
            if (resultado == 0)
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("No se pudo registrar la editorial");
                return View(editorial);

            }

            TempData["AlertMessage"] = Alert.SweetAlertSuccess("editorial registrado correctamente");
            return RedirectToAction("Index");
        }

        // Mostrar formulario de edición
        public ActionResult Edit(int id)
        {
            Editorial edi = _editorial.Find(id);
            return View(edi);
        }

        // Procesar edición
        [HttpPost]
        public ActionResult Edit(Editorial editorial)
        {
            int resultado = _editorial.Actualizar(editorial);

            if (resultado == 0)
            {
                TempData["AlertMessage"] = Alert.SweetAlertError("No se pudo Actualizar la editorial");
                return View(editorial);

            }

            TempData["AlertMessage"] = Alert.SweetAlertSuccess("editorial Actualizado correctamente");
            return RedirectToAction("Index");
        }


        // Ver detalles
        public ActionResult Details(int id)
        {
            Editorial edi = _editorial.Find(id);
            return View(edi);
        }
    }
}