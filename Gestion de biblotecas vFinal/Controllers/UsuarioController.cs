using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Gestion_de_Biblioteca.Models;
using DAO_Implement;
using Dominio.Dominio;

namespace Gestion_de_Biblioteca.Controllers
{
    public class UsuarioController : Controller
    {
        private UsuarioDAO usuarioDAO = new UsuarioDAO();

        // GET: Usuario/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario oUsuario = usuarioDAO.ValidarUsuario(modelo.Usuario, modelo.Password);

                    if (oUsuario != null)
                    {
                        FormsAuthentication.SetAuthCookie(oUsuario.NombreUsuario, false);

                        Session["NombreUsuario"] = oUsuario.NombreUsuario;
                        Session["IdUsuario"] = oUsuario.IdUsuario;
                        
                        if (oUsuario.Rol != null)
                        {
                            Session["Rol"] = oUsuario.Rol.Nombre;
                        }

                        return RedirectToAction("Index", "Libro");
                    }
                    else
                    {
                        ViewBag.MensajeError = $"Login Denegado. Diagnóstico - Usuario intentó entrar con: [{modelo.Usuario}] y clave: [{modelo.Password}].";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.MensajeError = "Error Crítico de Base de Datos: " + ex.Message;
                }
            }

            // Si hay error en validación del modelo o falló la autenticación
            return View(modelo);
        }

        // GET: Usuario/Salir
        [HttpGet]
        public ActionResult Salir()
        {
            // Cerramos la sesión de FormsAuthentication
            FormsAuthentication.SignOut();
            
            // Limpieza de las variables de sesión creadas
            Session.Clear();
            Session.Abandon();

            // Redirigir devuelta a la vista de Login
            return RedirectToAction("Login", "Usuario");
        }
    }
}
