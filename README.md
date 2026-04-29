# 🏛️ CRAI - Sistema de Gestión Bibliotecaria

Bienvenido al repositorio del **Centro de Recursos para el Aprendizaje y la Investigación (CRAI)**. 
Este es un proyecto académico de nivel universitario construido con **ASP.NET MVC (C#)** y **SQL Server**, orientado a la gestión avanzada de acervos bibliográficos, reservas transaccionales y seguridad basada en roles (RBAC).

---

## 🚀 Características Principales

*   **Arquitectura Limpia**: Desarrollado bajo un patrón de diseño en capas (Capa Visual MVC, Capa Lógica, Capa de Acceso a Datos DAO).
*   **Seguridad RBAC**: Control de acceso granular. Solo los administradores pueden realizar operaciones destructivas (CRUD de libros), mientras que los estudiantes acceden a la bandeja de reservas.
*   **Transacciones Seguras**: Implementación de `SqlTransaction` (ACID) para el procesamiento de reservas (checkout) y control de integridad relacional (Evita la eliminación de libros con historial).
*   **Diseño Premium (Soft UI)**: Interfaz de usuario rediseñada desde cero usando CSS nativo, paleta corporativa (Azul Marino y Oro), micro-interacciones (hover effects), Google Fonts (Inter) y un sistema moderno de grilla de tarjetas.
*   **Experiencia Reactiva**: Alertas dinámicas implementadas con `SweetAlert2` para notificaciones limpias sin recargas molestas.

---

## 🛠️ Stack Tecnológico

*   **Backend**: C# (ASP.NET MVC 5)
*   **Base de Datos**: Microsoft SQL Server (ADO.NET Nativo, Stored Procedures)
*   **Frontend**: Razor (HTML5), Vanilla CSS (Soft UI), Bootstrap 5
*   **Iconografía**: FontAwesome 6 y Bootstrap Icons
*   **Alertas**: SweetAlert2

---

## ⚙️ Instrucciones de Instalación

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/TU_USUARIO/TU_REPOSITORIO.git
    ```
2.  **Preparar la Base de Datos:**
    *   Abre SQL Server Management Studio (SSMS).
    *   Abre el archivo `Libreria.sql` ubicado en la raíz del proyecto.
    *   Ejecuta todo el script de golpe (`F5`). Esto creará la base de datos `bdLibreria`, las tablas, las llaves foráneas y todos los Procedimientos Almacenados (Stored Procedures).
3.  **Configurar la Cadena de Conexión:**
    *   Abre la Solución en **Visual Studio**.
    *   Ve al proyecto de Acceso a Datos (`DAO_Implement`).
    *   Edita la clase `ConexionDAO.cs` y asegúrate de que el `Server` coincida con el nombre de tu instancia local de SQL Server.
4.  **Ejecución:**
    *   Compila la solución (`Ctrl + Shift + B`) para restaurar cualquier paquete NuGet.
    *   Ejecuta el proyecto (`F5`).

---

## 🔐 Usuarios de Prueba (Por Defecto)

El script SQL inyecta los siguientes usuarios de prueba para que puedas explorar todos los módulos del sistema:

| Rol | Usuario | Contraseña | Permisos Destacados |
| :--- | :--- | :--- | :--- |
| **Administrador** | `admin` | `admin123` | Control total. Crear/Editar/Eliminar libros, ver historial. |
| **Bibliotecario** | `bibliotecario` | `biblio123` | Gestión intermedia. |
| **Estudiante/Alumno** | `user01` | `user123` | Explorar catálogo, agregar a bandeja, procesar reserva. |

---

## 📸 Estética del Proyecto

El sistema ha superado las convenciones básicas de plantillas MVC, implementando:
- *Tarjetas de Libros interactivas con sombras dinámicas.*
- *Badges de estado de inventario.*
- *Tema oscuro de alto contraste y legibilidad profunda.*

> **Nota Académica**: Este proyecto cumple rigurosamente con los estándares de "Desarrollo de Servicios Web 1", demostrando competencia en el manejo de sesiones de usuario, excepciones SQL, persistencia en memoria y separación de responsabilidades.
