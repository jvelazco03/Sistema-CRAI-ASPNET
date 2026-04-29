USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Biblioteca')
BEGIN
    ALTER DATABASE Biblioteca SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Biblioteca;
END
GO

CREATE DATABASE Biblioteca;
GO

USE Biblioteca;
GO

-- 1. TABLA ROL
CREATE TABLE tb_rol (
    IdRol INT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);
GO

-- 2. TABLA USUARIO
CREATE TABLE tb_usuario (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Clave VARCHAR(100) NOT NULL,
    IdRol INT NOT NULL FOREIGN KEY REFERENCES tb_rol(IdRol),
    Activo BIT DEFAULT 1
);
GO

-- 3. TABLA EDITORIAL
CREATE TABLE tb_editorial (
    CodigoEditorial INT PRIMARY KEY,
    Nombre VARCHAR(150),
    Pais VARCHAR(150)
);
GO

-- 4. TABLA LIBRO
CREATE TABLE tb_libro (
    CodigoLibro VARCHAR(7) PRIMARY KEY,
    Titulo VARCHAR(100),
    ISBN VARCHAR(50),
    CodigoEditorial INT FOREIGN KEY REFERENCES tb_editorial(CodigoEditorial),
    Precio DECIMAL(10,2),
    Stock INT
);
GO

-- 5. TABLAS VENTA
CREATE TABLE tb_venta_cabecera (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    IdUsuario INT NOT NULL FOREIGN KEY REFERENCES tb_usuario(IdUsuario),
    Total DECIMAL(10,2) NOT NULL
);
GO

CREATE TABLE tb_venta_detalle (
    IdVenta INT NOT NULL FOREIGN KEY REFERENCES tb_venta_cabecera(IdVenta),
    CodigoLibro VARCHAR(7) NOT NULL FOREIGN KEY REFERENCES tb_libro(CodigoLibro),
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (IdVenta, CodigoLibro)
);
GO

-- DATOS DE PRUEBA: ROLES Y USUARIOS
INSERT INTO tb_rol (IdRol, Nombre) VALUES 
(1, 'Admin'), 
(2, 'Bibliotecario'), 
(3, 'Vendedor');
GO

INSERT INTO tb_usuario (NombreUsuario, Clave, IdRol) VALUES 
('admin', 'admin123', 1),
('bibliotecario', 'biblio123', 2),
('user01', 'user123', 3);
GO

-- DATOS DE PRUEBA: EDITORIALES Y LIBROS
INSERT INTO tb_editorial (CodigoEditorial, Nombre, Pais) VALUES 
(1, 'Editorial Uno', 'Espana'),
(2, 'Editorial Dos', 'Mexico'),
(3, 'Editorial Tres', 'Argentina'),
(4, 'Editorial Cuatro', 'Colombia'),
(5, 'Editorial Cinco', 'Chile');
GO

INSERT INTO tb_libro (CodigoLibro, Titulo, ISBN, CodigoEditorial, Precio, Stock) VALUES 
('LIB001', 'El libro de la selva', '978-3-16-148410-0', 1, 15.99, 100),
('LIB002', 'Cien anos de soledad', '978-3-16-148410-1', 2, 20.50, 50),
('LIB003', 'Don Quijote de la Mancha', '978-3-16-148410-2', 3, 25.00, 30),
('LIB004', 'La casa de los espiritus', '978-3-16-148410-3', 4, 18.75, 20),
('LIB005', 'El tunel', '978-3-16-148410-4', 5, 22.30, 15);
GO

-- PROCEDIMIENTOS ALMACENADOS

-- 1. LOGIN
CREATE OR ALTER PROCEDURE usp_validarUsuario
    @NombreUsuario VARCHAR(50),
    @Clave VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        u.IdUsuario, 
        u.NombreUsuario, 
        u.IdRol, 
        r.Nombre AS RolNombre
    FROM tb_usuario u
    INNER JOIN tb_rol r ON u.IdRol = r.IdRol
    WHERE u.NombreUsuario = @NombreUsuario AND u.Clave = @Clave AND u.Activo = 1;
