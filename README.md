# SubscriptionsAPI
Proyecto WebApi realizado para pruebas, en el cual se pueden crear autores, libros y comentarios para esos libros, también tiene un sistema de autenticación y autorización para poder ser consumido para pruebas desde cualquier cliente, usa local db como base de datos al no ser un proyecto muy grande. Adicioonalmente, tiene una funcionalidad para crear API key emulando un sistema de pago por petición realizada el cual se manejará desde base de datos.

### Tecnologías Utilizadas
:keyboard: C# 10  
:keyboard: .Net 6  
:computer: Visual Studio 2022  

### :open_book: Configuración  
1. En una carpeta del sistema ejecutar el comando :arrow_forward: git clone https://github.com/andresali1/SubscriptionsAPI.git
2. Abrir la solución (WebApiAuthors.sln) con Visual Studio 2022:
3. Verificar en el AppSettings.json o AppSettings.Development.json el string de conexión y colocar un string válido. El nombre de la base de datos debe ser "WebApiAuthors", no es necesario crearla pero si colocar su nombre en el string de conexión.
4. Con SQL server Management Studio ejecutar los 2 procedimientos almacenados ubicados dentro de la carpeta "StoredProcedures".
5. Ubicarse dentro de la carpeta "WebApiAuthors" y usando la consola del administrador de paquetes escribir el comando "Update-Database" si se usa Visual estudio, o si se usa dotnet cli usar el comando "dotnet ef database update"
6.  Ejecutar el proyecto

