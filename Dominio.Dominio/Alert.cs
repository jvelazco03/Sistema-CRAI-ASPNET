using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public static class Alert
    {
        public static string SweetAlertInfo(string text)
        {
            return SweetAlert("Información", text, "info");
        }

        public static string SweetAlertSuccess(string text)
        {
            return SweetAlert("Exitoso", text, "success");
        }

        public static string SweetAlertError(string text)
        {
            return SweetAlert("Error", text, "error");
        }

        public static string SweetAlert(string title, string text, string icon)
        {
            string scriptText = $"<script>Swal.fire({{title: '{Escape(title)}', text: '{Escape(text)}', icon: '{icon}'}});</script>";
            return scriptText;
        }

        private static string Escape(string input)
        {
            return input.Replace("'", "\\'");
        }
    }

}