END;
GO

-- 2. EDITORIALES
CREATE OR ALTER PROCEDURE usp_listarEditorial
AS
BEGIN
    SELECT * FROM tb_editorial;
END;
GO

CREATE OR ALTER PROCEDURE usp_insertarEditorial
    @CodigoEditorial INT,
    @Nombre VARCHAR(150),
    @Pais VARCHAR(150)
AS
BEGIN
    INSERT INTO tb_editorial VALUES (@CodigoEditorial, @Nombre, @Pais);
END;
GO

CREATE OR ALTER PROCEDURE usp_actualizarEditorial
    @CodigoEditorial INT,
    @Nombre VARCHAR(150),
    @Pais VARCHAR(150)
AS
BEGIN
    UPDATE tb_editorial
    SET Nombre = @Nombre,
        Pais = @Pais
    WHERE CodigoEditorial = @CodigoEditorial;
END;
GO

-- 3. LIBROS
CREATE OR ALTER PROCEDURE usp_listarLibro
AS
BEGIN
    SELECT l.CodigoLibro, l.Titulo, l.ISBN, e.Nombre, l.Precio, l.Stock  
    FROM tb_libro l 
    INNER JOIN tb_editorial e ON l.CodigoEditorial = e.CodigoEditorial;
END;
GO

CREATE OR ALTER PROCEDURE usp_insertarLibro
    @CodigoLibro VARCHAR(7),
    @Titulo VARCHAR(100),
    @ISBN VARCHAR(50),
    @CodigoEditorial INT,
    @Precio DECIMAL(10,2),
    @Stock INT
AS
BEGIN
    INSERT INTO tb_libro VALUES (@CodigoLibro, @Titulo, @ISBN, @CodigoEditorial, @Precio, @Stock);
END;
GO

CREATE OR ALTER PROCEDURE usp_listarLibroxID
    @id varchar(7)
AS 
BEGIN
    SELECT l.CodigoLibro, l.Titulo, l.ISBN, l.CodigoEditorial, e.Nombre, l.Precio, l.Stock 
    FROM tb_libro l
    INNER JOIN tb_editorial e ON l.CodigoEditorial = e.CodigoEditorial
    WHERE CodigoLibro = @id;
END; 
GO

CREATE OR ALTER PROCEDURE usp_actualizarLibro
    @CodigoLibro VARCHAR(7),
    @Titulo VARCHAR(100),
    @ISBN VARCHAR(50),
    @CodigoEditorial INT,
    @Precio DECIMAL(10,2),
    @Stock INT
AS
BEGIN
    UPDATE tb_libro
    SET Titulo = @Titulo,
        ISBN = @ISBN,
        CodigoEditorial = @CodigoEditorial,
        Precio = @Precio,
        Stock = @Stock
    WHERE CodigoLibro = @CodigoLibro;
END;
GO

CREATE OR ALTER PROCEDURE usp_elimina_libro
    @id VARCHAR(7)
AS
BEGIN
    DELETE FROM tb_libro
    WHERE CodigoLibro = @id;
END;
GO

CREATE OR ALTER PROCEDURE usp_listarLibroPaginado
    @search VARCHAR(100) = '',
    @page INT = 1,
    @pageSize INT = 5
AS
BEGIN
    SELECT l.CodigoLibro, l.Titulo, l.ISBN, e.Nombre AS Editorial, l.Precio, l.Stock
    FROM tb_libro l
    INNER JOIN tb_editorial e ON l.CodigoEditorial = e.CodigoEditorial
    WHERE l.Titulo LIKE '%' + @search + '%' OR l.CodigoLibro LIKE '%' + @search + '%'
    ORDER BY l.CodigoLibro
    OFFSET (@page - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;
END;
GO

CREATE OR ALTER PROCEDURE usp_contarLibros
    @search VARCHAR(100) = ''
AS
BEGIN
    SELECT COUNT(*) 
    FROM tb_libro 
    WHERE Titulo LIKE '%' + @search + '%' OR CodigoLibro LIKE '%' + @search + '%';
END;
GO
